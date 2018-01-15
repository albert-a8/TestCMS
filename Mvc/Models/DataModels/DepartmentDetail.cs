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
    public class DepartmentDetail
    {
        public enum Fields
        {
            [Description("DepartmentName")]
            DepartmentName,

            [Description("DepartmentHead")]
            DepartmentHead
        }

        public DepartmentDetail() { }

        public DepartmentDetail(DynamicContent content)
        {
            DepartmentName = content.GetStringFieldValue(DepartmentDetail.Fields.DepartmentName.GetDescription());
            DepartmentHead = content.GetStringFieldValue(DepartmentDetail.Fields.DepartmentHead.GetDescription());
            DepartmentID = content.Id;
        }

        public string DepartmentName { get; set; }
        public string DepartmentHead { get; set; }
        public Guid DepartmentID { get; set; }
    }
}