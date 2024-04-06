using FHP_ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace FHP_Web_UI.Helper
{
    public class cls_XMLHelper
    {
        public void StoreEmployeesDataIntoXML(List<cls_Employee_VO> employeesList,string filePath)
        {

            XDocument xmlDoc = new XDocument(
          new XElement("Employees",
              employeesList.Select(u => new XElement("Employees",
                  new XElement("SerialNumber", u.SerialNo),
                  new XElement("Prefix", u.Prefix),
                  new XElement("FirstName", u.FirstName),
                  new XElement("MiddleName", u.MiddleName),
                  new XElement("LastName", u.LastName),
                  new XElement("CurrentAddress", u.CurrentAddress),
                  new XElement("DOB", u.DOB),
                  new XElement("Education", u.Education),
                  new XElement("CurrentCompany", u.CurrentCompany),
                  new XElement("JoiningDate", u.JoiningDate)
              ))
          )
      );

            string xmlString = xmlDoc.ToString();
            System.IO.File.WriteAllText(filePath, xmlString);
        }

        public List<cls_Employee_VO> ParseEmployeesFromXML(string filePath)
        {
            List<cls_Employee_VO> employeesList = new List<cls_Employee_VO>();

            XDocument xmlDoc = XDocument.Load(filePath);
            foreach (var employeeElement in xmlDoc.Root.Elements("Employees"))
            {
                cls_Employee_VO employee = new cls_Employee_VO
                {
                    SerialNo = (int)employeeElement.Element("SerialNumber"),
                    Prefix = (string)employeeElement.Element("Prefix"),
                    FirstName = (string)employeeElement.Element("FirstName"),
                    MiddleName = (string)employeeElement.Element("MiddleName"),
                    LastName = (string)employeeElement.Element("LastName"),
                    CurrentAddress = (string)employeeElement.Element("CurrentAddress"),
                    DOB = (DateTime)employeeElement.Element("DOB"),
                    Education = Convert.ToByte(employeeElement.Element("Education").Value),
                    CurrentCompany = (string)employeeElement.Element("CurrentCompany"),
                    JoiningDate = (DateTime)employeeElement.Element("JoiningDate")
                };
                employeesList.Add(employee);
            }

            return employeesList;
        }

    }
}