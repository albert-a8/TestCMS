using SitefinityWebApp.Mvc.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Lists.Model;
using Telerik.Sitefinity.Taxonomies.Model;

namespace SitefinityWebApp.Mvc.Models
{
    public class EmployeeListViewModel
    {
        public EmployeeListViewModel()
        {
            EmployeeClassification = new List<KeyValuePair<string, string>>();
            AllEmployees = null;
        }

        public EmployeeListViewModel(List<KeyValuePair<string, string>> empClass, IEnumerable<EmployeeDetail> allEmps)
        {
            EmployeeClassification = empClass;
            AllEmployees = allEmps;
        }

        public List<KeyValuePair<string, string>> EmployeeClassification { get; set; }

        public IEnumerable<EmployeeDetail> AllEmployees { get; set; }
    }
}