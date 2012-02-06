using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Lski.Windows.Forms.Controls
{
	public class TextBoxWithDefault : TextBox {

		#region "Properties"

		private String _defaultText;
		public String DefaultText {
			get { return _defaultText; }
			set { _defaultText = value; }
		}

		private System.Drawing.Color _defaultColor;
		public System.Drawing.Color DefaultColor {
			get { return _defaultColor; }
			set { _defaultColor = value; }
		}

		#endregion


		public TextBoxWithDefault()
		{
			InitDefaults("", this.ForeColor);
		}

		public TextBoxWithDefault(String defaultText)
		{
			InitDefaults(defaultText, this.ForeColor);
		}

		public TextBoxWithDefault(String defaultText, System.Drawing.Color defaultColor)
		{
			InitDefaults(defaultText, defaultColor);
		}

		public void InitDefaults(String defaultText, System.Drawing.Color defaultColor)
		{
			this.DefaultText = defaultText;
			this.DefaultColor = defaultColor;

			this.GotFocus += TextBoxRefresh;

			this.LostFocus += TextBoxRefresh;
		}

		#region "EventHandlers"

		private void TextBoxRefresh(object sender, EventArgs e)
		{
			this.Refresh();
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 15) {
				base.WndProc(ref m);

				Graphics g = this.CreateGraphics();

				if (this.TextLength == 0 && !this.ContainsFocus) {
					g.DrawString(this.DefaultText, this.Font, new SolidBrush(this.DefaultColor), 0, 0);
				}

				g.Dispose();
			} else {
				base.WndProc(ref m);
			}
		}


		#endregion

	}
}
