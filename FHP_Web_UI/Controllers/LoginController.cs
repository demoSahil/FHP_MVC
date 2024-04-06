
using FHP_BL;
using FHP_ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FHP_Web_UI.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            cls_User_VO user_VO= new cls_User_VO();

            return View(user_VO);
        }

        [HttpPost]
        public ActionResult Index(cls_User_VO user)
        {
            cls_ValidateUser_BL obj_User_BL = HttpContext.Application["BLObject_User"] as cls_ValidateUser_BL;

            
            if (ModelState.IsValid)
            {
                if (obj_User_BL.isUserPresent(user))
                {
                return RedirectToAction("Index", "Home");
                }
                
                return View();
            }
            
            return View();

        }
    }
}