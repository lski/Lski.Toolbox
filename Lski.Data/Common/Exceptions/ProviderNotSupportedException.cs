using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Data.Common.Exceptions {

	public class ProviderNotSupportedException : ArgumentException {

		private const string _message = "The provider {0} is not currently supported";

		public ProviderNotSupportedException(string factoryName) : base(string.Format(_message, factoryName)) { }
		public ProviderNotSupportedException(string factoryName, Exception innerException) : base(string.Format(_message, factoryName), innerException) { }
	}

}

