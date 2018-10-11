using Outlook = Microsoft.Office.Interop.Outlook;
using System;
using Microsoft.Office.Interop.Outlook;

namespace Selenium.QuickStart.Utilities
{
    public static class EmailSender
    {
        public static void SendEmail(string[] emailDestinations, string emailSubject, string filePathToBeAttached, string emailBody)
        {
            Application application = new Application();
            Outlook.MailItem mail = application.CreateItem(
                Outlook.OlItemType.olMailItem) as Outlook.MailItem;
            mail.Subject = emailSubject;
            Outlook.AddressEntry currentUser =
                application.Session.CurrentUser.AddressEntry;
            if (currentUser.Type == "EX")
            {
                foreach (string email in emailDestinations)
                {
                    if(!String.IsNullOrEmpty(email) && !String.IsNullOrWhiteSpace(email))
                        mail.Recipients.Add(email);
                }
                mail.Recipients.ResolveAll();
                mail.Attachments.Add(filePathToBeAttached,
                    Outlook.OlAttachmentType.olByValue, Type.Missing,
                    Type.Missing);
                mail.Body = emailBody;
                if(mail.Recipients.Count>0)
                    mail.Send();
            }
        }
    }
}
