using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Week5Project.Models;
using System.Collections.Generic;
using System.Linq;

namespace Week5Project.Pages
{
    public class IndexModel : PageModel
    {
        // Static list to act as an in-memory database
        public static List<ClassInformationModel> ClassInformation = new List<ClassInformationModel>();

        // Initialize with sample data if empty
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

        // Properties for form data binding
        [BindProperty]
        public string ClassName { get; set; }

        [BindProperty]
        public int StudentCount { get; set; }

        [BindProperty]
        public string Description { get; set; }

        // Table model for display
        public ClassInformationTable ClassTable { get; set; }

        // OnGet method with filtering and pagination
        public void OnGet(int currentPage = 1, int pageSize = 10, string filterClassName = null)
        {
            // Apply filtering
            var query = ClassInformation.AsQueryable();

            if (!string.IsNullOrEmpty(filterClassName))
            {
                query = query.Where(c => c.ClassName.Contains(filterClassName));
            }

            // Apply pagination
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

        // OnPost method to handle form submission (Add operation)
        public IActionResult OnPostAdd()
        {
            if (ModelState.IsValid)
            {
                var newClass = new ClassInformationModel
                {
                    ClassName = ClassName,
                    StudentCount = StudentCount,
                    Description = Description
                };

                // Generate new ID
                newClass.Id = ClassInformation.Any() ? ClassInformation.Max(c => c.Id) + 1 : 1;

                ClassInformation.Add(newClass);
                return RedirectToPage();
            }
            return Page();
        }

        // OnPost method to handle delete operation
        public IActionResult OnPostDelete(int id)
        {
            var classToDelete = ClassInformation.FirstOrDefault(c => c.Id == id);
            if (classToDelete != null)
            {
                ClassInformation.Remove(classToDelete);
            }
            return RedirectToPage();
        }

        // OnPost method to handle edit operation
        public IActionResult OnPostEdit(int id)
        {
            var classToEdit = ClassInformation.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                ClassName = classToEdit.ClassName;
                StudentCount = classToEdit.StudentCount;
                Description = classToEdit.Description;
                ClassInformation.Remove(classToEdit);
            }
            return Page();
        }
    }
}
