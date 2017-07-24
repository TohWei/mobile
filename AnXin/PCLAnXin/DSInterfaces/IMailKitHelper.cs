using System;

namespace PCLAnXin
{
	public interface IMailKitHelper
	{
		void SendMail();
		void SendMailGmail();
		void SendYahooEmail(string receiverEmailAddress, string masterPass);
	}
}

