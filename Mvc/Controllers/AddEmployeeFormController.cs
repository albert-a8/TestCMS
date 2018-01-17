using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using SitefinityWebApp.Mvc.Models;
using System.Collections.Generic;
using Telerik.Sitefinity.Taxonomies.Model;
using SitefinityWebApp.Custom.Helpers;
using Telerik.Sitefinity.DynamicModules.Model;
using SitefinityWebApp.Mvc.Models.DataModels;
using Telerik.Sitefinity.GenericContent.Model;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "AddEmployeeForm", Title = "AddEmployeeForm", SectionName = "Custom")]
    public class AddEmployeeFormController : Controller
    {
        const string employeeNameSpace = "Telerik.Sitefinity.DynamicTypes.Model.Employees.Employee";
        const string departmentNameSpace = "Telerik.Sitefinity.DynamicTypes.Model.EmployeeDepartments.EmployeeDepartment";

        public ActionResult Index()
        {
            // populate employee classifications
            List<KeyValuePair<string, string>> employeeClass = new List<KeyValuePair<string, string>>();
            IEnumerable<Taxon> taxonEClassList = ClassificationHelper.GetTaxaFromSimpleListClassification("employee-types");
            employeeClass.AddRange(taxonEClassList.Select(x => new KeyValuePair<string, string>(x.Title, x.Id.ToString())));

            // populate departments
            List<KeyValuePair<string, string>> departmentClass = new List<KeyValuePair<string, string>>();
            IEnumerable<DynamicContent> departmentContents = DynamicContentHelper.GetDataByNameSpace(departmentNameSpace, contentStatus: ContentLifecycleStatus.Live);
            IEnumerable<DepartmentDetail> departmentList = departmentContents.Select(x => new DepartmentDetail(x)).OrderBy(y => y.DepartmentName);
            departmentClass.AddRange(departmentList.Select(x => new KeyValuePair<string, string>(x.DepartmentName, x.DepartmentID.ToString())));

            AddEmployeeFormModel model = new AddEmployeeFormModel();
            model.EmployeeClassification = employeeClass;
            model.DepartmentClassification = departmentClass;

            // check if page is new or edit
            string passedID = GetPassedID();
            if(passedID != "") // edit!
            {
                model.PassEmployeeURL(passedID);
            }

            return View(model);
        }

        private string GetPassedID()
        {
            string passedID = "";
            string _pageQuery = Request.Url.ToString();
            if (_pageQuery.Contains("edit-employee") && _pageQuery.Contains("?"))
            {
                string[] temp1 = _pageQuery.Split(new string[] { "?" }, StringSplitOptions.RemoveEmptyEntries);
                string requestUrl1 = temp1 == null || temp1.Length != 2 ? "" : temp1[1];
                string[] temp2 = requestUrl1.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                passedID = temp2 == null || temp2.Length != 2 ? "" : temp2[1];
            }

            return passedID;
        }
    }
}