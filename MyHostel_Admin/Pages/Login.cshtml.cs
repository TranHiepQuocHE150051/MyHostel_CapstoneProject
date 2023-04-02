using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyHostel_Admin.Models;
using System;

namespace MyHostel_Admin.Pages
{
    public class LoginModel : PageModel
    {
        private readonly MyHostelContext context;

        public LoginModel(MyHostelContext _context)
        {
            context = _context;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("currentUser") != null)
            {
                String currentUser = HttpContext.Session.GetString("currentUser");
                Admin user = context.Admins.SingleOrDefault(p => p.AccountName.ToLower().Equals(currentUser.ToLower()));
                if (user == null)
                {
                    return Page();
                }
                else
                {
                    if (HttpContext.Session.GetString("Role") != null)
                    {
                        string role = HttpContext.Session.GetString("Role");
                        if (role.Equals("3"))
                        {
                            return RedirectToPage("/Admin_Page/Index");
                        }
                        else
                        {
                            return RedirectToPage("/Index");
                        }
                    }
                    else
                    {
                        return Page();
                    }
                }
            }
            return Page();
        }

        public string Message { set; get; }
        [BindProperty]
        public Admin userLoginInfo { set; get; }

        public IActionResult OnPost()
        {
            Admin userLogin = context.Admins.SingleOrDefault(u => u.AccountName.ToLower().Equals(userLoginInfo.AccountName.ToLower()));

            if (userLogin != null)
            {
                if (userLogin.Password.Equals(userLoginInfo.Password))
                {
                    HttpContext.Session.SetString("currentUser", userLogin.AccountName);
                    HttpContext.Session.SetString("Role", userLogin.RoleId.ToString());
                    if (userLogin.RoleId == 3)
                    {
                        return RedirectToPage("/Admin_Page/Index");
                    }
                    else
                    {
                        return RedirectToPage("/Index");
                    }
                }
                else
                {
                    Message = "Wrong password!";
                    return Page();
                }
            }
            else
            {
                Message = "User does not exist!";
                return Page();
            }
        }
    }
}

