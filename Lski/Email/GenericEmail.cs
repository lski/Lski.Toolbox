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

		private String _Username;
		private String _Password;
		private int _Timeout;
		private String _Server;

		[DataMember(Order = 0)]
		public String Server {
			get { return _Server; }
			set { _Server = value; }
		}

		[DataMember(Order = 1)]
		public int Timeout {
			get { return _Timeout; }
			set { _Timeout = value; }
		}

		[DataMember(Order = 2)]
		public String Username {
			get { return _Username; }
			set { _Username = value ?? String.Empty; }
		}

		[DataMember(Order = 3)]
		public String Password {
			get { return _Password; }
			set { _Password = value ?? String.Empty; }
		}
	}
}
