using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class SomeProtectedPageModel : PageModel
{
    public void OnGet()
    {
        // Kullanıcı bilgilerini session'dan al
        var username = HttpContext.Session.GetString("Username");
        var token = HttpContext.Session.GetString("Token");
        var sessionId = HttpContext.Session.GetString("SessionId");

        // Eğer session'da bir şey eksikse, login sayfasına yönlendir
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(sessionId))
        {
            Response.Redirect("/Login");  // Giriş yapılmamışsa login sayfasına yönlendir.
            return;
        }

        // Çerezlerden gelen kullanıcı bilgilerini al
        var cookieUsername = Request.Cookies["Username"];
        var cookieToken = Request.Cookies["Token"];

        // Eğer session ve çerezdeki bilgiler uyuşmuyorsa, login sayfasına yönlendir
        if (username != cookieUsername || token != cookieToken)
        {
            Response.Redirect("/Login");  // Giriş hatalıysa login sayfasına yönlendir.
            return;
        }

        // Eğer her şey yolundaysa, sayfaya devam et
    }
}
