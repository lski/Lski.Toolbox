using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Lski.Windows.Forms.Controls
{

	/// <summary>
	/// Simply provides a version of label that is thread safe on the call to the property text.
	/// </summary>
	/// <remarks></remarks>
	public class AsyncLabel : Label
	{

		private delegate void SetTextCallBack(string text);
		private delegate string GetTextCallBack();

		public override string Text {
			get {
				if (this.InvokeRequired) {
					GetTextCallBack d = new GetTextCallBack(PrvGetText);
					return (string)this.Invoke(d, null);
				} else {
					return base.Text;
				}
			}
			set {
				if (this.InvokeRequired) {
					SetTextCallBack d = new SetTextCallBack(PrvSetText);
					this.Invoke(d, new object[] { value });
				} else {
					base.Text = value;
				}
			}
		}

		private void PrvSetText(string text)
		{
			base.Text = text;
		}

		private string PrvGetText()
		{
			return base.Text;
		}

	}

}
