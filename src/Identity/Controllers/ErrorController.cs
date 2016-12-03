using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Models;
using Microsoft.AspNetCore.Mvc;


//return RedirectToRoute("Error", new {id = 120});

namespace Identity.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error()
        {
            return View(new ErrorViewModel());
        }

        public IActionResult Errors(int id, string message)
        {
            return View("Error", new ErrorViewModel { ErrorCode = id, ErrorMessage = message});
        }

        public IActionResult ErrorParser(int id = 0)
        {
            if (id == 0)
                return Error();
            return Errors(id, GetErrorDescription(id));
        }

        private string GetErrorDescription(int id)
        {
            switch (id)
            {
                case 403:
                    return "You don't have access to this file/repo.";
                case 404:
                    return "There is no such file/repo.";
                case 700:
                    return "This repository/branch doesn't contain requested file.";
                case 701:
                    return "This repository is empty! You have to push to it before viewing.";
                default:
                    return "";
            }
        }
    }
}
