﻿using FHP_DL;
using FHP_ValueObject;
using Resources;
using System;
using System.Collections.Generic;

namespace FHP_BL
{
    /// <summary>
    /// Represents the business logic layer for data processing.
    /// </summary>
    public class cls_DataProcessing_BL
    {
        /// <summary>
        /// Object for file handling.
        /// </summary>
        public IDataHandlerEmployee dataHandlerEmp;

        /// <summary>
        /// Property Injection 
        /// </summary>
        private IDataHandlerEmployee EmployeeDataObject
        {   
            set
            {
                this.dataHandlerEmp = value;
            }
        }

        /// <summary>
        /// Saves employee data into a file.
        /// </summary>
        /// <param name="employee">The employee data to be saved.</param>
        /// <param name="resource">Resource object for additional functionality.</param>
        /// <returns>True if the data is saved successfully, otherwise false.</returns>
        public bool SaveIntoDB(cls_Employee_VO employee, Resource resource)
        {
            
            if (isValid(employee, resource))
            {
                try
                {
                    if (employee.editMode == (byte)Resource.EditMode.add)
                    {
                        dataHandlerEmp.AddEmployeeInfoIntoFile(employee);

                    } // Adding a new Record

                    else if (employee.editMode == (byte)Resource.EditMode.edit)
                    {
                        dataHandlerEmp.UpdateEntry(employee);
                    } // Updating a present Record

                    return true;
                }

                catch (cls_DataLayerException ex)
                {
                    throw new cls_BusinessLayerException("Error in Business layer", ex);
                }

            } // if employee has valid details

            return false;
        }

        /// <summary>
        /// Checks if the employee data is valid.
        /// </summary>
        /// <param name="employee">The employee data to be validated.</param>
        /// <param name="resource">Resource object for additional functionality.</param>
        /// <returns>True if the employee data is valid, otherwise false.</returns>
        private bool isValid(cls_Employee_VO employee, Resource resource)
        {


            bool isValid = true;

            //--------------- Validating Empty fields--------------\\

            //------First Name
            if (string.IsNullOrEmpty(employee.FirstName) || string.IsNullOrWhiteSpace(employee.FirstName))
            {
                isValid = false;
                employee.ValidationMessage = (byte)Resource.ValidationMessage.FirstNameEmpty;
            }

            //------Current Company 
            else if (string.IsNullOrEmpty(employee.CurrentCompany) || string.IsNullOrWhiteSpace(employee.CurrentCompany))
            {
                isValid = false;
                employee.ValidationMessage = (byte)Resource.ValidationMessage.CurrentCompanyEmpty;
            }

            //------Education
            else if (employee.Education == 255)
            {
                isValid = false;
                employee.ValidationMessage = (byte)Resource.ValidationMessage.QualificationEmpty;
            }

            //-----------Validating fields length-------------\\

            //------First Name
            else if (employee.FirstName.Length > 50)
            {
                isValid = false;
                employee.ValidationMessage = (byte)Resource.ValidationMessage.FirstNameTooLong;
            }

            //------ Last Name
            else if (employee.LastName.Length > 50)
            {
                isValid = false;
                employee.ValidationMessage = (byte)Resource.ValidationMessage.LastNameTooLong;
            }

            //------ Middle Name
            else if (employee.MiddleName.Length > 25)
            {
                isValid = false;
                employee.ValidationMessage = (byte)Resource.ValidationMessage.MiddleNameTooLong;
            }

            //------ Current Address
            else if (employee.CurrentAddress.Length > 500)
            {
                isValid = false;
                employee.ValidationMessage = (byte)Resource.ValidationMessage.CurrentAddressTooLong;

            }

            //------ Current Company
            else if (employee.CurrentCompany.Length > 255)
            {
                isValid = false;
                employee.ValidationMessage = (byte)Resource.ValidationMessage.CurrentCompanyTooLong;

            }

            //----------------Validating User Age-----------------\\

            DateTime dob = employee.DOB;
            int dobYear = dob.Year;

            DateTime joiningDate = employee.JoiningDate;
            int joiningYear = joiningDate.Year;

          /*  if (joiningYear < dobYear || joiningYear - dobYear <= 18 || joiningYear - dobYear >= 90)
            {
                isValid = false;
                employee.ValidationMessage = (byte)Resource.ValidationMessage.AgeLimit;
            }*/
            return isValid;
        }

        /// <summary>
        /// Retrieves a list of all employees.
        /// </summary>
        /// <returns>A list of Employee objects.</returns>
        public List<cls_Employee_VO> GetEmployees()
        {
            try
            {
                return dataHandlerEmp.GetAllEmployee();
            }
            catch (cls_DataLayerException ex)
            {
                throw new cls_BusinessLayerException("Error in while Getting all records", ex);
            }
        }

        /// <summary>
        /// Deletes an employee data from the file.
        /// </summary>
        /// <param name="empDataToBeDelete">The employee data to be deleted.</param>
        /// <param name="resource">Resource object for additional functionality.</param>
        /// <returns>True if the employee data is deleted successfully, otherwise false.</returns>
        public bool DeleteEmployee(cls_Employee_VO empDataToBeDelete, Resource resource)
        {
            if (empDataToBeDelete.editMode != 3)
            {
                empDataToBeDelete.isDeleted = true;
                try
                {
                    dataHandlerEmp.DeleteEmployeeFromFile(empDataToBeDelete);
                    return true;

                }
                catch (cls_DataLayerException ex)
                {
                    throw new cls_BusinessLayerException("Error while deleting employee", ex);
                }

            } // Means the user is not readOnly user 

            empDataToBeDelete.ValidationMessage = (byte)Resource.ValidationMessage.ReadOnlyUserCannotDelete;
            return false;        // returning false means that user is readOnly user cannot delete data
        }
    }


}
