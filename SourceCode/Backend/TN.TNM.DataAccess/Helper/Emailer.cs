using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Results;
using TN.TNM.DataAccess.Databases;
using System.IO;
using System.Net.Mime;

namespace TN.TNM.DataAccess.Helper
{
    public static class Emailer
    {
        private const string ForwardEmailHeaderFormat = "<i>Email này sẽ được gửi cho: {0}</i><br /><br />";

        static Emailer()
        {
            
        }

        /// <summary>
        ///     Send an email without attach files
        /// </summary>
        /// <param name="fromAddress">Email address of sender</param>
        /// <param name="toAddress">Email address of receivers</param>
        /// <param name="ccAddress"></param>
        /// <param name="subject">subject of the email</param>
        /// <param name="message">content of the email</param>
        /// <returns>True: if send email successful, false: if send email fail</returns>
        public static BaseResult SendEmail(TNTN8Context context, string fromAddress, IEnumerable<string> toAddress,
            IEnumerable<string> ccAddress, string subject, string message)
        {
            return SendEmailNow(context, fromAddress, toAddress, ccAddress, subject, message);
        }
        /// <summary>
        ///     Send an email without attach files
        /// </summary>
        /// <param name="toAddress">Email address of receivers</param>
        /// <param name="ccAddress"></param>
        /// <param name="subject">subject of the email</param>
        /// <param name="message">content of the email</param>
        /// <returns>True: if send email successful, false: if send email fail</returns>
        public static BaseResult SendEmail(TNTN8Context context, IEnumerable<string> toAddress,
            IEnumerable<string> ccAddress, string subject, string message)
        {
            var SmtpEmailAccount =
                context.SystemParameter.FirstOrDefault(w => w.SystemKey == "Email")?.SystemValueString;
            return SendEmailNow(context, SmtpEmailAccount, toAddress, ccAddress, subject, message);
        }
        /// <summary>
        ///     Send and email with attach files
        /// </summary>
        /// <param name="fromAddress">Email address of sender</param>
        /// <param name="toAddress">Email address of receivers</param>
        /// <param name="ccAddress"></param>
        /// <param name="subject">subject of the email</param>
        /// <param name="message">content of the email</param>
        /// <param name="attachFiles">Attach files</param>
        /// <returns>True: if send email successful, false: if send email fail</returns>
        public static BaseResult SendEmailWithAttachments(TNTN8Context context, string fromAddress, IEnumerable<string> toAddress,
            IEnumerable<string> ccAddress, string subject, string message
            , List<string> attachFiles)
        {
            return SendEmailNow(context, fromAddress, toAddress, ccAddress, subject, message, attachFiles);
        }

