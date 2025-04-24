using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Week5Project.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Week5Project.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public LoginModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var users = await LoadUsersAsync();
            var user = users.FirstOrDefault(u => 
                u.Username == Username && 
                u.Password == Password && 
                u.IsActive);

            if (user == null)
            {
                ErrorMessage = "Invalid username or password";
                return Page();
            }

            // Generate a simple token
            var token = Guid.NewGuid().ToString();
            var sessionId = HttpContext.Session.Id;

            // Store in session
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", sessionId);
            HttpContext.Session.SetString("role", user.Role);

            // Store in cookies
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("username", user.Username, cookieOptions);
            Response.Cookies.Append("token", token, cookieOptions);
            Response.Cookies.Append("session_id", sessionId, cookieOptions);

            return RedirectToPage("/Index");
        }

        private async Task<List<User>> LoadUsersAsync()
        {
            var path = Path.Combine(_env.WebRootPath, "data", "users.json");
            using var stream = new FileStream(path, FileMode.Open);
            return await JsonSerializer.DeserializeAsync<List<User>>(stream) ?? new List<User>();
        }
    }
}