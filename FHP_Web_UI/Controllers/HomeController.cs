﻿using FHP_BL;
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
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using Unity;

namespace FHP_Web_UI.Controllers
{
    [Authorize]
    public  class HomeController : Controller
    {
        private readonly cls_DataProcessing_BL _dataProcessBL;
        public HomeController()
        {
            _dataProcessBL = UnityConfig.Container.Resolve<cls_DataProcessing_BL>("dataProcessingBL");
        }

        /// <summary>
        /// Action for Deleting Records
        /// </summary>
        /// <param name="ids"> an array of serial No to delete</param>
        /// <returns> An success true/false describes that records are deleed or not </returns>
        [HttpPost]
        [Authorize(Roles ="ADMIN,SELF,SUPERADMIN")]

        public ActionResult Delete(string[] ids)
        {
            bool isUserValid = true;
            bool userCanDelete = false;
            Dictionary<string, bool> userPermissions = Session["UserPermissions"] as Dictionary<String, bool>;
            userCanDelete = userPermissions["CanDelete"];

            if (userCanDelete)
            {
                foreach (string serialNo in ids)
                {
                    long serial = long.Parse(serialNo);
                    cls_Employee_VO empToBeDelete = _dataProcessBL.GetEmployees().Find(s => s.SerialNo == serial);

                    if (!_dataProcessBL.DeleteEmployee(empToBeDelete, Session["ResourceObject"] as Resource))
                    {
                        isUserValid = false;
                    }
                }
            }

            if (isUserValid && userCanDelete) return Json(new { success = true }); // In case of Success

            else
            {
                if (!userCanDelete) return Json(new { success = false, errorMessage = "User do not have permission To Delete" });

                else return Json(new { success = false, errorMessage = "Something Went Wrong! Try Again" });
            }
        }

        /// <summary>
        /// Action that retrieve all the records from storage
        /// </summary>
        /// <returns> A view that represents employee information in from of table</returns>
        public ActionResult Index()
        {

            // Getting all the employees list
            List<cls_Employee_VO> employeesList = _dataProcessBL.GetEmployees();

            var model = new
            {
                Resources = Session["ResourceObject"] as Resource
            };

            ViewBag.Model = Session["ResourceObject"] as Resource;

            //-Doubt
            cls_XMLHelper xMLHelper = new cls_XMLHelper();
            Session["xmlFilePath"] = Server.MapPath("~/App_Data/Employees.xml");
            Session["xmlHelperObject"] = xMLHelper;

            xMLHelper.StoreEmployeesDataIntoXML(employeesList, Session["xmlFilePath"] as string);
            return View(employeesList);
        }

        /// <summary>
        /// Action method which creates a New user
        /// </summary>
        /// <returns> A view which is a form which collects the data from user input and bind it into a model</returns>
        [Authorize(Roles = "ADMIN,SELF,SUPERADMIN")]
        public ActionResult New()
        {
            cls_Employee_VO newEmployee = new cls_Employee_VO();
            ViewData["Action"] = "New";
            return View("_EmployeeDetailsForm", newEmployee);
        }

