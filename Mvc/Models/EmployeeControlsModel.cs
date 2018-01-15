using SitefinityWebApp.Mvc.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Lists.Model;
using Telerik.Sitefinity.Taxonomies.Model;

namespace SitefinityWebApp.Mvc.Models
{
    public class EmployeeControlsModel
    {
        public EmployeeControlsModel(List<KeyValuePair<string, string>> empClass)
        {
            EmployeeClassification = empClass;
        }

        public List<KeyValuePair<string, string>> EmployeeClassification { get; set; }
    }
}