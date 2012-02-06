using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using System.ComponentModel;
using Lski.Txt;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace Lski.Misc {

	public class Applications {

		/// <summary>
		/// Simple selection of known application registry key names that can be used with IsInstalled
		/// </summary>
		public partial class Apps {

			public const String WORD = "Word.Application";
			public const String EXCEL = "Excel.Application";
			public const String ACCESS = "Access.Application";
		}

		/// <summary>
		/// Checks the currently running system computer to see if the passed application is installed on the local computer
		/// </summary>
		/// <param name="App"></param>
		/// <returns></returns>
		/// <remarks>Checks the currently running system computer to see if the passed application is installed on the local computer.
		/// 
		/// Note: The check is performed using the system registry rather than attempting to load the actual application, which saves 
		/// resources
		/// </remarks>
		public static bool IsInstalled(string app) {

			bool appInstalled = false;
			string strSubKey = null;
			RegistryKey objKey = null;
			RegistryKey objSubKey = null;

			objKey = Registry.ClassesRoot;
			objSubKey = objKey.OpenSubKey(strSubKey);
			appInstalled = (objSubKey != null);
			objKey.Close();

			return appInstalled;

		}
	}

}
