using System;
using Xamarin.Forms;
using AnXin.Droid;
using MimeKit;
using MailKit.Net.Smtp;
using PCLAnXin;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.Mail;
using System.Net;
using Android.Widget;

[assembly: Dependency(typeof(MailKitDroid))]
namespace AnXin.Droid
{
	public class MailKitDroid : IMailKitHelper
	{
		public MailKitDroid ()
		{
		}


		//Tested: doesnot work
		public void SendMail()
		{
			var message = new MimeMessage ();
			message.From.Add(new MailboxAddress("Jeff Lim", "sender@gmail.com"));
			message.To.Add (new MailboxAddress ("Jeff", "receiver@gmail.com"));
			message.Subject = "Test SMTP client";
			message.Body = new TextPart("plain"){
				Text = @"Hello jeff"
			};

			using (var client = new MailKit.Net.Smtp.SmtpClient ()) {
				//client.Connect ("smtp.gmail.com", 587, true);

				//client.AuthenticationMechanisms.Remove ("XOAUTH2"); 
				//client.ServerCertificateValidationCallback = delegate(object sender, X509Certificate certificate, X509Chain X509Chain, SslPolicyErrors sslPolicyErrors) {
				//	return true;
				//};

				client.Connect ("smtp.gmail.com", 587, true);
				client.Authenticate ("afoofacaci@gmail.com", "");

				client.Send (message);
				client.Disconnect (true);
			}
		}


		//Tested: doesnot work
		public void SendMailGmail(){
			try{
				MailMessage mail = new MailMessage ();
				System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient ("smtp.gmail.com");
				mail.From = new MailAddress ("sender@gmail.com");
				mail.To.Add ("receiver@artesiansolutions.com");
				mail.Body = "Test SMTP client";
				SmtpServer.Port = 587;
				SmtpServer.Credentials = new System.Net.NetworkCredential ("sender@gmail.com", "");
				SmtpServer.EnableSsl = true;
				ServicePointManager.ServerCertificateValidationCallback=delegate(object sender, X509Certificate certificate, X509Chain X509Chain, SslPolicyErrors sslPolicyErrors) 
				{
					return true;
				};
				SmtpServer.Send (mail);
				Toast.MakeText (Forms.Context, "Mail send successfully", ToastLength.Short).Show ();
			}catch(Exception e){
				throw;
				//Toast.MakeText (Forms.Context, e.ToString(), ToastLength.Long).Show ();
			}
		}


		public void SendYahooEmail(string receiverEmailAddress, string masterPass){
			string smtpAddress = "smtp.mail.yahoo.com";
			int portnumber = 587;
			bool enableSSL = true;

			try{
			using (MailMessage mail = new MailMessage ()) {
				mail.From = new MailAddress ("sender@yahoo.com","JJJC");
				mail.To.Add (receiverEmailAddress);
				mail.Subject = "AnXin app - You have setup an master account in AnXin app.";
				mail.Body = "You master pass in AnXin app is: " + masterPass;

				using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient (smtpAddress, portnumber)) 
				{
					smtp.Credentials = new NetworkCredential ("test@yahoo.com", "testpassword");
						smtp.EnableSsl = enableSSL;
					smtp.Send (mail);
				}
			}
			}catch(Exception ex){
				throw;
			}
		}
	}
}

