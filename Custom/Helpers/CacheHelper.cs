using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SitefinityWebApp.Custom.Models;
using Telerik.Sitefinity.Services;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Caching;
//using SitefinityWebApp.Custom.Configuration;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Data;

namespace SitefinityWebApp.Custom.Helpers
{
    public static class CacheHelper
    {
        public static void SetModelIntoCache<T>(string cacheKey, List<T> cacheList) where T : ICacheModel
        {
            ICacheManager cacheManager = SystemManager.GetCacheManager(CacheManagerInstance.Global);
            if (cacheManager.Contains(cacheKey))
                return;

            double expiration = 15;
            //XFGlobalSettingsConfig config = Config.Get<XFGlobalSettingsConfig>();
            //if (config.GeneralConfig["CacheExpirationKey"] != null)
            //    Double.TryParse(config.GeneralConfig["CacheExpirationKey"].Value, out expiration);

            cacheManager.Add(cacheKey, cacheList, CacheItemPriority.Normal, null,
                new AbsoluteTime(TimeSpan.FromMinutes(expiration)));
        }

        public static List<T> GetModelFromCache<T>(string cacheKey) where T : ICacheModel
        {
            ICacheManager cacheManager = SystemManager.GetCacheManager(CacheManagerInstance.Global);
            if (!cacheManager.Contains(cacheKey))
                return null;

            return cacheManager.GetData(cacheKey) as List<T>;
        }

        public static void SubscribeCacheDependency(Type dataType)
        {
            var cacheDependencyNotifiedObjects = CacheHelper.GetCacheDependencyObjects(dataType);

            if (!SystemManager.CurrentHttpContext.Items.Contains(PageCacheDependencyKeys.PageData))
            {
                SystemManager.CurrentHttpContext.Items.Add(PageCacheDependencyKeys.PageData, new List<CacheDependencyKey>());
            }
            ((List<CacheDependencyKey>)SystemManager.CurrentHttpContext.Items[PageCacheDependencyKeys.PageData]).AddRange(cacheDependencyNotifiedObjects);
        }

        private static IList<CacheDependencyKey> GetCacheDependencyObjects(Type dataType)
        {
            var cacheDependencyNotifiedObjects = new List<CacheDependencyKey>();

            cacheDependencyNotifiedObjects.Add(new CacheDependencyKey() { Type = dataType });

            return cacheDependencyNotifiedObjects;
        }
    }
}