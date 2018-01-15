using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Events.Model;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Modules.Events;

namespace SitefinityWebApp.Custom.Helpers
{
    public static class EventsManagementHelper
    {
        public static IEnumerable<Event> GetActiveEvents()
        {
            EventsManager eventsManager = EventsManager.GetManager();            
            return eventsManager.GetEvents()
                .Where(x => x.Status == ContentLifecycleStatus.Live && x.Visible
                && (!x.EventEnd.HasValue || (x.EventEnd.HasValue && x.EventEnd.Value.Date >= DateTime.UtcNow.Date)));
        }
    }
}