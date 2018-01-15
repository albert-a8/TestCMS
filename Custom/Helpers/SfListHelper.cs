using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Modules.Lists;
using Telerik.Sitefinity.Lists.Model;

namespace SitefinityWebApp.Custom.Helpers
{
    public static class SfListHelper
    {
        public static Dictionary<Guid, string> GetListItemsDictionary(string listUrl)
        {
            Dictionary<Guid, string> listItems = new Dictionary<Guid, string>();
            try
            {
                IQueryable<ListItem> items = ListsManager.GetManager().GetListItems().Where(x => x.Parent.UrlName == listUrl && x.Status == ContentLifecycleStatus.Live);
                //Needed to loop because items.Select(x => x.Title.ToString()) is throwing an error
                foreach (ListItem item in items)
                    listItems.Add(item.Id, item.Title);
            }
            catch (Exception ex)
            {
                listItems = new Dictionary<Guid, string>();
            }
            return listItems;
        }
    }
}