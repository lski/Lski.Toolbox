using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Lski.Toolbox.Misc {

	/// <summary>
	/// Using code from Msdn to enable running code as another user. Has been created with IDisposible so it can be used in a using statement
	/// </summary>
	public class Impersonate : IDisposable {

		public const int LOGON32_LOGON_INTERACTIVE = 2;
		public const int LOGON32_PROVIDER_DEFAULT = 0;

		WindowsImpersonationContext impersonationContext;
		String Domain;
		String Username;
		String Password;

		[DllImport("advapi32.dll")]
		public static extern int LogonUserA(
			String lpszUserName,
			String lpszDomain,
			String lpszPassword,
			int dwLogonType,
			int dwLogonProvider,
			ref IntPtr phToken);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int DuplicateToken(
			IntPtr hToken,
			int impersonationLevel,
			ref IntPtr hNewToken);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool RevertToSelf();

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern bool CloseHandle(IntPtr handle);

		/// <summary>
		/// States whether impersonation has been successful and is running. If not handle the issue.
		/// </summary>
		public Boolean IsImpersonated { get; set; }

		/// <summary>
		/// Creates a new impersonation object, that means code run between this constructor and being disposed (undone) is run as the user passed
		/// </summary>
		/// <param name="username">Username to impersonate</param>
		/// <param name="domain">The domain the user lives in</param>
		/// <param name="password">The users password</param>
		public Impersonate(String username, String domain, String password) {

			this.Username = username;
			this.Password = password;
			this.Domain = domain;
			this.IsImpersonated = this.RunImpersonation();
		}

		/// <summary>
		/// Creates a new impersonation object, that means code run between this constructor OR the the run command and being disposed (undone) is run as the user passed
		/// </summary>
		/// <param name="username">Username to impersonate</param>
		/// <param name="domain">The domain the user lives in</param>
		/// <param name="password">The users password</param>
		/// <param name="impersonateNow">If true, calls RunImpersonate automatically, true by default</param>
		public Impersonate(String username, String domain, String password, Boolean impersonateNow) {

			this.IsImpersonated = false;
			this.Username = username;
			this.Password = password;
			this.Domain = domain;

			if (impersonateNow)
				this.IsImpersonated = this.RunImpersonation();
		}

		/// <summary>
		/// When called it means the code is run as the user stated in the constructor
		/// </summary>
		/// <param name="Username"></param>
		/// <param name="Domain"></param>
		/// <param name="Password"></param>
		/// <returns></returns>
		public bool RunImpersonation() {

			WindowsIdentity tempWindowsIdentity;
			IntPtr token = IntPtr.Zero;
			IntPtr tokenDuplicate = IntPtr.Zero;

			if (RevertToSelf()) {

				if (LogonUserA(Username, Domain, Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token) != 0) {
					
					if (DuplicateToken(token, 2, ref tokenDuplicate) != 0) {
						
						tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
						impersonationContext = tempWindowsIdentity.Impersonate();
			
						if (impersonationContext != null) {
							CloseHandle(token);
							CloseHandle(tokenDuplicate);
							return true;
						}
					}
				}
			}

			if (token != IntPtr.Zero)
				CloseHandle(token);

			if (tokenDuplicate != IntPtr.Zero)
				CloseHandle(tokenDuplicate);

			return false;
		}

		// Calls UndoImpersonation to end the impersonation
		public void Dispose() {

			UndoImpersonation();
		}

		/// <summary>
		/// Undo the added impersonation, created when the object was created
		/// </summary>
		public void UndoImpersonation() {

			this.IsImpersonated = false;

			if (impersonationContext != null)
				impersonationContext.Undo();
		}
	}
}
