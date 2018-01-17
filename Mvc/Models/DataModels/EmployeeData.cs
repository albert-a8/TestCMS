using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.DynamicModules.Model;
using SitefinityWebApp.Custom.Extensions;
using SitefinityWebApp.Custom.Helpers;
using Telerik.Sitefinity.Taxonomies.Model;

namespace SitefinityWebApp.Mvc.Models.DataModels
{
    public class EmployeeData
    {
        const string departmentNameSpace = "Telerik.Sitefinity.DynamicTypes.Model.EmployeeDepartments.EmployeeDepartment";

        public enum Fields
        {
            [Description("EmployeeID")]
            EmployeeID,

            [Description("FirstName")]
            FirstName,

            [Description("LastName")]
            LastName,

            [Description("JobDescription")]
            JobDescription,

            [Description("BirthDate")]
            BirthDate,

            [Description("Salary")]
            Salary,

            [Description("employeetypes")]
            EmployeeTypes,

            [Description("ProfilePicture")]
            ProfilePicture,

            [Description("EmployeeDepartment")]
            EmployeeDepartment
        }

        public EmployeeData() { }

        public EmployeeData(DynamicContent content)
        {
            EmployeeID = content.GetStringFieldValue(EmployeeDetail.Fields.EmployeeID.GetDescription());
            FirstName = content.GetStringFieldValue(EmployeeDetail.Fields.FirstName.GetDescription());
            LastName = content.GetStringFieldValue(EmployeeDetail.Fields.LastName.GetDescription());
            JobDescription = content.GetStringFieldValue(EmployeeDetail.Fields.JobDescription.GetDescription());
            DateTime BirthDate = content.GetDateTimeFieldValue(EmployeeDetail.Fields.BirthDate.GetDescription());
            BirthdateDisplay = (BirthDate == null || BirthDate == DateTime.Parse("01/01/0001")) ? "" : BirthDate.ToString("dd/MM/yyyy");
            Salary = content.GetDecimalFieldValue(EmployeeDetail.Fields.Salary.GetDescription());
            EmployeeURL = content.GetStringFieldValue("UrlName");

            EmployeeTypesGUID = new List<Guid>();
            List<Taxon> AllEmployeeTypes = content.GetContentTags(EmployeeDetail.Fields.EmployeeTypes.GetDescription()).ToList();
            EmployeeType = string.Join(", ", AllEmployeeTypes.Select(x => x.Title.ToString()));
            foreach(Taxon item in AllEmployeeTypes)
            {
                EmployeeTypesGUID.Add(item.Id);
            }

            EmployeeImages = content.GetImageUrls(EmployeeDetail.Fields.ProfilePicture.GetDescription());

            EmployeeDepartmentsGUID = new List<Guid>();
            IEnumerable<DynamicContent> allDepartments = DynamicContentHelper.GetRelatedDynamicContent(content, EmployeeDetail.Fields.EmployeeDepartment.GetDescription(), departmentNameSpace);
            if (allDepartments.Any() && allDepartments != null)
            {
                IEnumerable<DepartmentDetail> employeeDept = allDepartments.Select(x => new DepartmentDetail(x)).OrderBy(y => y.DepartmentName);
                EmployeeDepartments = employeeDept.ToList();
                foreach(DepartmentDetail item in employeeDept)
                {
                    EmployeeDepartmentsGUID.Add(item.DepartmentID);
                }
            }

            EmployeeGUID = content.Id;
        }

        public Guid EmployeeGUID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobDescription { get; set; }
        public string BirthdateDisplay { get; set; }
        public decimal Salary { get; set; }
        public string EmployeeType { get; set; }
        public List<Guid> EmployeeTypesGUID { get; set; }
        public string EmployeeURL { get; set; }
        public List<string> EmployeeImages { get; set; }
        public List<DepartmentDetail> EmployeeDepartments { get; set; }
        public List<Guid> EmployeeDepartmentsGUID { get; set; }
    }
}