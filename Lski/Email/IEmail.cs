using System;

namespace Lski.Email {

	/// <summary>
	/// Represents a flat record with each of the fields for each of the types. Designed so that a flat database record containing settings can be
	/// used to fill an appropriate EmailSettings class 
	/// </summary>
	public interface IEmail {

		AuthenticationType AuthType { get; set; }
		string Domain { get; set; }
		string Type { get; set; }
		string Password { get; set; }
		short Port { get; set; }
		string Server { get; set; }
		short Timeout { get; set; }
		string Username { get; set; }
		string Mailbox { get; set; }
	}
}
