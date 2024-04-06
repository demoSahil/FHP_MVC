using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;

namespace FHP_Web_UI.App_Start
{
    public class cls_Init
    {
        public void Intialize()
        {
            try
            {

                string storageType = ConfigurationManager.AppSettings["StorageType"];
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

                //------ Defining assembly Locations
                string assemblyBlName = "FHP_BL";
                string assemblyDLName = "FHP_DL";

                //-----------Early Binding--------------\\
                Assembly assemblyBL = Assembly.Load(assemblyBlName);
                Assembly assemblyDL = Assembly.Load(assemblyDLName);

                //--------Lazy binding -------------\\
                /* Assembly assemblyBL = Assembly.Load("FHP_BL");
                 Assembly assemblyDL = Assembly.Load("FHP_DL");*/


                object dataProcessing_BL = CreateInstance(assemblyBL, "FHP_BL.cls_DataProcessing_BL");
                object validateUser_BL = CreateInstance(assemblyBL, "FHP_BL.cls_ValidateUser_BL");

                //--------------Injecting  Objects using setter Injections

                if (storageType == "Database")
                {
                    SetPropertyValue(dataProcessing_BL, "EmployeeDataObject", CreateInstance(assemblyDL, "FHP_DL.cls_DataHandlerDB_DL", connectionString));
                    SetPropertyValue(validateUser_BL, "UserDataObject", CreateInstance(assemblyDL, "FHP_DL.cls_UsersDataDB_DL", connectionString));
                }
                else if (storageType == "FlatFile")
                {
                    SetPropertyValue(dataProcessing_BL, "EmployeeDataObject", CreateInstance(assemblyDL, "FHP_DL.cls_DataHandlerFF_DL"));
                    SetPropertyValue(validateUser_BL, "UserDataObject", CreateInstance(assemblyDL, "FHP_DL.cls_UserDataFF_DL"));
                }

                HttpContext.Current.Application["BLObject_Employee"] = dataProcessing_BL;
                HttpContext.Current.Application["BLObject_User"] = validateUser_BL;


            }
            catch (Exception ex)
            {

            }
        }

        private static object CreateInstance(Assembly assembly, string className, params object[] parameters)
        {
            Type type = assembly.GetType(className);
            return Activator.CreateInstance(type, parameters);
        }


        private static void SetPropertyValue(object obj, string propertyName, object value)
        {
            Type type = obj.GetType();

            PropertyInfo propertyInfo = type.GetProperty(propertyName);

            if (propertyName == "EmployeeDataObject")
            {
                propertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);

            }
            propertyInfo.SetValue(obj, value);
        }
    }
}