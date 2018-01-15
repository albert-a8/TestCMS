using SitefinityWebApp.Custom.Helpers;
using SitefinityWebApp.Custom.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;
using System.Globalization;
using SitefinityWebApp.Custom.Extensions;
using SitefinityWebApp.Mvc.Models.DataModels;
using System.Web.Mvc;
using SitefinityWebApp.Mvc.Controllers;
using Telerik.Sitefinity.GenericContent.Model;
using SitefinityWebApp.Mvc.Models;
using System.Web.Script.Serialization;

namespace SitefinityWebApp.Custom.Services
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class EmployeesList
    {
        #region Public Methods

        [OperationContract]
        public string GetEmployees([FromBody]List<Guid> selectedEmployeeTypeIds, 
            [FromBody]string selectedStartDate, [FromBody]string selectedEndDate)
        {
            string filterQuery = "";
            string employeeNameSpace = "Telerik.Sitefinity.DynamicTypes.Model.Employees.Employee";
            DateTime startDate = Convert.ToDateTime(selectedStartDate);
            DateTime endDate = Convert.ToDateTime(selectedEndDate);

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

            if (startDate != null && startDate >= Convert.ToDateTime("1/1/1800"))
            {
                filterQuery += string.Format(" && {0} >= Convert.ToDateTime(\"{1}\")", EmployeeDetail.Fields.BirthDate.GetDescription(), startDate);
            }

            if (endDate != null && endDate >= Convert.ToDateTime("1/1/1800"))
            {
                filterQuery += string.Format(" && {0} <= Convert.ToDateTime(\"{1}\")", EmployeeDetail.Fields.BirthDate.GetDescription(), endDate);
            }

            // populate employee list items
            IEnumerable<DynamicContent> employeeContents = DynamicContentHelper.GetDynamicContentByFiltering(employeeNameSpace, filterQuery, contentStatus: ContentLifecycleStatus.Live);
            IEnumerable<EmployeeDetail> allContents = employeeContents.Select(x => new EmployeeDetail(x)).OrderBy(y => y.LastName).ThenBy(y => y.FirstName);

            //return ViewHelper.RenderPartialView(new EmployeeControlsController(), "_employeeListView", allContents);
            //return new EmployeeControlsController().ReturnListView(allContents);
            return JsonConvert.SerializeObject(allContents);
        }

        #endregion
    }
}
