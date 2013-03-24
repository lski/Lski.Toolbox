using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Net.Mail;
using System.Net;

namespace Lski.Email {

	public class Smtp : GenericEmail {

		public Smtp() : base() {
			this.Port = 25;
		}

		public int Port { get; set; }

		/// <summary>
		/// A method used to send mail to an assocated list of addresses, using the currently set smtpserver.
		/// </summary>
		/// <param name="from">The from address for the email</param>
		/// <param name="sendTo">The to address for the email</param>
		/// <param name="subject">The subject line of the email</param>
		/// <param name="body">The body text</param>
		/// <param name="attachments">The list of full filenames the email should attach</param>
		/// <param name="cc">The list of email addresses to place in the cc field</param>
		/// <param name="bcc">The list of email addressed to place in the bcc field</param>
		/// <remarks>A method used to send mail to an assocated list of addresses, using the currently set smtpserver.
		/// 
		/// If the setting attachmentRequired is set to true in the settings for the email object then the file must be accessable and
		/// must exist otherwise an exception is thrown. If the setting is set to false, then if the file can not be found, the email 
		/// will send without that particular file, but will attempt to add the next file in the list until the list is completely
		/// checked/attached.
		/// </remarks>
		public override void Send(String from, IEnumerable<String> to, String subject, String body, bool isHtml = false, IEnumerable<String> cc = null, IEnumerable<String> bcc = null) {

			MailMessage mess = new MailMessage();

			mess.From = new MailAddress(from);

			foreach (var x in to) {
				mess.To.Add(new MailAddress(x));
			}

			if (cc != null) {
				foreach (var x in cc) {
					mess.CC.Add(x);
				}
			}

			if (bcc != null) {
				foreach (var x in bcc) {
					mess.Bcc.Add(x);
				}
			}

			mess.IsBodyHtml = isHtml;
			mess.Subject = subject;
			mess.Body = body;

			ExecuteSend(mess);
		}

		///// <summary>
		///// A method used to send mail to an assocated list of addresses, using the currently set smtpserver.
		///// </summary>
		///// <param name="from">The from address for the email</param>
		///// <param name="sendTo">The list of to address for the email</param>
		///// <param name="subject">The subject line of the email</param>
		///// <param name="body">The body text</param>
		///// <param name="attachments">The list of full filenames the email should attach</param>
		///// <param name="cc">The list of email addresses to place in the cc field</param>
		///// <param name="bcc">The list of email addressed to place in the bcc field</param>
		///// <exception cref="BodyFileNotFoundException">Thrown if the body is marked as IsFile = True, but the file can not be found to 
		///// read the contents of. Subclass of FileNotFoundException
		///// </exception>
		///// <exception cref="RequiredAttachmentNotFoundException">When an attachment is marked as required but the file can not be found
		///// then this error is thrown. Subclass of FileNotFoundException
		///// </exception>
		///// <exception cref="System.FormatException">If an email address is not correctly formatted</exception>
		///// <remarks>A method used to send mail to an assocated list of addresses, using the currently set smtpserver.
		///// 
		///// If the setting attachmentRequired is set to true in the settings for the email object then the file must be accessable and
		///// must exist otherwise an exception is thrown. If the setting is set to false, then if the file can not be found, the email 
		///// will send without that particular file, but will attempt to add the next file in the list until the list is completely
		///// checked/attached.
		///// </remarks>
		//public override void Send(Email message) {

		//    MailMessage mess = new MailMessage();

		//    // Add each to address to the 'to' header
		//    foreach (var addy in message.TO) {
		//        mess.To.Add(addy.Value);
		//    }

		//    if (message.CC != null) {
		//        //Add each cc address to the to 'cc' header
		//        foreach (var addy in message.CC) {
		//            mess.CC.Add(addy.Value);
		//        }
		//    }


		//    // Add each bcc address to the 'bcc' header
		//    if (message.BCC != null) {
		//        foreach (var addy in message.BCC) {
		//            mess.Bcc.Add(addy.Value);
		//        }
		//    }

		//    // Set the main settings
		//    mess.From = message.From.Value;
		//    mess.Subject = message.Subject;
		//    mess.Body = message.Body.BodyText;

		//    // Change the format of the body text
		//    mess.IsBodyHtml = (message.Body.BodyFormat == EmailFormat.Html);

		//    ExecuteSend(mess);
		//}


		private void ExecuteSend(MailMessage msg) {

			try {

				SmtpClient client = new SmtpClient(this.Server, this.Port);

				if (this.Timeout > 0)
					client.Timeout = this.Timeout;

				if (!String.IsNullOrEmpty(this.Username))
					client.Credentials = new NetworkCredential(this.Username, this.Password);

				client.Send(msg);

				client = null; // Cleanup

			} catch (Exception ex) {

				throw new Exception("There was an error send email using SMTP", ex);
			}


		}
	}
}
