using SitefinityWebApp.Mvc.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Lists.Model;
using Telerik.Sitefinity.Taxonomies.Model;

namespace SitefinityWebApp.Mvc.Models
{
    public class EmployeeListDPModel
    {
        public EmployeeListDPModel()
        {
            EmployeeClassification = new List<KeyValuePair<string, string>>();
            DepartmentClassification = new List<KeyValuePair<string, string>>();
            AllEmployees = null;
        }

        public EmployeeListDPModel(List<KeyValuePair<string, string>> empClass, List<KeyValuePair<string, string>> deptClass)
        {
            EmployeeClassification = empClass;
            DepartmentClassification = deptClass;
            AllEmployees = null;
        }

        public EmployeeListDPModel(IEnumerable<EmployeeDetail> allEmps)
        {
            EmployeeClassification = new List<KeyValuePair<string, string>>();
            DepartmentClassification = new List<KeyValuePair<string, string>>();
            AllEmployees = allEmps;
        }

        public EmployeeListDPModel(List<KeyValuePair<string, string>> empClass, List<KeyValuePair<string, string>> deptClass, IEnumerable<EmployeeDetail> allEmps)
        {
            EmployeeClassification = empClass;
            DepartmentClassification = deptClass;
            AllEmployees = allEmps;
        }

        public List<KeyValuePair<string, string>> EmployeeClassification { get; set; }

        public List<KeyValuePair<string, string>> DepartmentClassification { get; set; }

        public IEnumerable<EmployeeDetail> AllEmployees { get; set; }
    }
}