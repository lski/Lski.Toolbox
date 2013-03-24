using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using Lski.Txt;

namespace Lski.Email {
	
	/// <summary>
	/// An abstract class, designed to store the settings required to send an email and provide an interface to enabling sending mail using those settings
	/// </summary>
	public abstract class Email {

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
		public void Send(String from, String to, String subject, String body, bool isHtml = false, IEnumerable<String> cc = null, IEnumerable<String> bcc = null) {
			Send(from, new String[] { to }, subject, body, isHtml, cc, bcc);
		}

		public abstract void Send(String from, IEnumerable<String> to, String subject, String body, bool isHtml = false, IEnumerable<String> cc = null, IEnumerable<String> bcc = null);

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
		//public abstract void Send(Email message);

		#region Static Members

		/// <summary>
		/// Opens the users email client. With the appropriate information.
		/// </summary>
		/// <param name="emailAddress"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static Process OpenEmail(string emailAddress, string subject = null, string body = null) {

			string sParams = emailAddress;
			Process pro = null;

			// Check if the email address contains the necessary 'mailTo:' at the beginning, if not add it
			if ((string.Compare(StringExt.SubStringAdv(sParams, 0, 7), "mailto:", true)) != 0) 
				sParams = "mailto:" + sParams;

			// Check if there is a subject to add
			if ((subject != null) && (subject.Length > 0)) 
				sParams = sParams + "?subject=" + subject;

			// Check if there is a body to add
			if ((body != null) && (body.Length > 0)) {
				sParams = sParams + (string.IsNullOrEmpty(subject) ? "?" : "&");
				sParams = (sParams + "body=" + body);
			}

			// Try running the process, Return nothing on fail
			try {
				pro = System.Diagnostics.Process.Start(sParams);
			} catch {
				return null;
			}

			// Return the running process
			return pro;
		}

		#endregion
	}
}
