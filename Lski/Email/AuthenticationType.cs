using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Email {

	/// <summary>
	/// States an authentication type as used by MSExchange
	/// </summary>
	public enum AuthenticationType : byte {
		NTLM = 1,
		Negotiate = 2,
		Basic = 3,
		Kerberos = 4
	}
}
