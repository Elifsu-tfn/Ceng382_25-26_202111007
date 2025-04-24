using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Week5Project.Pages
{
    public class TableModel : PageModel
    {
        public List<ClassInfo> Classes { get; set; } = new List<ClassInfo>();

        [BindProperty]
        public string ClassName { get; set; }

        [BindProperty]
        public int StudentCount { get; set; }

        [BindProperty]
        public string Description { get; set; }

        public IActionResult OnGet()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            // Sample data - in a real app you'd load from a database
            Classes = new List<ClassInfo>
            {
                new ClassInfo { ClassName = "Class 1", StudentCount = 11, Description = "Description for class 1" },
                new ClassInfo { ClassName = "Class 2", StudentCount = 12, Description = "Description for class 2" },
                new ClassInfo { ClassName = "Class 3", StudentCount = 13, Description = "Description for class 3" },
                new ClassInfo { ClassName = "Class 4", StudentCount = 14, Description = "Description for class 4" },
                new ClassInfo { ClassName = "Class 5", StudentCount = 15, Description = "Description for class 5" },
                new ClassInfo { ClassName = "Class 6", StudentCount = 16, Description = "Description for class 6" }
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            // Add new class to the list
            Classes.Add(new ClassInfo
            {
                ClassName = ClassName,
                StudentCount = StudentCount,
                Description = Description
            });

            return RedirectToPage("/Table");
        }

        private bool IsUserAuthenticated()
        {
            var usernameFromCookie = Request.Cookies["username"];
            var tokenFromCookie = Request.Cookies["token"];
            var sessionIdFromCookie = Request.Cookies["session_id"];

            var usernameFromSession = HttpContext.Session.GetString("username");
            var tokenFromSession = HttpContext.Session.GetString("token");
            var sessionIdFromSession = HttpContext.Session.GetString("session_id");

            return !string.IsNullOrEmpty(usernameFromCookie) &&
                   !string.IsNullOrEmpty(tokenFromCookie) &&
                   !string.IsNullOrEmpty(sessionIdFromCookie) &&
                   usernameFromCookie == usernameFromSession &&
                   tokenFromCookie == tokenFromSession &&
                   sessionIdFromCookie == sessionIdFromSession;
        }
    }

    public class ClassInfo
    {
        public string ClassName { get; set; }
        public int StudentCount { get; set; }
        public string Description { get; set; }
    }
}