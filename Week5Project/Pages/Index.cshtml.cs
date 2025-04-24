using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Week5Project.Models;
using Week5Project.Helpers;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;

namespace Week5Project.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassInformation = new List<ClassInformationModel>();

        static IndexModel()
        {
            if (ClassInformation.Count == 0)
            {
                for (int i = 1; i <= 100; i++)
                {
                    ClassInformation.Add(new ClassInformationModel
                    {
                        Id = i,
                        ClassName = $"Class {i}",
                        StudentCount = 10 + (i % 30),
                        Description = $"Description for class {i}"
                    });
                }
            }
        }

        [BindProperty]
        public string ClassName { get; set; } = string.Empty;

        [BindProperty]
        public int StudentCount { get; set; }

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        public ClassInformationTable ClassTable { get; set; } = new ClassInformationTable();

        public IActionResult OnGet(int currentPage = 1, int pageSize = 10, string filterClassName = null)
        {
            if (!IsAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            LoadTableData(currentPage, pageSize, filterClassName);
            return Page();
        }

        private bool IsAuthenticated()
        {
            var usernameFromSession = HttpContext.Session.GetString("username");
            var tokenFromSession = HttpContext.Session.GetString("token");
            var sessionIdFromSession = HttpContext.Session.GetString("session_id");

            var usernameFromCookie = Request.Cookies["username"];
            var tokenFromCookie = Request.Cookies["token"];
            var sessionIdFromCookie = Request.Cookies["session_id"];

            return !string.IsNullOrEmpty(usernameFromSession) &&
                   !string.IsNullOrEmpty(tokenFromSession) &&
                   !string.IsNullOrEmpty(sessionIdFromSession) &&
                   usernameFromSession == usernameFromCookie &&
                   tokenFromSession == tokenFromCookie &&
                   sessionIdFromSession == sessionIdFromCookie;
        }

        private void LoadTableData(int currentPage, int pageSize, string filterClassName)
        {
            var query = ClassInformation.AsQueryable();

            if (!string.IsNullOrEmpty(filterClassName))
            {
                query = query.Where(c => c.ClassName.Contains(filterClassName, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = query.Count();
            var classes = query
                .OrderBy(c => c.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ClassTable = new ClassInformationTable
            {
                Classes = classes,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = totalCount,
                FilterClassName = filterClassName
            };
        }

        public IActionResult OnPostAdd()
        {
            if (!IsAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                LoadTableData(1, ClassTable.PageSize, ClassTable.FilterClassName);
                return Page();
            }

            var newClass = new ClassInformationModel
            {
                Id = ClassInformation.Any() ? ClassInformation.Max(c => c.Id) + 1 : 1,
                ClassName = ClassName,
                StudentCount = StudentCount,
                Description = Description
            };

            ClassInformation.Add(newClass);
            return RedirectToPage(new { 
                currentPage = ClassTable.CurrentPage,
                pageSize = ClassTable.PageSize,
                filterClassName = ClassTable.FilterClassName
            });
        }

        public IActionResult OnPostDelete(int id)
        {
            if (!IsAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            var classToDelete = ClassInformation.FirstOrDefault(c => c.Id == id);
            if (classToDelete != null)
            {
                ClassInformation.Remove(classToDelete);
            }
            return RedirectToPage(new { 
                currentPage = ClassTable.CurrentPage,
                pageSize = ClassTable.PageSize,
                filterClassName = ClassTable.FilterClassName
            });
        }

        public IActionResult OnPostEdit(int id)
        {
            if (!IsAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            var classToEdit = ClassInformation.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                ClassName = classToEdit.ClassName;
                StudentCount = classToEdit.StudentCount;
                Description = classToEdit.Description;
                ClassInformation.Remove(classToEdit);
            }
            
            LoadTableData(ClassTable.CurrentPage, ClassTable.PageSize, ClassTable.FilterClassName);
            return Page();
        }

        public IActionResult OnGetExportVisibleData(string filterClassName, int currentPage, int pageSize)
        {
            if (!IsAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            var query = ClassInformation.AsQueryable();

            if (!string.IsNullOrEmpty(filterClassName))
            {
                query = query.Where(c => c.ClassName.Contains(filterClassName, StringComparison.OrdinalIgnoreCase));
            }

            var data = query
                .OrderBy(c => c.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new {
                    c.Id,
                    c.ClassName,
                    c.StudentCount,
                    c.Description,
                    ExportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToList();

            return new JsonResult(data);
        }
    }
}