        /// <summary>
        ///     Send an email to receivers with attach files if needed
        /// </summary>
        /// <param name="fromAddress">Email address of sender</param>
        /// <param name="toAddress">Email address of receivers</param>
        /// <param name="ccAddress"></param>
        /// <param name="subject">subject of the email</param>
        /// <param name="message">content of the email</param>
        /// <param name="attachFiles">Attach files</param>
        /// <returns>True: if send email successful, false: if send email fail</returns>
        private static BaseResult SendEmailNow(TNTN8Context context, string fromAddress, IEnumerable<string> toAddress,
            IEnumerable<string> ccAddress, string subject, string message
            , List<string> attachFiles = null)
        {
            var feedback = new BaseResult();

            #region Lấy thông tin cấu hình

            bool UsingDefaultReceiverEmail =
                context.SystemParameter.FirstOrDefault(x => x.SystemKey == "UsingDefaultReceiverEmail").SystemValue
                    .Value;
            
            var DefaultReceiverEmail = context.SystemParameter
                .FirstOrDefault(x => x.SystemKey == "DefaultReceiverEmail")?
                .SystemValueString;

            if (UsingDefaultReceiverEmail && !ValidateEmailAddress(DefaultReceiverEmail))
                UsingDefaultReceiverEmail = false;

            string SmtpEmailAccount =
                context.SystemParameter.FirstOrDefault(w => w.SystemKey == "Email")?.SystemValueString;

            string SmtpPassword =
                context.SystemParameter.FirstOrDefault(w => w.SystemKey == "Password").SystemValueString;

            string SmtpServer =
                context.SystemParameter.FirstOrDefault(w => w.SystemKey == "PrimaryDomain")?.SystemValueString;

            int SmtpPort =
                int.Parse(context.SystemParameter.FirstOrDefault(w => w.SystemKey == "PrimaryPort")?.SystemValueString);

            bool SmtpSsl =
                context.SystemParameter.FirstOrDefault(x => x.SystemKey == "Ssl").SystemValue.Value;

           #endregion

            //Config for using default receiver email

            var toEmailAddress = new List<string>(); //Config for using default receiver email
            var ccEmailAddress = new List<string>(); //Config for using default receiver email
            if (!UsingDefaultReceiverEmail)
            {
                toEmailAddress.AddRange(toAddress);
                ccEmailAddress.AddRange(ccAddress);
            }
            else
            {
                toEmailAddress.Add(DefaultReceiverEmail);
                message = string.Format(ForwardEmailHeaderFormat,
                    string.Concat("TO:", ConvertSeriesAddress(toAddress), ", CC:", ConvertSeriesAddress(ccAddress)) +
                    message);
            }

            // Validate sender and receiver email addresses););
            if (!ValidateEmailAddress(fromAddress))
            {
                feedback.Status = false;
                feedback.Message = "Emai không đúng định dạng";
                return feedback;
            }

            if (!ValidateEmailAddress(toEmailAddress))
            {
                feedback.Status = false;
                feedback.Message = "Emai không đúng định dạng";
                return feedback;
            }

            using (var mailMessage = new MailMessage())
            {
                // Credentials are necessary if the server requires the client 
                // to authenticate before it will send e-mail on the client's behalf.
                // smtp.UseDefaultCredentials = true;
                // smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                // Check we have email addresses
                var smtpClient = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(SmtpEmailAccount, SmtpPassword),
                    Host = SmtpServer,
                    Port = SmtpPort,
                    EnableSsl = SmtpSsl
                };

                // Build Email
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress(fromAddress,"TNM");
                foreach (var addReceiver in toEmailAddress) mailMessage.To.Add(addReceiver);
                foreach (var ccReceiver in ccEmailAddress) mailMessage.CC.Add(ccReceiver);
                AlternateView alterView = ContentToAlternateView(message);
                mailMessage.AlternateViews.Add(alterView);
                //mailMessage.Body = message;
                mailMessage.Subject = subject;
                // Add Attachments
                if (attachFiles != null)
                    foreach (var attachFile in attachFiles)
                    {
                        var attachment = new Attachment(attachFile);
                        mailMessage.Attachments.Add(attachment);
                    }

                try
                {
                    smtpClient.Send(mailMessage);
                }
                catch (SmtpException ex)
                {
                    feedback.Status = false;
                    feedback.Message = ex.ToString();
                }

                return feedback;
            }
        }

