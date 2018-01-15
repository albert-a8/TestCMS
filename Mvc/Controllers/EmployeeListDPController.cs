using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using SitefinityWebApp.Mvc.Models;
using Telerik.Sitefinity.Modules.Lists;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.Utilities.TypeConverters;
using SitefinityWebApp.Custom.Helpers;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.DynamicModules.Model;
using System.Collections.Generic;
using SitefinityWebApp.Mvc.Models.DataModels;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Mvc.ActionFilters;
using SitefinityWebApp.Custom.Extensions;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "EmployeeListDP", Title = "EmployeeListDP", SectionName = "Custom")]
    [Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesigner(typeof(WidgetDesigners.EmployeeListDP.EmployeeListDPDesigner))]
    public class EmployeeListDPController : Controller
    {
        const string employeeNameSpace = "Telerik.Sitefinity.DynamicTypes.Model.Employees.Employee";
        const string departmentNameSpace = "Telerik.Sitefinity.DynamicTypes.Model.EmployeeDepartments.EmployeeDepartment";
        
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [Category("String Properties")]
        public string Message { get; set; }

        /// <summary>
        /// This is the default Action.
        /// </summary>
        [RelativeRoute("")]
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

            // populate employee list items
            IEnumerable<DynamicContent> employeeContents = DynamicContentHelper.GetDataByNameSpace(employeeNameSpace, contentStatus: ContentLifecycleStatus.Live);
            IEnumerable<EmployeeDetail> employeeList = employeeContents.Select(x => new EmployeeDetail(x)).OrderBy(y => y.LastName).ThenBy(y => y.FirstName);

            EmployeeListDPModel model = new EmployeeListDPModel(employeeClass, departmentClass);
            return View(model);
        }

        [HttpPost, RelativeRoute("api/GetMultiList")]
        [StandaloneResponseFilter]
        public ActionResult GetMultiList(List<Guid> selectedEmployeeTypeIds, List<Guid> selectedDepartmentIds, DateTime selectedStartDate, DateTime selectedEndDate)
        {
            string filterQuery = "";
            if (selectedEmployeeTypeIds != null && selectedEmployeeTypeIds.Any())
            {
                filterQuery += " && (";
                Guid lastId = selectedEmployeeTypeIds.Last();
                foreach (var id in selectedEmployeeTypeIds)
                {
                    filterQuery += string.Format("{0}.Contains(Guid.Parse(\"{1}\"))", EmployeeDetail.Fields.EmployeeTypes.GetDescription(), id);
                    if (id != lastId)
                        filterQuery += " || ";
                }
                filterQuery += ")";
            }

            if (selectedStartDate != null)
            {
                filterQuery += string.Format(" && {0} >= Convert.ToDateTime(\"{1}\")", EmployeeDetail.Fields.BirthDate.GetDescription(), selectedStartDate);
            }

            if (selectedEndDate != null)
            {
                filterQuery += string.Format(" && {0} <= Convert.ToDateTime(\"{1}\")", EmployeeDetail.Fields.BirthDate.GetDescription(), selectedEndDate);
            }

            // populate employee list items
            IEnumerable<DynamicContent> employeeContents = DynamicContentHelper.GetDynamicContentByFiltering(employeeNameSpace, filterQuery, contentStatus: ContentLifecycleStatus.Live);
            IEnumerable<EmployeeDetail> allContents = employeeContents.Select(x => new EmployeeDetail(x)).OrderBy(y => y.LastName).ThenBy(y => y.FirstName);
            
            return View("_employeeListDP", allContents);
        }
    }
}