using System;
using System.Collections.Generic;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using MyCompany.Data;
using MyCompany.Models;
using MyCompany.Utilities;
namespace Chess_Up.Services
{
    public class MailSenderService
    {
        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="model"></param>
        public static void SendMail(MailModel model)
        {
            try
            {
                MailMessage mail = new MailMessage();

                SmtpClient SmtpServer = new SmtpClient(Constants.SMTPCLIENT_MAIL_RU);

                mail.From = new MailAddress(Constants.MAIL_ADDRESS);
                if (model.ToMail != null && model.ToMail.Count >= 1)
                {
                    foreach (var item in model.ToMail)
                    {
                        mail.To.Add(item);
                    }
                }
                else
                {
                    mail.To.Add(Constants.MAIL_ADDRESS);
                }

                mail.Subject = model.Subject;

                if (model.Subject.Equals("Contact Us"))
                {
                    mail.Body = "Name: " + model.Name + ", Email: " + model.YourMail + ", Phone Number: " + model.Phone + ", Mail Body: " + model.Body;
                }
                else
                {
                    mail.Body = model.Body;
                }

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Constants.MAIL_ADDRESS, Constants.MAIL_PASSWORD);

                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
