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
using Telerik.Sitefinity.Versioning;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.RelatedData;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Lifecycle;

namespace SitefinityWebApp.Custom.Services
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class EmployeesList
    {
        const string employeeNameSpace = "Telerik.Sitefinity.DynamicTypes.Model.Employees.Employee";
        const string departmentNameSpace = "Telerik.Sitefinity.DynamicTypes.Model.EmployeeDepartments.EmployeeDepartment";

        #region Public Methods

        [OperationContract]
        public string GetEmployees([FromBody]List<Guid> selectedEmployeeTypeIds, 
            [FromBody]string selectedStartDate, [FromBody]string selectedEndDate)
        {
            string filterQuery = "";
            DateTime startDate = Convert.ToDateTime(selectedStartDate);
            DateTime endDate = Convert.ToDateTime(selectedEndDate);

            if (selectedEmployeeTypeIds != null && selectedEmployeeTypeIds.Any())
            {
                filterQuery += " && (";
                Guid lastId = selectedEmployeeTypeIds.Last();
                foreach (Guid id in selectedEmployeeTypeIds)
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
            IEnumerable<EmployeeData> allContents = employeeContents.Select(x => new EmployeeData(x)).OrderBy(y => y.LastName).ThenBy(y => y.FirstName);

            //return ViewHelper.RenderPartialView(new EmployeeControlsController(), "_employeeListView", allContents);
            //return new EmployeeControlsController().ReturnListView(allContents);
            return JsonConvert.SerializeObject(allContents);
        }

        [OperationContract]
        public BaseResponse AddEmployee([FromBody]string EmployeeID,
            [FromBody]string FirstName,
            [FromBody]string LastName,
            [FromBody]string JobDescription,
            [FromBody]string Salary,
            [FromBody]string BirthDate,
            [FromBody]List<Guid> EmployeeTypeIds,
            [FromBody]List<Guid> DepartmentIds)
        {
            BaseResponse response = new BaseResponse()
            {
                IsSuccessful = true,
                ResponseMessage = "Add Successful",
                ErrorMessage = ""
            };
            try
            {
                var providerName = "OpenAccessProvider";
                var transactionName = "addNewEmployee";
                var versionManager = VersionManager.GetManager(null, transactionName);

                DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);
                Type employeeType = TypeResolutionService.ResolveType(employeeNameSpace);
                DynamicContent employeeItem = dynamicModuleManager.CreateDataItem(employeeType);

                // This is how values for the properties are set
                employeeItem.SetValue("EmployeeID", EmployeeID);
                employeeItem.SetValue("FirstName", FirstName);
                employeeItem.SetValue("LastName", LastName);
                employeeItem.SetValue("JobDescription", JobDescription);

                DateTime _getBirthdate = Convert.ToDateTime(BirthDate);
                if(_getBirthdate > Convert.ToDateTime("1/1/1900"))
                {
                    employeeItem.SetValue("BirthDate", Convert.ToDateTime(BirthDate));
                }

                decimal _getSalary = 0;
                if (Salary != null && Salary != "" && Decimal.TryParse(Salary, out _getSalary))
                {
                    employeeItem.SetValue("Salary", _getSalary);
                }

                // add employee types
                if (EmployeeTypeIds != null && EmployeeTypeIds.Any())
                {
                    TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
                    var checkEmpType = taxonomyManager.GetTaxa<FlatTaxon>().Where(t => t.Taxonomy.Name == "employee-types").FirstOrDefault();
                    if (checkEmpType != null)
                    {
                        foreach (Guid item in EmployeeTypeIds)
                        {
                            employeeItem.Organizer.AddTaxa("employeetypes", item);
                        }
                    }
                }

                if (DepartmentIds != null && DepartmentIds.Any())
                {
                    DynamicModuleManager employeeDepartmentManager = DynamicModuleManager.GetManager();
                    Type employeeDepartmentType = TypeResolutionService.ResolveType(departmentNameSpace);

                    foreach (Guid id in DepartmentIds)
                    {
                        DynamicContent employeeDepartmentItem = dynamicModuleManager.GetDataItem(employeeDepartmentType, id);
                        if(employeeDepartmentItem != null)
                        {
                            employeeItem.CreateRelation(employeeDepartmentItem, "EmployeeDepartment");
                        }
                    }
                }


                if(FirstName != "" && LastName != "")
                {
                    employeeItem.SetString("UrlName", FirstName.Replace(" ", "").ToLower() + "-" + LastName.Replace(" ", "").ToLower());
                }
                else
                {
                    employeeItem.SetString("UrlName", "employee-" + EmployeeID);
                }

                employeeItem.SetValue("PublicationDate", DateTime.UtcNow);
                employeeItem.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Draft");
                TransactionManager.CommitTransaction(transactionName);
                //dynamicModuleManager.SaveChanges();

                //publish the transaction
                ILifecycleDataItem publishedClassItem = dynamicModuleManager.Lifecycle.Publish(employeeItem);

                //You need to set appropriate workflow status
                employeeItem.SetWorkflowStatus(dynamicModuleManager.Provider.ApplicationName, "Published");
                // You need to call SaveChanges() in order for the items to be actually persisted to data store
                versionManager.CreateVersion(employeeItem, true);
                TransactionManager.CommitTransaction(transactionName);

                return response;
            }
            catch (Exception ex)
            {
                response = new BaseResponse()
                {
                    IsSuccessful = false,
                    ResponseMessage = "Add Failed",
                    ErrorMessage = string.Format("Error Message: {0} || StackTrace: {1}", ex.Message, ex.StackTrace)
                };

                return response;
            }
        }

        [OperationContract]
        public BaseResponse EditEmployee([FromBody]string EmployeeGUID,
            [FromBody]string EmployeeID,
            [FromBody]string FirstName,
            [FromBody]string LastName,
            [FromBody]string JobDescription,
            [FromBody]string Salary,
            [FromBody]string BirthDate,
            [FromBody]List<Guid> EmployeeTypeIds)
        {
            BaseResponse response = new BaseResponse()
            {
                IsSuccessful = true,
                ResponseMessage = "Edit Successful",
                ErrorMessage = ""
            };
            try
            {
                var providerName = "OpenAccessProvider";
                var transactionName = "editEmployee_" + DateTime.Now.ToString("YYYYMMdd_HHmmss");
                var versionManager = VersionManager.GetManager(null, transactionName);

                DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager(providerName, transactionName);
                Type employeeType = TypeResolutionService.ResolveType(employeeNameSpace);

                DynamicContent employeeItemInit = dynamicModuleManager.GetDataItem(employeeType, Guid.Parse(EmployeeGUID));
                DynamicContent employeeItemMaster = DynamicModuleManager.GetManager().Lifecycle.GetMaster(employeeItemInit) as DynamicContent;
                DynamicContent employeeItem = dynamicModuleManager.Lifecycle.CheckOut(employeeItemMaster) as DynamicContent;

                // This is how values for the properties are set
                employeeItem.SetValue("EmployeeID", EmployeeID);
                employeeItem.SetValue("FirstName", FirstName);
                employeeItem.SetValue("LastName", LastName);
                employeeItem.SetValue("JobDescription", JobDescription);

                DateTime _getBirthdate = Convert.ToDateTime(BirthDate);
                if (_getBirthdate > Convert.ToDateTime("1/1/1900"))
                {
                    employeeItem.SetValue("BirthDate", Convert.ToDateTime(BirthDate));
                }

                decimal _getSalary = 0;
                if (Salary != null && Salary != "" && Decimal.TryParse(Salary, out _getSalary))
                {
                    employeeItem.SetValue("Salary", _getSalary);
                }

                // add employee types
                if (EmployeeTypeIds != null && EmployeeTypeIds.Any())
                {
                    TaxonomyManager taxonomyManager = TaxonomyManager.GetManager();
                    var checkEmpType = taxonomyManager.GetTaxa<FlatTaxon>().Where(t => t.Taxonomy.Name == "employee-types").FirstOrDefault();
                    if (checkEmpType != null)
                    {
                        employeeItem.Organizer.ClearAll();
                        foreach (Guid item in EmployeeTypeIds)
                        {
                            employeeItem.Organizer.AddTaxa("employeetypes", item);
                        }
                    }
                }

                if (FirstName != "" && LastName != "")
                {
                    employeeItem.SetString("UrlName", FirstName.Replace(" ", "").ToLower() + "-" + LastName.Replace(" ", "").ToLower());
                }
                else
                {
                    employeeItem.SetString("UrlName", "employee-" + EmployeeID);
                }

                employeeItem.SetValue("PublicationDate", DateTime.UtcNow);

                ILifecycleDataItem checkInStorageItem = dynamicModuleManager.Lifecycle.CheckIn(employeeItem);
                dynamicModuleManager.Lifecycle.Publish(checkInStorageItem);
                TransactionManager.CommitTransaction(transactionName);

                //dynamicModuleManager.Lifecycle.Publish(employeeItem);
                //dynamicModuleManager.SaveChanges();

                //TransactionManager.CommitTransaction(transactionName);

                return response;
            }
            catch (Exception ex)
            {
                response = new BaseResponse()
                {
                    IsSuccessful = false,
                    ResponseMessage = "Edit Failed",
                    ErrorMessage = string.Format("Error Message: {0} || StackTrace: {1}", ex.Message, ex.StackTrace)
                };

                return response;
            }
        }

        #endregion
    }
}
