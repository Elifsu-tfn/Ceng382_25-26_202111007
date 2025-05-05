using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Week5Project.Models;
using Week5Project.Models.Data;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Week5Project.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string ClassName { get; set; } = string.Empty;

        [BindProperty]
        public int StudentCount { get; set; }

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        public ClassInformationTable ClassTable { get; set; } = new ClassInformationTable();

        public async Task<IActionResult> OnGetAsync(int currentPage = 1, int pageSize = 10, string filterClassName = null)
        {
            if (!IsAuthenticated())
                return RedirectToPage("/Login");

            await LoadTableDataAsync(currentPage, pageSize, filterClassName);
            return Page();
        }

        private bool IsAuthenticated()
        {
            var sessionUser = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");

            var cookieUser = Request.Cookies["username"];
            var cookieToken = Request.Cookies["token"];
            var cookieId = Request.Cookies["session_id"];

            return !string.IsNullOrEmpty(sessionUser) &&
                   !string.IsNullOrEmpty(sessionToken) &&
                   !string.IsNullOrEmpty(sessionId) &&
                   sessionUser == cookieUser &&
                   sessionToken == cookieToken &&
                   sessionId == cookieId;
        }

        private async Task LoadTableDataAsync(int currentPage, int pageSize, string filterClassName)
        {
            var query = _context.Classes.AsQueryable();

            if (!string.IsNullOrEmpty(filterClassName))
            {
                query = query.Where(c => c.ClassName.Contains(filterClassName));
            }

            var totalCount = await query.CountAsync();
            var classes = await query
                .OrderBy(c => c.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ClassTable = new ClassInformationTable
            {
                Classes = classes,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = totalCount,
                FilterClassName = filterClassName
            };
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!IsAuthenticated())
                return RedirectToPage("/Login");

            if (!ModelState.IsValid)
            {
                await LoadTableDataAsync(1, ClassTable.PageSize, ClassTable.FilterClassName);
                return Page();
            }

            var newClass = new Class
            {
                ClassName = ClassName,
                StudentCount = StudentCount,
                Description = Description,
                IsActive = true 
            };

            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            return RedirectToPage(new
            {
                currentPage = ClassTable.CurrentPage,
                pageSize = ClassTable.PageSize,
                filterClassName = ClassTable.FilterClassName
            });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!IsAuthenticated())
                return RedirectToPage("/Login");

            var classToDelete = await _context.Classes.FindAsync(id);
            if (classToDelete != null)
            {
                _context.Classes.Remove(classToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new
            {
                currentPage = ClassTable.CurrentPage,
                pageSize = ClassTable.PageSize,
                filterClassName = ClassTable.FilterClassName
            });
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            if (!IsAuthenticated())
                return RedirectToPage("/Login");

            var classToEdit = await _context.Classes.FindAsync(id);
            if (classToEdit != null)
            {
                ClassName = classToEdit.ClassName;
                StudentCount = classToEdit.StudentCount;
                Description = classToEdit.Description;

                // (silme yerine update)
                classToEdit.ClassName = ClassName;
                classToEdit.StudentCount = StudentCount;
                classToEdit.Description = Description;
                
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new
            {
                currentPage = ClassTable.CurrentPage,
                pageSize = ClassTable.PageSize,
                filterClassName = ClassTable.FilterClassName
            });
        }

        public async Task<IActionResult> OnGetExportVisibleDataAsync(string filterClassName, int currentPage, int pageSize)
        {
            if (!IsAuthenticated())
                return RedirectToPage("/Login");

            var query = _context.Classes.AsQueryable();

            if (!string.IsNullOrEmpty(filterClassName))
            {
                query = query.Where(c => c.ClassName.Contains(filterClassName));
            }

            var data = await query
                .OrderBy(c => c.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Id,
                    c.ClassName,
                    c.StudentCount,
                    Description = c.Description ?? string.Empty, // Null kontrolü
                    ExportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    c.IsActive 
                })
                .ToListAsync();

            return new JsonResult(data);
        }
    }

    public class ClassInformationTable
    {
        public List<Class> Classes { get; set; } = new List<Class>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string FilterClassName { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}