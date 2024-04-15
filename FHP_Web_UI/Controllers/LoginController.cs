
using FHP_BL;
using FHP_ValueObject;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace FHP_Web_UI.Controllers
{
    public class LoginController : Controller
    {
        private readonly cls_ValidateUser_BL _validateUserBL;
        public LoginController()
        {
            _validateUserBL = UnityConfig.Container.Resolve<cls_ValidateUser_BL>("validateUserBL");



        }
        public ActionResult Index()
        {
            Resource resource = new Resource();
            Session["ResourceObject"] = resource;
            cls_User_VO user_VO = new cls_User_VO();

            return View(user_VO);
        }

        [HttpPost]
        public ActionResult Index(cls_User_VO user)
        {
            if (ModelState.IsValid)
            {
                if (_validateUserBL.isUserPresent(user))
                {
                    Session["UserPermissions"] = _validateUserBL.GetUserPermission(user);
                    return RedirectToAction("Index", "Home");
                }

                return View();
            }

            return View();

        }
    }
}