using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Windows.Forms;

namespace EmailSender
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                if (args.Length != 9)
                {
                    Console.WriteLine("Invalid number of parameters.");
                    Console.WriteLine("Usage: EmailSender.exe \"server.com\" \"user@server.com\" \"password\" \"from@domain.com\" \"FromName\" \"to@domain.com\" \"Subject\" \"Body\" \"jpg,txt,png\"");
                    return;
                }

                //Get parameters
                var serverAdress = args[0];
                var serverUser = args[1];
                var serverPassword = args[2];

                var from = args[3];
                var fromName = args[4];
                var to = args[5];
                var subject = args[6];
                var body = args[7];

                var extensions = args[8];

                //Get the files in the path
                var files = new List<string>();
                foreach (var ext in extensions.Split(','))
                    files.AddRange(Directory.GetFiles(Application.StartupPath, "*." + ext));

                //Config SMTP servert
                var server = new SmtpClient(serverAdress, 25)
                {
                    Credentials = new System.Net.NetworkCredential(serverUser, serverPassword),
                    UseDefaultCredentials = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true
                };

                //Config the mail
                var mail = new MailMessage
                {
                    From = new MailAddress(from, fromName),
                    Subject = subject,
                    Body = body
                };
                mail.To.Add(new MailAddress(to));

                //Config the attach
                foreach (var file in files)
                    mail.Attachments.Add(new Attachment(file));

                //Send!
                server.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}