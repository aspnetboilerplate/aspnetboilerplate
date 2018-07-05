using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    public class ModelValidationController : DemoControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(UserModel model, string test)
        {
            return View("Index", model);
        }
    }

    public class UserModel
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }
    }
}