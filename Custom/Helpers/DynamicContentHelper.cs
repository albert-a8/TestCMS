using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Utilities.TypeConverters;
using System.ComponentModel;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.RelatedData;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.OpenAccess;
using Telerik.Sitefinity.Taxonomies;

using Telerik.Sitefinity.Data.Linq.Dynamic;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Data.ContentLinks;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Model.ContentLinks;

namespace SitefinityWebApp.Custom.Helpers
{
    /// <summary>
    /// This class for dynamic content modules.
    /// </summary>
    public static class DynamicContentHelper
    {
        public static IEnumerable<DynamicContent> GetDataByNameSpace(string contentNamespace, DynamicModuleManager dynamicModuleManager = null, ContentLifecycleStatus? contentStatus = null)
        {
            if (dynamicModuleManager == null)
                dynamicModuleManager = DynamicModuleManager.GetManager();
            Type contentType = TypeResolutionService.ResolveType(contentNamespace);

            // This is how we get the collection of Product items
            IQueryable<DynamicContent> myCollection = dynamicModuleManager
                .GetDataItems(contentType);

            if (contentStatus != null)
            {
                myCollection = myCollection.Where(x => x.Status == contentStatus);
                if (contentStatus == ContentLifecycleStatus.Live)
                {
                    myCollection = myCollection.Where(x => x.Visible);
                }
            }

            return myCollection;
        }

        public static IEnumerable<DynamicContent> GetDynamicContentByFiltering(string contentNamespace, string filter,
            DynamicModuleManager dynamicModuleManager = null, ContentLifecycleStatus? contentStatus = null)
        {
            if (dynamicModuleManager == null)
                dynamicModuleManager = DynamicModuleManager.GetManager();
            Type contentType = TypeResolutionService.ResolveType(contentNamespace);

            if (contentStatus != null)
            {
                switch (contentStatus)
                {
                    case ContentLifecycleStatus.Master:
                        filter = "Status = \"Master\" " + filter;
                        break;
                    case ContentLifecycleStatus.Live:
                        filter = "Status = \"Live\" && Visible " + filter;
                        break;
                }
            }
            return dynamicModuleManager.GetDataItems(contentType).Where(filter);
        }

        public static DynamicContent GetDynamicContentById(string contentNamespace, Guid contentId, DynamicModuleManager dynamicModuleManager = null)
        {
            if (dynamicModuleManager == null)
                dynamicModuleManager = DynamicModuleManager.GetManager();

            Type contentType = TypeResolutionService.ResolveType(contentNamespace);

            // This is how we get the course items through filtering
            var myFilteredCollection = dynamicModuleManager.GetDataItem(contentType, contentId);

            return myFilteredCollection;
        }

        public static IEnumerable<DynamicContent> GetChildItemsByDynamicContentLive(DynamicContent parentContent, string contentNamespace, string filter)
        {
            // A list containing all child items from all child types
            List<DynamicContent> allChildItems = new List<DynamicContent>();

            if (parentContent != null)
            {
                // Resolve Class type
                Type classType = TypeResolutionService.ResolveType(contentNamespace);
                // Get query of child items with live status if the courseItem is live and with master status otherwise.
                IQueryable<DynamicContent> classAllItems = parentContent.GetChildItems(classType).Where("Status = \"Live\" && Visible " + filter);
                // Add them to the result
                allChildItems.AddRange(classAllItems);

                // Get query of child items with live status if the courseItem is live and with master status otherwise.
                // Can be used in widget templates with "Eval" expression (example: <asp:Repeater runat="server" DataSource='<%# Eval("Class") %>'>).
                // IQueryable<DynamicContent> classItems = courseItem.GetValue("Class") as IQueryable<DynamicContent>;
            }

            return allChildItems;
        }

        public static IEnumerable<DynamicContent> GetRelatedDynamicContent(DynamicContent content, string fieldName, string relatedContentNamespace)
        {
            IEnumerable<DynamicContent> result = new List<DynamicContent>();
            IQueryable<ContentLink> contentLinks = GetRelatedContentLinks(content, fieldName);
            Type relatedContentType = TypeResolutionService.ResolveType(relatedContentNamespace);
            result = DynamicModuleManager.GetManager().GetDataItems(relatedContentType)
                .Where(x => x.Status == ContentLifecycleStatus.Master
                    && contentLinks.Any(y => y.ChildItemId == x.Id));

            return result;
        }

        private static IQueryable<ContentLink> GetRelatedContentLinks(DynamicContent content, string fieldName)
        {
            DynamicContent masterContent = DynamicModuleManager.GetManager().Lifecycle.GetMaster(content) as DynamicContent;
            if (masterContent == null)
                throw new Exception("no master content found!");
            return ContentLinksManager.GetManager().GetContentLinks()
                .Where(cl => cl.ParentItemId == masterContent.Id && !cl.IsChildDeleted && !cl.IsParentDeleted
                    && cl.AvailableForLive && cl.ComponentPropertyName == fieldName);
        }

        public static IEnumerable<DynamicContent> GetMainContentsByRelatedData(DynamicContent relatedContent, string fieldName, string mainContentNamespace)
        {
            IEnumerable<DynamicContent> result = new List<DynamicContent>();
            DynamicModuleManager dynamicModuleManager = DynamicModuleManager.GetManager();
            DynamicContent masterContent = dynamicModuleManager.Lifecycle.GetMaster(relatedContent) as DynamicContent;
            IQueryable<ContentLink> contentLinks = ContentLinksManager.GetManager().GetContentLinks()
            .Where(cl => cl.ChildItemId == masterContent.Id && !cl.IsChildDeleted && !cl.IsParentDeleted
                    && cl.AvailableForLive && cl.ComponentPropertyName == fieldName);

            if (!contentLinks.Any())
                return result;

            Type mainContentType = TypeResolutionService.ResolveType(mainContentNamespace);
            result = dynamicModuleManager.GetDataItems(mainContentType)
                .Where(x => x.Status == ContentLifecycleStatus.Master
                    && contentLinks.Any(y => y.ParentItemId == x.Id));

            return result;
        }

    }
}