        /// <summary>
        /// An action which catches the model sent by form as a POST request
        /// </summary>
        /// <param name="employee"></param>
        /// <returns> Redirects to the Action Index </returns>
        [HttpPost]
        [Authorize(Roles = "ADMIN,SELF,SUPERADMIN")]
        public ActionResult New(cls_Employee_VO employee)
        {
            if (_dataProcessBL.SaveIntoDB(employee, Session["ResourceObject"] as Resource))
            {
                TempData["message"] = "Employee Added Successfully";
                return RedirectToAction("Index");
            }

            TempData["message"] = "Employee Not Added Successfully";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Action Method that Sort the data in ascending
        /// </summary>
        /// <param name="columnName"> Represents the column based on sorting is to be done </param>
        /// <returns> The sorted data which is to be sent as repsonse to the caller to display the sorted data</returns>
        
        public ActionResult SortAsc(string columnName)
        {
            cls_XMLHelper xmlHelper = Session["xmlHelperObject"] as cls_XMLHelper;
            List<cls_Employee_VO> sortedData = xmlHelper.ParseEmployeesFromXML(Session["xmlFilePath"] as string);
            sortedData = sortedData.OrderBy(e => e.GetType().GetProperty(columnName).GetValue(e, null)).ToList();
            return Json(sortedData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Action Method that Sort the data in descending
        /// </summary>
        /// <param name="columnName"> Represents the column based on sorting is to be done </param>
        /// <returns> The sorted data which is to be sent as repsonse to the caller to display the sorted data</returns>
        public ActionResult SortDesc(string columnName)
        {
            cls_XMLHelper xmlHelper = Session["xmlHelperObject"] as cls_XMLHelper;
            List<cls_Employee_VO> sortedData = xmlHelper.ParseEmployeesFromXML(Session["xmlFilePath"] as string);
            sortedData = sortedData.OrderByDescending(e => e.GetType().GetProperty(columnName).GetValue(e, null)).ToList();
            return Json(sortedData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Action Method that filters the data
        /// </summary>
        /// <param name="searchBoxesValues"> An string array consist of values inside filer box of every column</param>
        /// <returns>  The filtered data which is to be sent as repsonse to the caller to display the sorted data </returns>
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

        /// <summary>
        /// Retrieves employee details for updating based on the provided serial number.
        /// </summary>
        /// <param name="id">The serial number of the employee to update.</param>
        /// <returns>The view for updating employee details.</returns>
        [Authorize(Roles = "ADMIN,SELF,SUPERADMIN")]
        public ActionResult Update(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("Error");
            }

            cls_XMLHelper xmlHelper = Session["xmlHelperObject"] as cls_XMLHelper;
            string xmlFilePath = Session["xmlFilePath"] as string;

            if (xmlHelper == null || string.IsNullOrEmpty(xmlFilePath))
            {
                return View("Error");
            }

            List<cls_Employee_VO> employeesList = xmlHelper.ParseEmployeesFromXML(xmlFilePath);
            cls_Employee_VO empToUpdate = employeesList.FirstOrDefault(x => x.SerialNo == long.Parse(id));

            if (empToUpdate == null)
            {
                return View("Error");
            }
            ViewData["Action"] = "Update";

            return View("_EmployeeDetailsForm", empToUpdate);
        }

        /// <summary>
        /// Updates employee details in the database.
        /// </summary>
        /// <param name="updatedEmp">The updated employee details.</param>
        /// <returns>Redirects to the index page if the update is successful; otherwise, returns the view for updating.</returns>
        [HttpPost]
        [Authorize(Roles = "ADMIN,SELF,SUPERADMIN")]
        public ActionResult Update(cls_Employee_VO updatedEmp)
        {
            updatedEmp.editMode = 1;
            if (_dataProcessBL.SaveIntoDB(updatedEmp, Session["ResourceObject"] as Resource))
            {
                TempData["message"] = "Employee Updated Successfully";
                return RedirectToAction("Index");
            }

            TempData["message"] = "Employee Not Updated ";
            return View("Error");

        }

        /// <summary>
        /// Retrieves employee details for updating based on the provided serial number.
        /// </summary>
        /// <param name="id">The serial number of the employee to update.</param>
        /// <returns>The view for updating employee details.</returns>
        public ActionResult Pagination(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("Error");
            }

            cls_XMLHelper xmlHelper = Session["xmlHelperObject"] as cls_XMLHelper;
            string xmlFilePath = Session["xmlFilePath"] as string;

            if (xmlHelper == null || string.IsNullOrEmpty(xmlFilePath))
            {
                return View("Error");
            }

            List<cls_Employee_VO> employeesList = xmlHelper.ParseEmployeesFromXML(xmlFilePath);
            cls_Employee_VO empToUpdate = employeesList.FirstOrDefault(x => x.SerialNo == long.Parse(id));

            if (empToUpdate == null)
            {
                return View("Error");
            }

            ViewData["Action"] = "Update";
            return View("_EmployeeDetailsForm", empToUpdate);
        }

        /// <summary>
        /// Retrieves the list of all employees' data.
        /// </summary>
        /// <returns>JSON representation of the list of employees.</returns>
        public ActionResult EmployeesData()
        {
            cls_XMLHelper xmlHelper = Session["xmlHelperObject"] as cls_XMLHelper;
            List<cls_Employee_VO> employeesList = xmlHelper.ParseEmployeesFromXML(Session["xmlFilePath"] as string);
            return Json(employeesList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}