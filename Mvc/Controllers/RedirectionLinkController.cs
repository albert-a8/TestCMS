using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using SitefinityWebApp.Mvc.Models;

namespace SitefinityWebApp.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "RedirectionLink", Title = "RedirectionLink", SectionName = "HTML")]
    [Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesigner(typeof(WidgetDesigners.RedirectionLink.RedirectionLinkDesigner))]
    public class RedirectionLinkController : Controller
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [Category("String Properties")]
        public string PageURL { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [Category("String Properties")]
        public string PageName { get; set; }

        /// <summary>
        /// This is the default Action.
        /// </summary>
        public ActionResult Index()
        {
            RedirectionLinkModel model = new RedirectionLinkModel();
            model.PageName = this.PageName;
            model.PageURL = this.PageURL;

            return View(model);
        }
    }
}