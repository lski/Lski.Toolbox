using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Lski.Data.Common;

namespace Lski.Data.Repository {
	
	public class RepoAdv : Repo {

		public RepoAdv(String connectionStringName) : base(connectionStringName)  {}
		public RepoAdv(String connectionString, String providerName) : base(connectionString, providerName) {}
		internal RepoAdv(String connectionString, DbProvider factory) : base(connectionString, factory) {}

		public DbProvider ProviderAdv {
			get { return (DbProvider)this.Provider; }
			set { this.Provider = value; }
		}

		protected override System.Data.Common.DbProviderFactory GetFactory(string connectionString, string providerName) {
			return DbProviders.GetFactoryPrecise(connectionString, providerName);
		}
	}
}
