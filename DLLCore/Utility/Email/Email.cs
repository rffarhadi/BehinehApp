using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.Utility.Email
{
    public class Email
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Smtp">smtp</param>
        /// <param name="From">ایمیل فرستنده</param>
        /// <param name="Password">رمز عبور ایمیل</param>
        /// <param name="To">ایمیل گیرنده</param>
        /// <param name="Subject">موضوع ایمیل</param>
        /// <param name="Body">متن ایمیل</param>
        public void SendEmail(string Smtp, string From, string Password, string To, string Bcc, string Subject, string Body)
        {
            MailMessage MyEmail = new MailMessage();

            MyEmail.From = new MailAddress(From);
            foreach (var item in To.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                MyEmail.To.Add(item);
            }
            foreach (var item in Bcc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                MyEmail.Bcc.Add(item);
            }

            MyEmail.Subject = Subject;
            MyEmail.Body = Body;
            MyEmail.IsBodyHtml = true;
            MyEmail.Priority = MailPriority.High;

            SmtpClient mysmtp = new SmtpClient(Smtp);
            mysmtp.UseDefaultCredentials = false;
            mysmtp.EnableSsl = true;
            mysmtp.Port = 587;
            mysmtp.Credentials = new System.Net.NetworkCredential(From, Password);
            mysmtp.Send(MyEmail);

        }
    }
}
