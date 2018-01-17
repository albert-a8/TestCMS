using System;
using System.Collections.Generic;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.DynamicModules.Model;
using SitefinityWebApp.Custom.Extensions;
using SitefinityWebApp.Custom.Helpers;
using Telerik.Sitefinity.Taxonomies.Model;
using System.ComponentModel;
using Telerik.Sitefinity.GenericContent.Model;
using SitefinityWebApp.Mvc.Models.DataModels;
using System.Linq;

namespace SitefinityWebApp.Mvc.Models
{
    public class AddEmployeeFormModel
    {
        public Guid EmployeeGUID { get; set; }
        public string EditedEmployeeURL { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobDescription { get; set; }
        public string BirthDate { get; set; }
        public decimal Salary { get; set; }
        public List<string> EmployeePhotos { get; set; }
        public List<Guid> EmployeeTypes { get; set; }
        public List<string> EmployeeTypes_Selected { get; set; }
        public List<Guid> EmployeeDepartments { get; set; }
        public List<string> EmployeeDepartments_Selected { get; set; }

        public List<KeyValuePair<string, string>> EmployeeClassification { get; set; }
        public List<KeyValuePair<string, string>> DepartmentClassification { get; set; }

        public AddEmployeeFormModel()
        {
            EmployeeGUID = new Guid();
            EditedEmployeeURL = "";
            EmployeeID = "";
            FirstName = "";
            LastName = "";
            JobDescription = "";
            BirthDate = "";
            Salary = 0;
            EmployeePhotos = new List<string>();
            EmployeeTypes = new List<Guid>();
            EmployeeTypes_Selected = new List<string>();
            EmployeeDepartments = new List<Guid>();
            EmployeeDepartments_Selected = new List<string>();
            EmployeeClassification = new List<KeyValuePair<string, string>>();
            DepartmentClassification = new List<KeyValuePair<string, string>>();
        }

        public void PassEmployeeURL(string passedID)
        {
            string employeeNameSpace = "Telerik.Sitefinity.DynamicTypes.Model.Employees.Employee";

            EditedEmployeeURL = passedID;
            string filterQuery = string.Format(" && UrlName = \"{0}\"", passedID);

            IEnumerable<DynamicContent> employeeContents = DynamicContentHelper.GetDynamicContentByFiltering(employeeNameSpace, filterQuery, contentStatus: ContentLifecycleStatus.Live);
            IEnumerable<EmployeeData> allContents = employeeContents.Select(x => new EmployeeData(x)).OrderBy(y => y.LastName).ThenBy(y => y.FirstName);

            if(allContents.Any() && allContents.Count() == 1)
            {
                foreach (EmployeeData data in allContents)
                {
                    EmployeeGUID = data.EmployeeGUID;
                    EmployeeID = data.EmployeeID;
                    FirstName = data.FirstName;
                    LastName = data.LastName;
                    JobDescription = data.JobDescription;
                    Salary = data.Salary;
                    BirthDate = data.BirthdateDisplay;

                    if (data.EmployeeTypesGUID.Any() && data.EmployeeTypesGUID != null)
                    {
                        foreach (Guid item in data.EmployeeTypesGUID)
                        {
                            EmployeeTypes_Selected.Add(item.ToString());
                        }
                    }

                    if (data.EmployeeDepartmentsGUID.Any() && data.EmployeeDepartmentsGUID != null)
                    {
                        foreach (Guid item in data.EmployeeDepartmentsGUID)
                        {
                            EmployeeDepartments_Selected.Add(item.ToString());
                        }
                    }
                }
            }
        }
    }
}