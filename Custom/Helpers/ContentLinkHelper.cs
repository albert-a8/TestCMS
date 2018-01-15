using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Telerik.Everlive.Sdk.Core.Model.Interfaces;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Data.ContentLinks;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Events.Model;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Model.ContentLinks;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.News.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Utilities.TypeConverters;

namespace SitefinityWebApp.Custom.Helpers
{
    public static class ContentLinkHelper
    {
        public static string NewsImage(string liveId, string newsNamespace)
        {
            string imageUrl = string.Empty;
           Guid liveGuid = Guid.Empty;
           if (!string.IsNullOrWhiteSpace(liveId) && Guid.TryParse(liveId, out liveGuid))
           {
               NewsItem master = null;
               NewsItem live = App.WorkWith().NewsItems().Where(item => item.Id.ToString() == liveId).Get().SingleOrDefault();

               if (live != null)
               {
                   master = App.WorkWith().NewsItem().GetManager().GetMaster(live);
                   if (master != null)
                   {
                       imageUrl = GetChildImageMediaUrl(master.Id, newsNamespace, "Image", false);
                   }
               }
           }
            return imageUrl;
        }

        public static string NewsDocument(string liveId, string linkCSS)
        {
            string docString = string.Empty;
            Guid liveGuid = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(liveId) && Guid.TryParse(liveId, out liveGuid))
            {
                NewsItem master = null;
                string docMediaUrl = string.Empty;
                string docType = string.Empty;
                string docSize = string.Empty;
                NewsItem live = App.WorkWith().NewsItems().Where(item => item.Id == liveGuid).Get().SingleOrDefault();

                if (live != null)
                {
                    master = App.WorkWith().NewsItem().GetManager().GetMaster(live);
                    var contentLinks = ContentLinksManager.GetManager().GetContentLinks();
                    var relatedDocumentLink = contentLinks.FirstOrDefault(cl => cl.ParentItemId == master.Id && !cl.IsChildDeleted && !cl.IsParentDeleted && cl.AvailableForLive && cl.ComponentPropertyName == "Document");

                    if (relatedDocumentLink != null)
                    {
                        var librariesManager = LibrariesManager.GetManager();
                        var document = librariesManager.GetDocument(relatedDocumentLink.ChildItemId);
                        if (document != null)
                        {
                            docMediaUrl = document.MediaUrl;
                            docType = document.MimeType.Substring(document.MimeType.IndexOf('/') + 1).ToUpper();
                            docSize = (document.TotalSize / 1024).ToString();
                            docString = "<a href=\"" + docMediaUrl + "\" title=\"Read More\" class=\"" + linkCSS + "\" target=\"_blank\">View News Article (" + docType + ", (" + docSize + " KB))</a> ";
                        }
                    }
                }
            }
            return docString;
        }

        /// <summary>
        /// Get the event images.
        /// </summary>
        public static string EventsImage(string liveId, string imageFieldName, string eventsNamespace)
        {
            string imageUrl = string.Empty;
            Guid liveGuid = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(liveId) && Guid.TryParse(liveId, out liveGuid) && !string.IsNullOrWhiteSpace(imageFieldName))
            {
                Event master = null;
                var live = App.WorkWith().Events().Where(item => item.Id == liveGuid).Get().SingleOrDefault();

                if (live != null)
                {
                    master = App.WorkWith().Events().GetManager().GetMaster(live);
                    if (master != null)
                    {
                        imageUrl = GetChildImageMediaUrl(master.Id, eventsNamespace, imageFieldName, false);
                    }
                }
            }
            return imageUrl;
        }

        public static IEnumerable<Image> GetEventImages(Event liveEvent, string imageFieldName, string eventsNamespace)
        {
            List<Image> eventImages = new List<Image>();
            Event masterEvent = App.WorkWith().Events().GetManager().GetMaster(liveEvent);
            Type contentType = TypeResolutionService.ResolveType(eventsNamespace);
            IEnumerable<ContentLink> contentLinks = ContentLinksManager.GetManager().GetContentLinks(masterEvent.Id, contentType, imageFieldName);
            LibrariesManager librariesManager = LibrariesManager.GetManager();
            foreach (ContentLink link in contentLinks)
            {
                eventImages.Add(librariesManager.GetImage(link.ChildItemId));
            }
            return eventImages;
        }

        /// <summary>
        /// Get the banner images.
        /// </summary>
        public static string MainBannerImage(string liveId, string mainBannerNamespace)
        {
            string imageUrl = string.Empty;
            Guid liveGuid = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(liveId) && Guid.TryParse(liveId, out liveGuid))
            {
                var dynamicModuleManager = DynamicModuleManager.GetManager();

                var liveContent = DynamicContentHelper.GetDynamicContentById(mainBannerNamespace, liveGuid);
                    if (liveContent != null)
                    {
                        var master = dynamicModuleManager.Lifecycle.GetMaster(liveContent);
                        if (master != null)
                        {
                            imageUrl = GetChildImageMediaUrl(master.Id, mainBannerNamespace, "Image", false);                            
                        }
                    }
                
            }
            return imageUrl;
        }


        /// <summary>
        /// Get the image medial url of child related item via parent content from content link manager.
        /// 
        /// </summary>
        public static string GetChildImageMediaUrl(Guid parentMasterContentId, string parentContentTypeNameSpace, string imageFieldName, bool isThumbnail)
        {
            string imageUrl = string.Empty;
            Type contentType = TypeResolutionService.ResolveType(parentContentTypeNameSpace);
            var contentLinks = ContentLinksManager.GetManager().GetContentLinks(parentMasterContentId, contentType, imageFieldName);

            if (contentLinks != null && contentLinks.Count() > 0)
            {
                var relatedImageLink = contentLinks.FirstOrDefault(cl => !cl.IsChildDeleted && !cl.IsParentDeleted && cl.AvailableForLive);
                if (relatedImageLink != null)
                {
                    var librariesManager = LibrariesManager.GetManager();
                    var image = librariesManager.GetImage(relatedImageLink.ChildItemId);
                    if (image != null)
                    {
                        if (!isThumbnail)
                        {
                            imageUrl = image.MediaUrl;
                        }
                        else
                        {
                            imageUrl = image.ThumbnailUrl;
                        }
                    }
                }
            }
            return imageUrl;
        }
    }
}