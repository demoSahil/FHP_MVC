using FHP_BL;
using Resources;
using FHP_ValueObject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Resources;
using System.Data.Common;
using System.Xml.Linq;
using FHP_Web_UI.Helper;

namespace FHP_Web_UI.Controllers
{
    public class HomeController : Controller
    {
        List<cls_Employee_VO> employeesList;

        cls_DataProcessing_BL obj_Employee_BL;
        Resource resource = new Resource();

        [HttpPost]
        public ActionResult Delete(string[] ids)
        {
            bool isUserValid = true; ;
            obj_Employee_BL = HttpContext.Application["BLObject_Employee"] as cls_DataProcessing_BL;
            List<cls_Employee_VO> x = Session["employeeList"] as List<cls_Employee_VO>;

            foreach (string serialNo in ids)
            {
                long serial = long.Parse(serialNo);
                cls_Employee_VO empToBeDelete = obj_Employee_BL.GetEmployees().Find(s => s.SerialNo == serial);
                if (!obj_Employee_BL.DeleteEmployee(empToBeDelete, resource))
                {
                    isUserValid = false;
                }
            }

            if (isUserValid)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }


        public ActionResult Index()
        {

            obj_Employee_BL = HttpContext.Application["BLObject_Employee"] as cls_DataProcessing_BL;

            // Getting all the employees list
            employeesList = obj_Employee_BL.GetEmployees();

            var model = new
            {
                Resources = resource // Adding resources as a dynamic property
            };

            ViewBag.Model = resource;

            cls_XMLHelper xMLHelper = new cls_XMLHelper();
            Session["xmlFilePath"] = Server.MapPath("~/App_Data/Employees.xml");
            Session["xmlHelperObject"] = xMLHelper;

            xMLHelper.StoreEmployeesDataIntoXML(employeesList, Session["xmlFilePath"] as string);

            /* List<cls_Employee_VO> x= Session["employeeList"] as List<cls_Employee_VO>;*/
            return View(employeesList);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult New()
        {

            cls_Employee_VO newEmployee = new cls_Employee_VO();
            return View(newEmployee);
        }

        [HttpPost]
        public ActionResult New(cls_Employee_VO employee)
        {


            obj_Employee_BL = HttpContext.Application["BLObject_Employee"] as cls_DataProcessing_BL;
            obj_Employee_BL.SaveIntoDB(employee, resource);
            return RedirectToAction("Index");
        }

        public ActionResult SortAsc(string columnName)
        {

            /* List<cls_Employee_VO> sortedData= Session["employeeList"] as List<cls_Employee_VO>;*/
            cls_XMLHelper xmlHelper = Session["xmlHelperObject"] as cls_XMLHelper;
            List<cls_Employee_VO> sortedData = xmlHelper.ParseEmployeesFromXML(Session["xmlFilePath"] as string);
            sortedData = sortedData.OrderBy(e => e.GetType().GetProperty(columnName).GetValue(e, null)).ToList();
            return Json(sortedData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SortDesc(string columnName)
        {
            cls_XMLHelper xmlHelper = Session["xmlHelperObject"] as cls_XMLHelper;
            List<cls_Employee_VO> sortedData = xmlHelper.ParseEmployeesFromXML(Session["xmlFilePath"] as string);
            sortedData = sortedData.OrderByDescending(e => e.GetType().GetProperty(columnName).GetValue(e, null)).ToList();
            return Json(sortedData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ApplyFilter(string[] searchBoxesValues)
        {
             cls_XMLHelper xmlHelper = Session["xmlHelperObject"] as cls_XMLHelper;
            List<cls_Employee_VO> sortedData = xmlHelper.ParseEmployeesFromXML(Session["xmlFilePath"] as string);
            List<cls_Employee_VO> filteredList = new List<cls_Employee_VO>();

            for (int i = 0; i < sortedData.Count; i++)
            {
                bool dataMatched = true;

                for (int j = 0; j < searchBoxesValues.Length; j++)
                {
                    if (!String.IsNullOrEmpty(searchBoxesValues[j]))
                    {
                        string searchString = searchBoxesValues[j].ToLower();

                        string targetString = "";

                        switch (j)
                        {
                            case 0:
                                targetString = sortedData[i].SerialNo.ToString();
                                break;
                            case 1:
                                targetString = sortedData[i].Prefix;
                                break;
                            case 2:
                                targetString = sortedData[i].FirstName;
                                break;
                            case 3:
                                targetString = sortedData[i].MiddleName;
                                break;
                            case 4:
                                targetString = sortedData[i].LastName;
                                break;
                            case 5:
                                targetString = sortedData[i].CurrentAddress;
                                break;
                            case 6:
                                targetString = sortedData[i].DOB.ToShortDateString();
                                break;
                            /*case 7:
                                targetString = sortedData[i].Education;*/
                            //break;
                            case 8:
                                targetString = sortedData[i].CurrentCompany;
                                break;
                            case 9:
                                targetString = sortedData[i].JoiningDate.ToShortDateString().ToString();
                                break;
                        }


                        if (targetString.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) == -1)
                        {
                            dataMatched = false;
                            break;
                        }
                    }
                }

                if (dataMatched)
                {
                    filteredList.Add(sortedData[i]);
                }
            }

            // Return the filtered data as JSON
            return Json(filteredList, JsonRequestBehavior.AllowGet);
        }

        


    }
}