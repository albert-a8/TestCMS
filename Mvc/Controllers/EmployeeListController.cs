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
    [ControllerToolboxItem(Name = "EmployeeList", Title = "EmployeeList", SectionName = "Custom")]
    [Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesigner(typeof(WidgetDesigners.EmployeeList.EmployeeListDesigner))]
    public class EmployeeListController : Controller
    {
        const string employeeNameSpace = "Telerik.Sitefinity.DynamicTypes.Model.Employees.Employee";

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
            List<KeyValuePair<string, string>> employeeClass = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("All Classifications", "all")
            };
            IEnumerable<Taxon> taxonList = ClassificationHelper.GetTaxaFromSimpleListClassification("employee-types");
            employeeClass.AddRange(taxonList.Select(x => new KeyValuePair<string, string>(x.Title, x.Id.ToString())));

            // populate employee list items
            IEnumerable<DynamicContent> employeeContents = DynamicContentHelper.GetDataByNameSpace(employeeNameSpace, contentStatus: ContentLifecycleStatus.Live);
            IEnumerable<EmployeeDetail> employeeList = employeeContents.Select(x => new EmployeeDetail(x)).OrderBy(y => y.EmployeeID);
            
            EmployeeListModel model = new EmployeeListModel(employeeClass, employeeList);
            return View(model);
        }

        [HttpPost, RelativeRoute("api/GetList")]
        [StandaloneResponseFilter]
        public ActionResult GetList(string selectedEmployeeTypeId)
        {
            string filterQuery = string.Empty;
            Guid employeeTypeId = Guid.Empty;
            Guid.TryParse(selectedEmployeeTypeId, out employeeTypeId);

            if (employeeTypeId != Guid.Empty)
                filterQuery += string.Format(" && {0}.Contains(Guid.Parse(\"{1}\"))", EmployeeDetail.Fields.EmployeeTypes.GetDescription(), employeeTypeId);

            // populate employee list items
            IEnumerable<DynamicContent> employeeContents = DynamicContentHelper.GetDynamicContentByFiltering(employeeNameSpace, filterQuery, contentStatus: ContentLifecycleStatus.Live);
            IEnumerable<EmployeeDetail> allContents = employeeContents.Select(x => new EmployeeDetail(x)).OrderBy(y => y.EmployeeID);

            return View("_employeeGrid", allContents.ToList());
        }
    }
}