        public  static BaseResult SendMailWithIcsAttachment(TNTN8Context context, IEnumerable<string> toAddress,
            IEnumerable<string> ccAddress, string subject, string message, DateTime TimeStart, DateTime? TimeEnd, Guid customerMeetingId,
            string address, bool isDelete)
        {
            var feedback = new BaseResult();
         
            #region Lấy thông tin cấu hình

            bool UsingDefaultReceiverEmail =
                context.SystemParameter.FirstOrDefault(x => x.SystemKey == "UsingDefaultReceiverEmail").SystemValue
                    .Value;

            var DefaultReceiverEmail = context.SystemParameter
                .FirstOrDefault(x => x.SystemKey == "DefaultReceiverEmail")?
                .SystemValueString;

            if (UsingDefaultReceiverEmail && !ValidateEmailAddress(DefaultReceiverEmail))
                UsingDefaultReceiverEmail = false;

            //string SmtpEmailAccount =
            //    context.SystemParameter.FirstOrDefault(w => w.SystemKey == "Email")?.SystemValueString;

            //string SmtpPassword =
            //    context.SystemParameter.FirstOrDefault(w => w.SystemKey == "Password").SystemValueString;

            //string SmtpServer =
            //    context.SystemParameter.FirstOrDefault(w => w.SystemKey == "PrimaryDomain")?.SystemValueString;

            //int SmtpPort =
            //    int.Parse(context.SystemParameter.FirstOrDefault(w => w.SystemKey == "PrimaryPort")?.SystemValueString);

            //bool SmtpSsl =
            //    context.SystemParameter.FirstOrDefault(x => x.SystemKey == "Ssl").SystemValue.Value;
            string SmtpEmailAccount = "dongphuongyhocquanly@gmail.com";
            string SmtpPassword = "haidenk45";
            string SmtpServer = "smtp.gmail.com";
            int SmtpPort = 587;
            bool SmtpSsl = true;

            #endregion

            //Config for using default receiver email
            var toEmailAddress = new List<string>(); //Config for using default receiver email
            var ccEmailAddress = new List<string>(); //Config for using default receiver email
            if (!UsingDefaultReceiverEmail)
            {
                var listAddress = new List<string> { "gianglt@tringhiatech.vn", "lehai0196@gmail.com", "hailt@tringhiatech.vn" };
                toEmailAddress.AddRange(listAddress);
                //if (ccAddress != null)
                //{
                //    ccEmailAddress.AddRange(ccAddress);
                //}
            }
            else
            {
                toEmailAddress.Add(DefaultReceiverEmail);
                message = string.Format(ForwardEmailHeaderFormat,
                    string.Concat("TO:", ConvertSeriesAddress(toAddress), ", CC:", ConvertSeriesAddress(ccAddress)) +
                    message);
            }

            // Validate sender and receiver email addresses););
            if (!ValidateEmailAddress(SmtpEmailAccount))
            {
                feedback.Status = false;
                feedback.Message = "Emai không đúng định dạng";
                return feedback;
            }

            if (!ValidateEmailAddress(toEmailAddress))
            {
                feedback.Status = false;
                feedback.Message = "Emai không đúng định dạng";
                return feedback;
            }

            using (var mailMessage = new MailMessage())
            {
                var smtpClient = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(SmtpEmailAccount, SmtpPassword),
                    Host = SmtpServer,
                    Port = SmtpPort,
                    EnableSsl = SmtpSsl
                };

                #region Build Mail
                mailMessage.Headers.Add("Content-class", "urn:content-classes:calendarmessage");
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress(SmtpEmailAccount, "TNM");
                foreach (var addReceiver in toEmailAddress) mailMessage.To.Add(addReceiver);
                foreach (var ccReceiver in ccEmailAddress) mailMessage.CC.Add(ccReceiver);
                AlternateView alterView = ContentToAlternateView(message);
                mailMessage.AlternateViews.Add(alterView);
                mailMessage.Subject = subject;

                // Now Contruct the ICS file using string builder
                StringBuilder str = new StringBuilder();
                str.AppendLine("BEGIN:VCALENDAR");
                str.AppendLine("PRODID:-//Schedule a Meeting");
                str.AppendLine("VERSION:2.0");
                if (!isDelete)
                {
                    str.AppendLine("METHOD:REQUEST");
                }
                else
                {
                    str.AppendLine("METHOD:CANCEL");
                }
                str.AppendLine("BEGIN:VEVENT");
                var startTime = TimeStart;
                str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", TimeZoneInfo.ConvertTimeToUtc(startTime, TimeZoneInfo.Local)));
                str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
                var endTime = TimeEnd ?? TimeStart.AddMinutes(+60);
                str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", TimeZoneInfo.ConvertTimeToUtc(endTime, TimeZoneInfo.Local)));
                str.AppendLine("LOCATION: " + address);
                str.AppendLine(string.Format("UID:{0}", customerMeetingId));
                str.AppendLine(string.Format("DESCRIPTION:{0}", mailMessage.Body));
                str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", mailMessage.Body));
                str.AppendLine(string.Format("SUMMARY:{0}", mailMessage.Subject));
                str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", mailMessage.From.Address));
                str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", mailMessage.To[0].DisplayName, mailMessage.To[0].Address));

                str.AppendLine("BEGIN:VALARM");
                str.AppendLine("TRIGGER:-PT15M");
                str.AppendLine("ACTION:DISPLAY");
                str.AppendLine("DESCRIPTION:Reminder");
                str.AppendLine("END:VALARM");
                str.AppendLine("END:VEVENT");
                str.AppendLine("END:VCALENDAR");

                System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
                contype.Parameters.Add("method", "REQUEST");
                contype.Parameters.Add("name", "Meeting.ics");
                AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), contype);
                mailMessage.AlternateViews.Add(avCal);

                try
                {
                    smtpClient.Send(mailMessage);
                }
                catch (SmtpException ex)
                {
                    feedback.Status = false;
                    feedback.Message = ex.ToString();
                }

                return feedback;
                #endregion
            }
        }

        /// <summary>
        ///     Validate and email address
        ///     It must be follow these rules:
        ///     Has only one @ character
        ///     Has at least 3 chars after the @
        ///     Domain portion contains at least one dot
        ///     Dot can't be before or immediately after the @ character
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>True: If valid, False: If not</returns>
        private static bool ValidateEmailAddress(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress)) return false;

            if (!Regex.IsMatch(emailAddress, "^[-A-Za-z0-9_@.]+$")) return false;

            // Search for the @ char
            var i = emailAddress.IndexOf("@", StringComparison.Ordinal);

            // There must be at least 3 chars after the @
            if (i <= 0 || i >= emailAddress.Length - 3) return false;

            // Ensure there is only one @
            if (emailAddress.IndexOf("@", i + 1, StringComparison.Ordinal) > 0) return false;


            // Check the domain portion contains at least one dot
            var j = emailAddress.LastIndexOf(".", StringComparison.Ordinal);

            // It can't be before or immediately after the @ character
            if (j < 0 || j <= i + 1) return false;

            // EmailAddress is validated
            return true;
        }

        /// <summary>
        ///     Validate list email address
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>true: If all of email address are valid, false: If one of email address is invalid</returns>
        private static bool ValidateEmailAddress(IEnumerable<string> emailAddress)
        {
            foreach (var email in emailAddress)
            {
                // ReShaper suggest declare variable in here
                var retValue = ValidateEmailAddress(email);
                if (!retValue) return false;
            }

            return true;
        }

        private static string ConvertSeriesAddress(IEnumerable<string> address)
        {
            if (address == null) return null;

            var sb = new StringBuilder();
            foreach (var addr in address) sb.Append(addr + ", ");

            if (sb.Length > 0) sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }
        private static AlternateView ContentToAlternateView(string content)
        {
            var imgCount = 0;
            List<LinkedResource> resourceCollection = new List<LinkedResource>();
            foreach (Match m in Regex.Matches(content, "<img(?<value>.*?)>"))
            {
                imgCount++;
                var imgContent = m.Groups["value"].Value;
                string type = Regex.Match(imgContent, ":(?<type>.*?);base64,").Groups["type"].Value;
                string base64 = Regex.Match(imgContent, "base64,(?<base64>.*?)\"").Groups["base64"].Value;
                if (String.IsNullOrEmpty(type) || String.IsNullOrEmpty(base64))
                {
                    //ignore replacement when match normal <img> tag
                    continue;
                }
                var replacement = " src=\"cid:" + imgCount + "\"";
                content = content.Replace(imgContent, replacement);
                var tempResource = new LinkedResource(Base64ToImageStream(base64), new ContentType(type))
                {
                    ContentId = imgCount.ToString()
                };
                resourceCollection.Add(tempResource);
            }

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(content, null, MediaTypeNames.Text.Html);
            foreach (var item in resourceCollection)
            {
                alternateView.LinkedResources.Add(item);
            }

            return alternateView;
        }

        private static Stream Base64ToImageStream(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            return ms;
        }
    }
}