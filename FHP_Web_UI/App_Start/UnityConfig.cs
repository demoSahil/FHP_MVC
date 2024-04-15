using System.Configuration;
using System.Reflection;
using System;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using FHP_BL;
using Unity.Lifetime;

namespace FHP_Web_UI
{
    public static class UnityConfig
    {
        private static readonly UnityContainer _container = new UnityContainer();

        public static UnityContainer Container
        {
            get { return _container; }
        }

        public static void Initialize()
        {
            try
            {
                string storageType = ConfigurationManager.AppSettings["StorageType"];
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

                // Load BL and DL assemblies
                Assembly assemblyBL = Assembly.Load("FHP_BL");
                Assembly assemblyDL = Assembly.Load("FHP_DL");

                // Create BL objects
                object dataProcessing_BL = CreateInstance(assemblyBL, "FHP_BL.cls_DataProcessing_BL");
                object validateUser_BL = CreateInstance(assemblyBL, "FHP_BL.cls_ValidateUser_BL");

                // Inject DL objects
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

                // Register instance with Unity container with ContainerControlledLifetimeManager
                _container.RegisterInstance<cls_ValidateUser_BL>("validateUserBL", (cls_ValidateUser_BL)validateUser_BL, new ContainerControlledLifetimeManager());
                _container.RegisterInstance<cls_DataProcessing_BL>("dataProcessingBL", (cls_DataProcessing_BL)dataProcessing_BL, new ContainerControlledLifetimeManager());

            }
            catch (Exception ex)
            {
                // Handle or log exceptions appropriately
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