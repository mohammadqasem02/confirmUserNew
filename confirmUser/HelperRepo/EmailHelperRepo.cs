
using confirmUser.Utils.Encryption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using static System.Net.Mime.MediaTypeNames;

namespace confirmUser.HelperRepo
{
    public class EmailHelperRepo : IEmailHelperRepo
    {
        public static bool IsStagingEnvironment { get; set; } = true;
        private readonly IConfiguration _IConfiguration;
        private readonly IStringLocalizer<EmailHelperRepo> _localizer;
        public EmailHelperRepo(IConfiguration _configuration, IStringLocalizer<EmailHelperRepo> localizer, IWebHostEnvironment webHostEnvironment)
        {
            _IConfiguration = _configuration;
            _localizer = localizer;
           
        }

        bool IEmailHelperRepo.SendErrorEmail(string eNemailBody, string emailSubject, string JSonBody, Exception ex)
        {
            throw new NotImplementedException();
        }

        bool IEmailHelperRepo.SendMail(string eNemailBody, string emailSubject, List<string> sentTo, string link, List<string> ccmail, List<string> bccmail, List<string> AttachmentPath)
        {
            
                bool result;
                try
                {
                    //#region Staging Emails
                    //if (IsStagingEnvironment)
                    //{
                    //    sentTo = new List<string>() { _localizer["SentToItServiceTeam"].ToString() };
                    //    ccmail = new List<string>() { _localizer["SentToItServiceTeam"].ToString() };
                    //    bccmail = new List<string>() { _localizer["SentToItServiceTeam"].ToString() };
                    //}
                    //#endregion

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    ContentType HTMLType = new System.Net.Mime.ContentType("text/html");
                    using (MailMessage message = new MailMessage())
                    {

                        message.Body += eNemailBody;

                        if (link != string.Empty)
                        {
                            message.Body += "<a href='" + link + "'>" + link + "</a><br/>";
                        }


                        var strImageUrl = Directory.GetCurrentDirectory() + "\\wwwroot\\Images\\Logo 1.png";

                        LinkedResource linkedImage = new LinkedResource(strImageUrl)
                        {
                            ContentId = "PIC",
                            ContentType = new ContentType(Image.Png)
                        };

                        message.IsBodyHtml = true;
                        message.Body += string.Format(_localizer["footerBody_Email"].Value);

                        if (AttachmentPath != null)
                        {
                            foreach (var item in AttachmentPath)
                            {
                                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(item, MediaTypeNames.Application.Octet);
                                message.Attachments.Add(attachment);
                            }

                        }

                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                          message.Body,
                          null, "text/html");

                        htmlView.LinkedResources.Add(linkedImage);
                        message.AlternateViews.Add(htmlView);

                        var SenderEmail = EncryptionHelper.Decrypt(_IConfiguration["SenderEmail"].ToString());
                        var SenderPassword = EncryptionHelper.Decrypt(_IConfiguration["SenderPassword"].ToString());
                        var port = _IConfiguration["port"].ToString();
                        var host = _IConfiguration["host"].ToString();
                        if (!string.IsNullOrEmpty(SenderEmail) && !string.IsNullOrEmpty(SenderPassword) && !string.IsNullOrEmpty(port))

                            message.Subject = IsStagingEnvironment ? "staging- " + emailSubject : emailSubject;
                        message.From = new MailAddress(SenderEmail);
                        if (sentTo != null)
                        {
                            foreach (var item in sentTo)
                            {
                                if (item.Contains(","))
                                    item.Replace(",", ";");

                                foreach (var email in item.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    message.To.Add(email);
                                }
                            }

                        }

                        if (ccmail != null)
                        {
                            foreach (var item in ccmail)
                            {
                                if (item.Contains(","))
                                    item.Replace(',', ';');

                                foreach (var EmailCC in item.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    message.CC.Add(EmailCC);
                                }
                            }
                        }

                        if (bccmail != null)
                        {
                            foreach (var item in bccmail)
                            {
                                if (item.Contains(","))
                                    item.Replace(',', ';');
                                foreach (var EmailBcc in item.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    message.Bcc.Add(EmailBcc);
                                }
                            }

                        }


                        SmtpClient smtpClient = new SmtpClient()
                        {
                            Host = host,
                            UseDefaultCredentials = false,
                            Port = int.Parse(port),
                            Credentials = new System.Net.NetworkCredential(SenderEmail, SenderPassword),
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network
                        };
                        smtpClient.Send(message);
                        result = true;
                    }
                }
                catch (SmtpException ex)
                {
                    result = false;
                }

                return result;
            }
        }
    }

