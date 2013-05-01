using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Drawing.Printing;
using Lski.Txt;
using System.Drawing;

namespace Lski.Windows.Forms.Printing
{

	public abstract class PrintDoc
	{

		public abstract void PrintPage(object sender, PrintPageEventArgs e);

		/// <summary>
		/// The default font for any text, can be changed
		/// </summary>
		/// <remarks></remarks>

		protected Font _defaultFont;
		protected Header _pageHeader;

		protected Footer _pageFooter;
		/// <summary>
		/// Holds the drawing area of the page, which is a combination of the MarginBounds of the PrintDocument offest with the header
		/// and footer sections. Only if they exist!
		/// </summary>
		/// <remarks></remarks>

		protected Rectangle _drawingArea;
		/// <summary>
		/// The print doucment used to print the item (and fire events)
		/// </summary>
		/// <remarks></remarks>
		private System.Drawing.Printing.PrintDocument withEventsField__printDoc;
		protected System.Drawing.Printing.PrintDocument _printDoc {
			get { return withEventsField__printDoc; }
			set {
				if (withEventsField__printDoc != null) {
					withEventsField__printDoc.BeginPrint -= _printDoc_BeginPrint;
					withEventsField__printDoc.EndPrint -= _printDoc_EndPrint;
					withEventsField__printDoc.PrintPage -= _printDoc_PrintPage;
				}
				withEventsField__printDoc = value;
				if (withEventsField__printDoc != null) {
					withEventsField__printDoc.BeginPrint += _printDoc_BeginPrint;
					withEventsField__printDoc.EndPrint += _printDoc_EndPrint;
					withEventsField__printDoc.PrintPage += _printDoc_PrintPage;
				}
			}

		}
		///''''''''''''''''''''
		//' Helper variables for functions
		///''''''''''''''''''''
		protected Int32 _currentPageNo = 0;

		protected Int32 _totalNumOfPages = 0;
		public PrintDoc(PrintDocument printDoc)
		{
			Init(printDoc, null, null);
		}

		public PrintDoc(PrintDocument printDoc, Header header)
		{
			Init(printDoc, header, null);
		}

		public PrintDoc(PrintDocument printDoc, Footer footer)
		{
			Init(printDoc, null, footer);
		}

		public PrintDoc(PrintDocument printDoc, Header header, Footer footer)
		{
			Init(printDoc, header, footer);
		}

		protected void Init(PrintDocument printDoc, Header header, Footer footer)
		{
			_printDoc = printDoc;
			_pageHeader = header;
			_pageFooter = footer;
		}

		public PrintDocument PrintDocument {
			get { return _printDoc; }
			set { _printDoc = value; }
		}

		/// <summary>
		/// Holds the position of the margin bounds AFTER headings and footers have been applied
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public Rectangle DrawingArea {
			get { return _drawingArea; }
			set { _drawingArea = value; }
		}

		public Header PageHeader {
			get { return _pageHeader; }
			set { _pageHeader = value; }
		}

		public Footer PageFooter {
			get { return _pageFooter; }
			set { _pageFooter = value; }
		}

		public Int32 TotalNumberOfPages {
			get { return _totalNumOfPages; }
		}

		/// <summary>
		/// Used by subclasses to assign variables prior to pages being printed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		public virtual void BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			// Stub
		}

