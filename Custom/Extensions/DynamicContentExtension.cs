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
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Data.ContentLinks;
using Telerik.Sitefinity.Model.ContentLinks;

namespace SitefinityWebApp.Custom.Extensions
{
    public static class DynamicContentExtension
    {
        public static string GetImageUrl(this DynamicContent content, string fieldName, bool isThumbnail = false)
        {
            string imageUrl = string.Empty;
            ContentLink relatedContent = GetRelatedContentLink(content, fieldName);

            if (relatedContent == null)
                return "no content link found!";
            Image image = LibrariesManager.GetManager().GetImage(relatedContent.ChildItemId);

            if (image == null)
                return "no image found!";
            if (isThumbnail)
                imageUrl = image.ThumbnailUrl;
            else
                imageUrl = image.MediaUrl;

            return imageUrl;
        }

        public static List<string> GetImageUrls(this DynamicContent content, string fieldName, bool isThumbnail = false)
        {
            DynamicContent masterCOntent = DynamicModuleManager.GetManager().Lifecycle.GetMaster(content) as DynamicContent;
            IEnumerable<ContentLink> relatedContents = GetRelatedContentLinks(masterCOntent, fieldName);
            List<string> imageUrls = new List<string>();

            foreach (ContentLink relatedContent in relatedContents)
            {
                Image image = LibrariesManager.GetManager().GetImage(relatedContent.ChildItemId);
                if (image == null)
                    continue;
                string imageUrl = (isThumbnail) ? image.ThumbnailUrl : image.MediaUrl;
                imageUrls.Add(imageUrl);
            }

            return imageUrls;
        }

        public static string GetVideoUrl(this DynamicContent content, string fieldName)
        {
            ContentLink relatedContent = GetRelatedContentLink(content, fieldName);

            if (relatedContent == null)
                return "no content link found!";
            Video video = LibrariesManager.GetManager().GetVideo(relatedContent.ChildItemId);

            if (video == null)
                return "no video found!";

            return video.MediaUrl;
        }

        public static string GetDocumentUrl(this DynamicContent content, string fieldName)
        {
            ContentLink relatedContent = GetRelatedContentLink(content, fieldName);

            if (relatedContent == null)
                return "no content link found!";
            Document document = LibrariesManager.GetManager().GetDocument(relatedContent.ChildItemId);

            if (document == null)
                return "no document found!";

            return document.MediaUrl;
        }

        public static IEnumerable<Taxon> GetContentTags(this DynamicContent content, string fieldName)
        {
            List<Guid> tagIds = content.GetValue<TrackedList<Guid>>(fieldName).ToList();
            List<Taxon> tags = new List<Taxon>();
            TaxonomyManager manager = TaxonomyManager.GetManager();
            foreach (Guid id in tagIds.Where(x => x!=Guid.Empty))
            {
                    tags.Add(manager.GetTaxon<Taxon>(id));             
            }
            return tags;
        }

		public static IEnumerable<DynamicContent> GetRelatedDynamicContent(this DynamicContent content, string fieldName, string relatedContentNamespace)
        {
            IEnumerable<DynamicContent> result = new List<DynamicContent>();
            IEnumerable<ContentLink> contentLinks = GetRelatedContentLinks(content, fieldName);
            Type relatedContentType = TypeResolutionService.ResolveType(relatedContentNamespace);
            result = DynamicModuleManager.GetManager().GetDataItems(relatedContentType)
                .Where(x => x.Status == ContentLifecycleStatus.Master
                    && contentLinks.Any(y => y.ChildItemId == x.Id));

            return result;
        }

        private static ContentLink GetRelatedContentLink(DynamicContent content, string fieldName)
        {
            DynamicContent masterContent = DynamicModuleManager.GetManager().Lifecycle.GetMaster(content) as DynamicContent;
            if (masterContent == null)
                throw new Exception("no master content found!");
            return GetRelatedContentLinks(masterContent, fieldName).FirstOrDefault();
        }

        private static IEnumerable<ContentLink> GetRelatedContentLinks(DynamicContent masterContent, string fieldName)
        {
            return ContentLinksManager.GetManager().GetContentLinks()
                .Where(cl => cl.ParentItemId == masterContent.Id && !cl.IsChildDeleted && !cl.IsParentDeleted
                    && cl.AvailableForLive && cl.ComponentPropertyName == fieldName);
        }
    }
}