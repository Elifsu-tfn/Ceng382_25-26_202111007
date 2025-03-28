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

        // Properties for form data binding
        [BindProperty]
        public string ClassName { get; set; }

        [BindProperty]
        public int StudentCount { get; set; }

        [BindProperty]
        public string Description { get; set; }

        // OnGet method to render the page
        public void OnGet()
        {
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
