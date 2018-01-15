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
using System.IO;
using System.Web;
using System.ServiceModel.Channels;
using System.Web.Routing;
using SitefinityWebApp.Custom.Models;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "EmployeeControls", Title = "EmployeeControls", SectionName = "Custom")]
    public class EmployeeControlsController : Controller
    {
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

            EmployeeControlsModel model = new EmployeeControlsModel(employeeClass);
            return View(model);
        }
    }
}