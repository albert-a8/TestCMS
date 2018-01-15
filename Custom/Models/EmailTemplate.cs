using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.DynamicModules.Model;
using SitefinityWebApp.Custom.Extensions;


using System.ComponentModel;namespace SitefinityWebApp.Custom.Models
{
    public class EmailTemplate
    {
        public enum Fields
        {
            [Description("Title")]
            Title,

            [Description("BodyHTML")]
            BodyHTML,

            [Description("Subject")]
            Subject,
        }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string BodyHTML { get; set; }

        public EmailTemplate()
        { 
        
        }

        public EmailTemplate(DynamicContent content)
        {
            Id = content.Id;
            Title = content.GetStringFieldValue(EmailTemplate.Fields.Title.GetDescription());
            BodyHTML = content.GetStringFieldValue(EmailTemplate.Fields.BodyHTML.GetDescription());
            Subject = content.GetStringFieldValue(EmailTemplate.Fields.Subject.GetDescription());
        }

        public EmailTemplate(Guid templateId, string title)
        {
            Id = templateId;
            Title = title;
        }
    }
}