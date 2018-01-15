using SitefinityWebApp.Custom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.Mail;

namespace SitefinityWebApp.Custom.Helpers
{
    public static class EmailHelper
    {
        public static bool IsEmailAddressExisting(string email)
        {
            UserManager userManager = UserManager.GetManager();
            bool isExisting = userManager.GetUsers().ToList().Any(x => x.Email == email);
            return isExisting;
        }

        public static bool SendEmail(string emailSubject, string emailBody, string recieverEmailAdd, string recieverName, string senderEmailAdd = "", List<string> ccList = null)
        {
            bool isSuccessful = true;
            try
            {
                SmtpElement smtpSettings = Config.Get<SystemConfig>().SmtpSettings;
                MailMessage message = new MailMessage();

                message.From = (senderEmailAdd != "") ? new MailAddress(senderEmailAdd)
                    : new MailAddress(smtpSettings.DefaultSenderEmailAddress);
                message.To.Add(new MailAddress(recieverEmailAdd, recieverName));
                message.Subject = emailSubject;
                message.Body = emailBody.ToString();
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.Unicode;
                message.SubjectEncoding = Encoding.Unicode;

                if (ccList != null && ccList.Any())
                {
                    foreach (string email in ccList)
                    {
                        message.CC.Add(new MailAddress(email));
                    }
                }

                EmailSender.Get().Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccessful;
        }

        public static EmailTemplate GetEmailTemplateFromContent(Guid liveId, string emailTemplateNamespace)
        {
            DynamicContent emailTemplateContent = DynamicContentHelper.GetDynamicContentById(emailTemplateNamespace, liveId);
            if (emailTemplateContent == null)
                throw new Exception(string.Format("EmailTemplate with id: {0} does not exist", liveId.ToString()));
            return new EmailTemplate(emailTemplateContent);
        }
    }
}