		/// <summary>
		/// Used by subclasses to free up variables after all of the pages has been printed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		public virtual void EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			// Stub
		}


		public static string ConvertToString(object obj) {

			// If null or DBNull then return an empty string
			if (obj == null || obj is DBNull)
				return string.Empty;

			// If is a string, simply cast
			if (obj is string)
				return (string)obj;

			// If an object 
			return obj.ToString();

		}


		/// <summary>
		/// Sets up some of the class variables for the print cycle
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>

		private void _printDoc_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			// Reset the page and pointer information
			//_isNewPage = True
			_currentPageNo = 0;

			BeginPrint(sender, e);

		}

		/// <summary>
		/// Handles the End print event by calling the method EndPrint()
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>

		private void _printDoc_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			EndPrint(sender, e);

			//_isNewPage = True
			_currentPageNo = 0;

		}

		private void _printDoc_PrintPage(System.Object sender, System.Drawing.Printing.PrintPageEventArgs e) {

			// If this is the first page then work out the size of the header and footers and subtract them from the MarginBounds
			// to create the drawing area.

			if (_currentPageNo == 0) {
				// Get static refs
				var boundsAfterOrientation = e.PageBounds;
				var margins = e.PageSettings.Margins;

				// Calculate the different sides to the drawing area, after orientation has been applied, but before header and footer are
				// calculated
				var x = boundsAfterOrientation.X + margins.Left;
				var y = boundsAfterOrientation.Y + margins.Top;
				var width = boundsAfterOrientation.Width - margins.Right - margins.Left;
				var height = boundsAfterOrientation.Height - margins.Bottom - margins.Top;

				// Get the drawing areas for the header and footers
				// AND alter the drawing area so that fits between the header and footer

				if ((PageHeader != null)) {
					PageHeader.DrawingArea = PrvCalculateHeaderRectangle(e, x, y, width, height);

					// Add the header height to the top to offset it
					y += PageHeader.DrawingArea.Height;

				}

				if ((PageFooter != null)) {
					PageFooter.DrawingArea = PrvCalculateFooterRectangle(e, x, y, width, height);

					// Remove the footer height from the bottom to offset it
					height -= PageFooter.DrawingArea.Height;

				}

				this.DrawingArea = new Rectangle(x, y, width, height);

			}

			PrintPage(sender, e);

			// For each page draw the footer and header last (hopefully by this point the total number of pages will have been worked out
			PrvDrawHeader(e, this.PageHeader);
			PrvDrawFooter(e, this.PageFooter, _currentPageNo, _totalNumOfPages);

		}

		private Rectangle PrvCalculateHeaderRectangle(System.Drawing.Printing.PrintPageEventArgs e, Int32 x, Int32 y, Int32 width, Int32 height)
		{

			SizeF textSize = default(SizeF);
			Int32 sectionWidth = default(Int32);

			Int32 largestHeight = 0;
			Int32 currentHeight = 0;

			try {
				sectionWidth = Convert.ToInt32(Math.Floor((double)width / 3));
			} catch (DivideByZeroException) {
				sectionWidth = 0;
			}


			if (string.IsNullOrEmpty(PageHeader.Right.String)) {

				// get the size of the text when fitted into the section
				textSize = e.Graphics.MeasureString(PageHeader.ParsedRight.String, PageHeader.Right.Font, sectionWidth);

				// Get the height plus the padding for the text
				currentHeight = Convert.ToInt32(textSize.Height + (PageHeader.Padding * 2));

				if (currentHeight > largestHeight) largestHeight = currentHeight;
			}

			// Left Align

			if (string.IsNullOrEmpty(PageHeader.Left.String)) {

				// get the size of the text when fitted into the section
				textSize = e.Graphics.MeasureString(PageHeader.ParsedLeft.String, PageHeader.Left.Font, sectionWidth);

				// Get the height plus the padding for the text
				currentHeight = Convert.ToInt32(textSize.Height + (PageHeader.Padding * 2));
				
				if (currentHeight > largestHeight) largestHeight = currentHeight;
			}

			// Center
			if (string.IsNullOrEmpty(PageHeader.Centre.String)) {

				// get the size of the text when fitted into the section
				textSize = e.Graphics.MeasureString(PageHeader.ParsedCentre.String, PageHeader.Centre.Font, sectionWidth);

				// Get the height plus the padding for the text
				currentHeight = Convert.ToInt32(textSize.Height + (PageHeader.Padding * 2));
				
				if (currentHeight > largestHeight) largestHeight = currentHeight;
			}

			return new Rectangle(x, y, width, largestHeight);

		}

		private Rectangle PrvCalculateFooterRectangle(System.Drawing.Printing.PrintPageEventArgs e, Int32 x, Int32 y, Int32 width, Int32 height)
		{

			SizeF textSize = default(SizeF);
			Int32 sectionWidth = default(Int32);
			Int32 largestHeight = 0;
			Int32 currentHeight = 0;

			try {
				sectionWidth = (int)Math.Floor(width / 3d);
			} catch (DivideByZeroException) {
				sectionWidth = 0;
			}


			if (string.IsNullOrEmpty(PageFooter.Right.String)) {
				// get the size of the text when fitted into the section
				textSize = e.Graphics.MeasureString(PageFooter.ParsedRight.String, PageFooter.Right.Font, sectionWidth);

				// Get the height plus the padding for the text
				currentHeight = Convert.ToInt32(textSize.Height + (PageFooter.Padding * 2));
				if (currentHeight > largestHeight)
					largestHeight = currentHeight;

			}

			// Left Align

			if (string.IsNullOrEmpty(PageFooter.Left.String)) {
				// get the size of the text when fitted into the section
				textSize = e.Graphics.MeasureString(PageFooter.ParsedLeft.String, PageFooter.Left.Font, sectionWidth);

				// Get the height plus the padding for the text
				currentHeight = Convert.ToInt32(textSize.Height + (PageFooter.Padding * 2));
				if (currentHeight > largestHeight)
					largestHeight = currentHeight;

			}

			// Center
			if (string.IsNullOrEmpty(PageFooter.Centre.String)) {
				// get the size of the text when fitted into the section
				textSize = e.Graphics.MeasureString(PageFooter.ParsedCentre.String, PageFooter.Centre.Font, sectionWidth);

				// Get the height plus the padding for the text
				currentHeight = Convert.ToInt32(textSize.Height + (PageFooter.Padding * 2));
				if (currentHeight > largestHeight)
					largestHeight = currentHeight;

			}

			return new Rectangle(x, (y + height - largestHeight), width, largestHeight);

		}


		private void PrvDrawHeader(System.Drawing.Printing.PrintPageEventArgs e, Header pageHeader)
		{
			// If there is no header then simply return without drawing anything
			if (pageHeader == null)
				return;

			// Used to hold the size of the text
			SizeF textSize = default(SizeF);
			Int32 sectionWidth = default(Int32);

			// The text for each section
			string text = null;


			try {
				sectionWidth = (int)Math.Floor(pageHeader.DrawingArea.Width / 3d);
			} catch (DivideByZeroException) {
				sectionWidth = 0;
			}


			if (string.IsNullOrEmpty(pageHeader.Right.String)) {
				text = pageHeader.ParsedRight.String;

				// get the size of the text when fitted into the section
				textSize = e.Graphics.MeasureString(text, pageHeader.Right.Font, sectionWidth);

				// The section is on the right hand side of the page = totalWidth minus that section (at the top of the page = 0)
				var a = DrawingArea;

				e.Graphics.DrawString(text, pageHeader.Right.Font, pageHeader.Right.Brush, new RectangleF(pageHeader.DrawingArea.Left + (sectionWidth * 2), pageHeader.DrawingArea.Top, sectionWidth, textSize.Height), pageHeader.Right.Format);
			}

			// Left Align

			if (string.IsNullOrEmpty(pageHeader.Left.String)) {
				text = pageHeader.ParsedLeft.String;

				// get the size of the text when fitted into the section
				textSize = e.Graphics.MeasureString(text, pageHeader.Left.Font, sectionWidth);

				// The section is on the left hand side of the page = 0 (at the top of the page = 0)
				e.Graphics.DrawString(text, pageHeader.Left.Font, pageHeader.Left.Brush, new RectangleF(pageHeader.DrawingArea.Left, pageHeader.DrawingArea.Top, sectionWidth, textSize.Height), pageHeader.Left.Format);
			}

			// Center

			if (string.IsNullOrEmpty(pageHeader.Centre.String)) {
				text = pageHeader.ParsedCentre.String;

				// get the size of the text when fitted into the section
				textSize = e.Graphics.MeasureString(text, pageHeader.Centre.Font, sectionWidth);
				e.Graphics.DrawString(text, pageHeader.Centre.Font, pageHeader.Centre.Brush, new RectangleF((pageHeader.DrawingArea.Left + sectionWidth), pageHeader.DrawingArea.Top, sectionWidth, pageHeader.DrawingArea.Height), pageHeader.Centre.Format);
			}

		}


		private void PrvDrawFooter(System.Drawing.Printing.PrintPageEventArgs e, Footer pageFooter, Int32 pageNumber, Int32 numberOfPages)
		{
			// If footer not set simply exit 
			if (pageFooter == null)
				return;

			// Update the footer object with page numbers
			pageFooter.PageNumber = pageNumber;
			pageFooter.TotalNumOfPages = numberOfPages;

			// Used to hold the size of the text
			SizeF textSize = default(SizeF);
			Int32 sectionWidth = default(Int32);

			string text = null;

			try {
				sectionWidth = (int)Math.Floor(pageFooter.DrawingArea.Width / 3d);
			} catch (DivideByZeroException) {
				sectionWidth = 0;
			}


			if (string.IsNullOrEmpty(pageFooter.Right.String)) {
				text = pageFooter.ParsedRight.String;

				// get the size of the text when fitted into the section
				textSize = e.Graphics.MeasureString(text, pageFooter.Right.Font, sectionWidth);

				// The section is on the right hand side of the page = totalWidth minus that section (at the top of the page = 0)
				var a = DrawingArea;

				e.Graphics.DrawString(text, pageFooter.Right.Font, pageFooter.Right.Brush, new RectangleF(pageFooter.DrawingArea.Left + (sectionWidth * 2), pageFooter.DrawingArea.Top, sectionWidth, textSize.Height), pageFooter.Right.Format);
			}

			// Left Align

			if (string.IsNullOrEmpty(pageFooter.Left.String)) {
				text = pageFooter.ParsedLeft.String;

				// get the size of the text when fitted into the section
				textSize = e.Graphics.MeasureString(text, pageFooter.Left.Font, sectionWidth);

				// The section is on the left hand side of the page = 0 (at the top of the page = 0)
				e.Graphics.DrawString(text, pageFooter.Left.Font, pageFooter.Left.Brush, new RectangleF(pageFooter.DrawingArea.Left, pageFooter.DrawingArea.Top, sectionWidth, textSize.Height), pageFooter.Left.Format);
			}

			// Center

			if (string.IsNullOrEmpty(pageFooter.Centre.String)) {
				text = pageFooter.ParsedCentre.String;

				// get the size of the text when fitted into the section
				textSize = e.Graphics.MeasureString(text, pageFooter.Left.Font, sectionWidth);
				e.Graphics.DrawString(text, pageFooter.Centre.Font, pageFooter.Centre.Brush, new RectangleF((pageFooter.DrawingArea.Left + sectionWidth), pageFooter.DrawingArea.Top, sectionWidth, pageFooter.DrawingArea.Height), pageFooter.Centre.Format);
			}

		}


		#region "Header/Footer Classes"

		public class Header
		{

			protected RichString _leftText;
			protected RichString _centreText;

			protected RichString _rightText;

			protected Rectangle _drawingArea;

			protected Int16 _padding;
			public Int32 PageNumber = 1;

			public Int32 TotalNumOfPages = 1;
			public enum Position
			{
				Left,
				Right,
				Centre
			}

			public enum AutoText
			{
				Page,
				Pages,
				PageOfPages,
				LongTime,
				LongDate,
				LongDateTime,
				ShortTime,
				ShortDate,
				ShortDateTime,
				Username
			}


			public Header()
			{
				this.Init();
			}


			public void Init()
			{
				_padding = 3;

				_leftText = new RichString(null);
				_leftText.Format = new StringFormat();
				_leftText.Format.Alignment = StringAlignment.Near;

				_centreText = new RichString(null);
				_centreText.Format = new StringFormat();
				_centreText.Format.Alignment = StringAlignment.Center;

				_rightText = new RichString(null);
				_rightText.Format = new StringFormat();
				_rightText.Format.Alignment = StringAlignment.Far;
			}

			public RichString this[Position pos] {
				get {
					if (pos == Position.Left) return _leftText;
					if (pos == Position.Right) return _rightText;
					if (pos == Position.Centre)	return _centreText;
					return null;
				}
			}

			/// <summary>
			/// Returns the actual text held within the left section of the header/footer, along with the formatting information
			/// (pre-parsed of the auto-text contained)
			/// </summary>
			/// <value></value>
			/// <returns></returns>
			/// <remarks></remarks>
			public RichString ParsedLeft {
				get { return new RichString(ParseStringForAutoText(_leftText.String), _leftText.Font, _leftText.Format, _leftText.Brush); }
			}

			/// <summary>
			/// Returns the actual text held within the right section of the header/footer, along with the formatting information
			/// (pre-parsed of the auto-text contained)
			/// </summary>
			/// <value></value>
			/// <returns></returns>
			/// <remarks></remarks>
			public RichString ParsedRight {
				get { return new RichString(ParseStringForAutoText(_rightText.String), _rightText.Font, _rightText.Format, _rightText.Brush); }
			}

			/// <summary>
			/// Returns the actual text held within the right section of the header/footer, along with the formatting information
			/// (pre-parsed of the auto-text contained)
			/// </summary>
			/// <value></value>
			/// <returns></returns>
			/// <remarks></remarks>
			public RichString ParsedCentre {
				get { return new RichString(ParseStringForAutoText(_centreText.String), _centreText.Font, _centreText.Format, _centreText.Brush); }
			}


			/// <summary>
			/// Returns the actual text held within the left section of the header/footer, along with the formatting information
			/// (pre-parsed of the auto-text contained)
			/// </summary>
			/// <value></value>
			/// <returns></returns>
			/// <remarks></remarks>
			public RichString Left {

				get { return _leftText; }
			}

			/// <summary>
			/// Returns the actual text held within the right section of the header/footer, along with the formatting information
			/// (pre-parsed of the auto-text contained)
			/// </summary>
			/// <value></value>
			/// <returns></returns>
			/// <remarks></remarks>
			public RichString Right {
				get { return _rightText; }
			}

			/// <summary>
			/// Returns the text contained in RichString
			/// </summary>
			/// <value></value>
			/// <returns></returns>
			/// <remarks></remarks>
			public RichString Centre {
				get { return _centreText; }
			}

			public Rectangle DrawingArea {
				get { return _drawingArea; }
				set { _drawingArea = value; }
			}

			public Int16 Padding {
				get { return _padding; }
				set { _padding = value; }
			}

			public virtual string CreateAutoText(AutoText a)
			{

				switch (a) {

					case AutoText.Username:

						return "~un~";
					case AutoText.Page:

						return "~#~";
					case AutoText.Pages:

						return "~##~";
					case AutoText.PageOfPages:

						return "~#~ of ~##~";
					case AutoText.LongTime:

						return "~LT~";
					case AutoText.LongDate:

						return "~LD~";
					case AutoText.LongDateTime:

						return "~LD~ ~LT~";
					case AutoText.ShortTime:

						return "~ST~";
					case AutoText.ShortDateTime:

						return "~SD~ ~ST~";
					case AutoText.ShortDate:

						return "~SD~";
				}

				return null;

			}

			private string ParseStringForAutoText(string textToParse)
			{

				System.Text.StringBuilder tmp = new System.Text.StringBuilder(textToParse);

				tmp.Replace("~un~", Environment.UserName);
				tmp.Replace("~#~", ConvertToString(PageNumber));
				tmp.Replace("~##~", ConvertToString(TotalNumOfPages));
				tmp.Replace("~LT~", System.DateTime.Now.ToLongTimeString());
				tmp.Replace("~LD~", System.DateTime.Now.ToLongDateString());
				tmp.Replace("~SD~", System.DateTime.Now.ToShortDateString());
				tmp.Replace("~ST~", System.DateTime.Now.ToShortTimeString());

				return tmp.ToString();

			}

		}

		public class Footer : Header
		{

			public Footer() : base()
			{
			}

			public override string CreateAutoText(AutoText a)
			{
				return base.CreateAutoText(a);
			}

		}

		//Public Class Footer
		//    Inherits Header

		//    Public PageNumber As Int32 = 1
		//    Public TotalNumOfPages As Int32 = 1

		//    'Public Shadows Enum Position
		//    '    Left
		//    '    Right
		//    '    Centre
		//    'End Enum

		//    Public Enum AutoText
		//        Page
		//        Pages
		//        PageOfPages
		//        LongTime
		//        LongDate
		//        LongDateTime
		//        ShortTime
		//        ShortDate
		//        ShortDateTime
		//        Username
		//    End Enum

		//    Public Sub New()
		//        MyBase.Init()
		//    End Sub

		//    Public Sub AddAutoText(ByVal text As AutoText, ByVal position As Position)

		//        If (position = Footer.Position.Centre) Then _centreText.String = AddAutoText(text)
		//        If (position = Footer.Position.Left) Then _leftText.String = AddAutoText(text)
		//        If (position = Footer.Position.Right) Then _rightText.String = AddAutoText(text)

		//    End Sub

		//    Private Function AddAutoText(ByVal a As AutoText) As String

		//        Select Case a

		//            Case AutoText.Username
		//                Return "~un~"

		//            Case AutoText.Page
		//                Return "~#~"

		//            Case AutoText.Pages
		//                Return "~##~"

		//            Case AutoText.PageOfPages
		//                Return "~#~ of ~##~"

		//            Case AutoText.LongTime
		//                Return "~LT~"

		//            Case AutoText.LongDate
		//                Return "~LD~"

		//            Case AutoText.LongDateTime
		//                Return "~LD~ ~LT~"

		//            Case AutoText.ShortTime
		//                Return "~ST~"

		//            Case AutoText.ShortDateTime
		//                Return "~SD~ ~ST~"

		//            Case AutoText.ShortDate
		//                Return "~SD~"

		//        End Select

		//        Return Nothing

		//    End Function

		//    Private Function ParseStringForAutoText(ByVal textToParse As String) As String

		//        textToParse.Replace("~un~", My.User.Name)
		//        textToParse.Replace("~#~", PageNumber)
		//        textToParse.Replace("~##~", Me.TotalNumOfPages)
		//        textToParse.Replace("~LT~", Date.Now.ToLongTimeString)
		//        textToParse.Replace("~LD~", Date.Now.ToLongDateString)
		//        textToParse.Replace("~SD~", Date.Now.ToShortDateString)
		//        textToParse.Replace("~ST~", Date.Now.ToShortTimeString)

		//        Return textToParse

		//    End Function

		//End Class

		#endregion

	}

}

