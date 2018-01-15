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
    public class EmployeeDetail
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

        public EmployeeDetail() { }

        public EmployeeDetail(DynamicContent content)
        {
            EmployeeID = content.GetStringFieldValue(EmployeeDetail.Fields.EmployeeID.GetDescription());
            FirstName = content.GetStringFieldValue(EmployeeDetail.Fields.FirstName.GetDescription());
            LastName = content.GetStringFieldValue(EmployeeDetail.Fields.LastName.GetDescription());
            JobDescription = content.GetStringFieldValue(EmployeeDetail.Fields.JobDescription.GetDescription());
            DateTime BirthDate = content.GetDateTimeFieldValue(EmployeeDetail.Fields.BirthDate.GetDescription());
            BirthdateDisplay = (BirthDate == null || BirthDate == DateTime.Parse("01/01/0001 00:00")) ? "" : BirthDate.ToString("dd/MM/yyyy HH:mm");
            Salary = content.GetDecimalFieldValue(EmployeeDetail.Fields.Salary.GetDescription());
            List<Taxon> AllEmployeeTypes = content.GetContentTags(EmployeeDetail.Fields.EmployeeTypes.GetDescription()).ToList();
            EmployeeTypeDisplay = string.Join(", ", AllEmployeeTypes.Select(x => x.Title.ToString()));
            List<string> WebImageUrls = content.GetImageUrls(EmployeeDetail.Fields.ProfilePicture.GetDescription());

            EmployeeImagesDisplay = "";
            if (WebImageUrls.Any() && WebImageUrls != null)
            {
                EmployeeImagesDisplay += "<div class=\"imagesList\"><img src=\"";
                EmployeeImagesDisplay += WebImageUrls.Aggregate((i, j) => i + "\" /><img src=\"" + j);
                EmployeeImagesDisplay += "\" /></div>";
            }

            DepartmentsDisplay = "";
            IEnumerable<DynamicContent> allDepartments = DynamicContentHelper.GetRelatedDynamicContent(content, EmployeeDetail.Fields.EmployeeDepartment.GetDescription(), departmentNameSpace);
            if(allDepartments.Any() && allDepartments != null)
            {
                allDepartments.All(x => { DepartmentText(new DepartmentDetail(x)); return true; });
            }
            else
            {
                DepartmentsDisplay += "<p><b>Department/s:</b> N/A</p>";
            }
        }

        public void DepartmentText(DepartmentDetail content)
        {
            DepartmentsDisplay += "<p><b>Department/s:</b></p>";
            DepartmentsDisplay += "<div class=\"department\">";
            DepartmentsDisplay += "<p><b>Department Name:</b>&nbsp;" + content.DepartmentName + "</p>";
            DepartmentsDisplay += "<p><b>Department Head:</b>&nbsp;" + content.DepartmentHead + "</p>";
            DepartmentsDisplay += "</div>";
        }

        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobDescription { get; set; }
        public string BirthdateDisplay { get; set; }
        public decimal Salary { get; set; }
        public string EmployeeTypeDisplay { get; set; }
        public string EmployeeImagesDisplay { get; set; }
        public string DepartmentsDisplay { get; set; }
    }
}