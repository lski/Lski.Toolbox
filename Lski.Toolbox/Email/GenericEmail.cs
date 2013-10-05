using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Lski.Email {
	
    /// <summary>
    /// A basic intermidiatary class used to bind together and be a super to each of the solid implementations, also provides the defaut ordering of data members 
    /// </summary>
	public abstract class GenericEmail : Email {

		public GenericEmail() : base() {
			this.Username = String.Empty;
			this.Password = String.Empty;
		}

		public String Server { get; set; }
		public int Timeout { get ; set; }
		public String Username { get; set; }
		public String Password { get; set; }
	}
}
