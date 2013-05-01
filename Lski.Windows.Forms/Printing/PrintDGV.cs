using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Lski.Txt;
//using Microsoft.VisualBasic.Conversion;

namespace Lski.Windows.Forms.Printing
{

	public class PrintDGV : PrintDoc
	{

		protected System.Windows.Forms.DataGridView _dataGridView;
		protected Color _headerBGColor = System.Drawing.Color.LightGray;
		protected bool _includeDGVheader = true;

		protected bool _selectedRowsOnly = false;
		///''''''''''''''''''''
		//' Helper variables for functions
		///''''''''''''''''''''

		// Holds the rows that user wants to display, stored as member so avoid constant recalculation

		private List<DataGridViewRow> _rowsToDisplay;
		// Holds the column numbers that fit on each page (each page is represented by a new list)

		private List<List<Int32>> _horizontalPageInfo;
		private Int32 _currentHorizontalPage;

		private const Int32 DGVPadding = 5;
		private StringFormat _stringFormat;

		private StringFormat _stringFormatComboBox;
		private Button _button;
		private CheckBox _chkBox;

		private ComboBox _cboBox;
		// The total width of the dataGridView
		private Int32 _currentRowPos;

		private Int32 _pageStartRowPos;

		private Int32 _numOfRowsPerPage;
		// Stores the meta information about each of the columns in the DataGridView required for printing the values within

		private List<ColumnData> _columnsData = new List<ColumnData>();
		public PrintDGV(PrintDocument printDoc) : base(printDoc)
		{
		}

		public PrintDGV(PrintDocument printDoc, Header header) : base(printDoc, header)
		{
		}

		public PrintDGV(PrintDocument printDoc, Footer footer) : base(printDoc, footer)
		{
		}

		public PrintDGV(PrintDocument printDoc, Header header, Footer footer) : base(printDoc, header, footer)
		{
		}

		public DataGridView DataGridView {
			get { return _dataGridView; }
			set { _dataGridView = value; }
		}

		public bool SelectedRowsOnly {
			get { return _selectedRowsOnly; }
			set { _selectedRowsOnly = value; }
		}

		public bool PrintHeader {
			get { return _includeDGVheader; }
			set { _includeDGVheader = value; }
		}

		public Color HeaderBackgroudColor {
			get { return _headerBGColor; }
			set { _headerBGColor = value; }
		}


		public override void BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			_stringFormat = new StringFormat();
			_stringFormat.LineAlignment = StringAlignment.Center;
			_stringFormat.Trimming = StringTrimming.EllipsisCharacter;

			_stringFormatComboBox = new StringFormat();
			_stringFormatComboBox.LineAlignment = StringAlignment.Center;
			_stringFormatComboBox.FormatFlags = StringFormatFlags.NoWrap;
			_stringFormatComboBox.Trimming = StringTrimming.EllipsisCharacter;

			// Create a couple of object that will facilitate drawing different types of cell onto the printDocument
			_button = new Button();
			_chkBox = new CheckBox();
			_cboBox = new ComboBox();

			// Reset the pointer information
			_currentRowPos = 0;
			_pageStartRowPos = 0;

			_rowsToDisplay = new List<DataGridViewRow>();

			_horizontalPageInfo = new List<List<Int32>>();
			_currentHorizontalPage = 0;

		}


		public override void EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			_stringFormat = null;
			_stringFormatComboBox = null;

			_button = null;
			_chkBox = null;
			_cboBox = null;

			_columnsData.Clear();

			_rowsToDisplay = null;
			_horizontalPageInfo = null;

		}


		public override void PrintPage(System.Object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			// If there is no datagridview object associated with this object exit, and mark no more pages
			if (_dataGridView == null) {
				e.HasMorePages = false;
				return;
			}

			// Reference holders
			DataGridViewCell cell = null;
			DataGridViewRow row = null;
			DataGridViewColumn col = null;

			// Holds the height of the current row
			Int32 rowHeight = default(Int32);

			// Current positions
			var topPosition = DrawingArea.Top;
			var leftPosition = DrawingArea.Left;

			DataGridViewContentAlignment textAlignment = default(DataGridViewContentAlignment);
			string val = "";

			// The page number needs to go up as a new page will start
			_currentPageNo += 1;

			// If this is the first page work out the meta data about the columns in the DataGridView and widths
			// Required here instead of in beginPrint because it needs access to the Graphics object created 

			if (_currentPageNo == 1) {
				List<Int32> columnsOnPage = null;
				Int32 totalWidth = 0;
				DataGridViewSelectedRowCollection selectedRows = null;

				// Collect the rows to display as single type of collection (without the need for casting)

				if (SelectedRowsOnly) {
					selectedRows = _dataGridView.SelectedRows;
					
					for (Int32 i = 0, n = selectedRows.Count; i < n; i++) { //Was (<= selectRows.Count -1)
						_rowsToDisplay.Add(selectedRows[i]);
					}


				} else {
					Int32 loopUBound = default(Int32);

					// If the datagridview adds blanks rows at the bottom for users to add there own values, remove
					if ((_dataGridView.AllowUserToAddRows)) {
						loopUBound = _dataGridView.Rows.Count - 2;
					} else {
						loopUBound = _dataGridView.Rows.Count - 1;
					}

					for (Int32 i = 0, n = loopUBound; i < n; i++) {
						_rowsToDisplay.Add(_dataGridView.Rows[i]);
					}

				}
				
				columnsOnPage = new List<Int32>();

				// Loop through each of the columns store information about each column and the pages that they fit on

				for (Int32 i = 0 , n = _dataGridView.Columns.Count; i < n; i++) { // TODO: Check this is right as was i <= (_dataGridView.Columns.Count - 1)
					
					col = _dataGridView.Columns[i];

					// 1. If the totalWidth is greater than the width of the drawing area (or )
					// then add the list of columns to the list of columns on each page
					// 2. If the last column in the set of columns then just add the columnsOnPage to _horizontalPage info

					if (((totalWidth + col.Width) > DrawingArea.Width)) {
						_horizontalPageInfo.Add(columnsOnPage);

						// Make a new list
						columnsOnPage = new List<Int32>();

						// Reset the width and left position
						totalWidth = 0;
						leftPosition = DrawingArea.Left;

					}

					// Add a new column to the column meta data list
					_columnsData.Add(new ColumnData(leftPosition, col.Width, col.GetType()));

					// Increment the left edge, with the width of the added column
					leftPosition += col.Width;

					// Increment the size of the dataGridViewObject
					totalWidth += col.Width;

					// Add column Index
					columnsOnPage.Add(col.Index);

					// If this is the last column, but there are columns not added, then add them
					if ((i == _dataGridView.ColumnCount - 1) && columnsOnPage.Count > 0) {
						_horizontalPageInfo.Add(columnsOnPage);
					}

				}

			}

			// Now the columns information has been 
			// If a new page then draw columns, if desired
			// Reset the top and the loop counter
			//topPosition = DrawingArea.Top

			// If the user wants to include the header from the table on each page then do so
			if (PrintHeader) {
				PrintHeaderToPage(DataGridView, e, topPosition, _dataGridView.ColumnHeadersHeight);

				// Only drop the topPosition if a header is added
				topPosition += _dataGridView.ColumnHeadersHeight;
			}

			// Loop through each row of the _dataGridView, however use a do while, because _currentRowPos might not start at 0
			// but the value it is when the page starts

			while (_currentRowPos < _rowsToDisplay.Count) {
				// Get a reference to the current row object
				row = _rowsToDisplay[_currentRowPos];

				//rowHeight = e.Graphics.MeasureString(row.Height, col.InheritedStyle.Font, nWidth).Height + 11
				rowHeight = row.Height;

				// If at the limit of page (top has already been recalulated, 
				// change settings to accomodate a new page, then the current row on that page instead
				if ((topPosition + rowHeight) >= (DrawingArea.Height + DrawingArea.Top)) break;

				// loop through each of the columns for this horizontal page and draw it to the print document
				foreach (int i in _horizontalPageInfo[_currentHorizontalPage]) {

					// get reference to the cell
					cell = row.Cells[i];
					// Get a ref to the padding object of the cell
					var padding = cell.Style.Padding;

					// 1. If a type that simply contains text then simply draw the text in that box
					// 2. If a button type then use a button object to draw it to the printDocument
					// 3. If a checkbox type then use a checkbox object to draw it to the print document
					// 4. If a combobox type then use a combobox object to draw it to the print document
					// 5. If a image type then simpy draw it to the print document

					if ((object.ReferenceEquals(_columnsData[i].Type, typeof(DataGridViewTextBoxColumn))) || (object.ReferenceEquals(_columnsData[i].Type, typeof(DataGridViewLinkColumn)))) {
						textAlignment = cell.Style.Alignment;

						if (textAlignment == DataGridViewContentAlignment.BottomRight || textAlignment == DataGridViewContentAlignment.MiddleRight || textAlignment == DataGridViewContentAlignment.TopRight) {
							_stringFormat.Alignment = StringAlignment.Far;

						} else if (textAlignment == DataGridViewContentAlignment.BottomCenter || textAlignment == DataGridViewContentAlignment.MiddleCenter || textAlignment == DataGridViewContentAlignment.TopCenter) {
							_stringFormat.Alignment = StringAlignment.Center;
						} else {
							_stringFormat.Alignment = StringAlignment.Near;
						}


						if ((cell.Value != null)) {
							val = (cell.FormattedValue).ToString();

							e.Graphics.DrawString(val, cell.InheritedStyle.Font, new SolidBrush(cell.InheritedStyle.ForeColor), new RectangleF(_columnsData[i].Left + padding.Left, topPosition + padding.Top, _columnsData[i].Width - padding.Horizontal, rowHeight - padding.Vertical), _stringFormat);
						}



					} else if ((object.ReferenceEquals(_columnsData[i].Type, typeof(DataGridViewButtonColumn)))) {
						// Setup up the button object with the same visual style as the dataGridView button
						_button.Text = (cell.Value).ToString();
						_button.Size = new Size(_columnsData[i].Width, rowHeight);

						// Create a bitmap of the button to draw to the print document object (same size as cell)
						Bitmap bitmap = new Bitmap(_button.Width, _button.Height);
						_button.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

						// Draw bitmap to the print document
						e.Graphics.DrawImage(bitmap, new Point(_columnsData[i].Left, topPosition));


					} else if ((object.ReferenceEquals(_columnsData[i].Type, typeof(DataGridViewCheckBoxColumn)))) {
						// Setup up the checkbox object with the same visual style as the dataGridView checkbox
						_chkBox.Size = new Size(14, 14);
						_chkBox.Checked = (bool)cell.Value;

						// Create a bitmap of the checkbox to draw to the print document object (same size as cell)
						Bitmap bitmap = new Bitmap(_columnsData[i].Width, rowHeight);

						//???????? Fill the box on the checkbox
						//???????? why fromImage rather than New Bitmap???
						Graphics tmpGraphics = Graphics.FromImage(bitmap);
						tmpGraphics.FillRectangle(Brushes.White, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

						// Draw the checkbox as a bitmap (use CType to truncate the double returned AND create the value of type Int32 required)
						_chkBox.DrawToBitmap(bitmap, new Rectangle((Int32)(bitmap.Width - _chkBox.Width) / 2, (Int32)(bitmap.Height - _chkBox.Height) / 2, _chkBox.Width, _chkBox.Height));

						// Draw bitmap to the print document
						e.Graphics.DrawImage(bitmap, new Point(_columnsData[i].Left, topPosition));


					} else if ((object.ReferenceEquals(_columnsData[i].Type, typeof(DataGridViewComboBoxColumn)))) {
						textAlignment = cell.InheritedStyle.Alignment;

						if (textAlignment == DataGridViewContentAlignment.BottomRight || textAlignment == DataGridViewContentAlignment.MiddleRight || textAlignment == DataGridViewContentAlignment.TopRight) {
							_stringFormatComboBox.Alignment = StringAlignment.Far;

						} else if (textAlignment == DataGridViewContentAlignment.BottomCenter || textAlignment == DataGridViewContentAlignment.MiddleCenter || textAlignment == DataGridViewContentAlignment.TopCenter) {
							_stringFormatComboBox.Alignment = StringAlignment.Center;
						} else {
							_stringFormatComboBox.Alignment = StringAlignment.Near;
						}

						// Setup up the combobox object with the same visual style as the dataGridView combobox
						_cboBox.Size = new Size(_columnsData[i].Width, rowHeight);

						// Create a bitmap of the combobox to draw to the print document object (same size as cell)
						Bitmap bitmap = new Bitmap(_cboBox.Width, _cboBox.Height);
						_cboBox.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

						// Draw the combobox display
						e.Graphics.DrawImage(bitmap, new Point(_columnsData[i].Left, topPosition));
						// Draw to text of the cell to the text visible in the combo cell, not added to combobox because
						// a combobox handles selected values differently to DataGridViewComboBoxColumn
						e.Graphics.DrawString(ConvertToString(cell.Value), cell.InheritedStyle.Font, new SolidBrush(cell.InheritedStyle.ForeColor), new RectangleF(_columnsData[i].Left + 1, topPosition, _columnsData[i].Width - 16, rowHeight), _stringFormatComboBox);


					} else if ((object.ReferenceEquals(_columnsData[i].Type, typeof(DataGridViewImageColumn)))) {
						if ((cell.Value != null)) {

							Image image = (Image)cell.Value;

							Rectangle cellSize = new Rectangle(_columnsData[i].Left, topPosition, _columnsData[i].Width, rowHeight);
							Size oImageSize = image.Size;

							// Fit the image into the size of the cell
							e.Graphics.DrawImage(image, new Rectangle(_columnsData[i].Left + (Int32)((cellSize.Width - oImageSize.Width) / 2), topPosition + (Int32)((cellSize.Height - oImageSize.Height) / 2), image.Width, image.Height));
						}


					}

					// Draw the box to go round the cell
					e.Graphics.DrawRectangle(new Pen(this._dataGridView.GridColor), new Rectangle(_columnsData[i].Left, topPosition, _columnsData[i].Width, rowHeight));


				}

				topPosition += rowHeight;

				// Increment the currentRow count
				_currentRowPos += 1;

				// If this is the first page continue to work out the number of rows per page for the footer to be able to add page numbers
				if (_currentPageNo == 1)
					_numOfRowsPerPage += 1;

			}


			if (_currentPageNo == 1) {
				// Handle any divide by zero error
				if (_numOfRowsPerPage == 0 || _rowsToDisplay.Count == 0)
					_totalNumOfPages = 1;
				else
					_totalNumOfPages = Convert.ToInt16(Math.Ceiling((double)_rowsToDisplay.Count / _numOfRowsPerPage) * _horizontalPageInfo.Count);
			}

			// 1. If there are still horizontal pages to print then change settings to print that too
			// 2. If there are still records to print then change settings to print them too
			// 3. Simply exit and tell the printDoc that there is no more pages to print

			if ((_horizontalPageInfo.Count - 1) > _currentHorizontalPage) {
				// Reset the position of currentRowPos so it will print from the correct row
				_currentRowPos = _pageStartRowPos;

				// Increment the horizontal page
				_currentHorizontalPage += 1;

				// As it requires a new page, set the flag to true for when it reloads
				//_isNewPage = True

				// Ensures recall of the method
				e.HasMorePages = true;


			} else if ((_currentRowPos < _rowsToDisplay.Count - 1)) {
				_pageStartRowPos = _currentRowPos;

				// Reset the horizontal page number
				_currentHorizontalPage = 0;

				// As it requires a new page, set the flag to true for when it reloads
				//_isNewPage = True
				// The page number needs to go up as a new page will statr
				//_currentPageNo += 1
				// Ensures recall of the method
				e.HasMorePages = true;

			} else {
				// If there there are no more pages to be run, stop recall of this method
				e.HasMorePages = false;
			}

		}

		/// <summary>
		/// Prints the header of the DataGridView to page where stated
		/// </summary>
		/// <param name="dgv"></param>
		/// <param name="e"></param>
		/// <param name="yPosition"></param>
		/// <param name="rowHeight"></param>
		/// <remarks></remarks>

		private void PrintHeaderToPage(DataGridView dgv, System.Drawing.Printing.PrintPageEventArgs e, Int32 yPosition, Int32 rowHeight)
		{
			DataGridViewColumn col = null;


			foreach (int i in _horizontalPageInfo[_currentHorizontalPage]) {

				// Get reference to the current column
				col = _dataGridView.Columns[i];

				// Fills the header bock with a background colur
				e.Graphics.FillRectangle(new SolidBrush(HeaderBackgroudColor), new Rectangle(_columnsData[i].Left, yPosition, _columnsData[i].Width, rowHeight));

				// Draws the box around the header column cell
				e.Graphics.DrawRectangle(new Pen(this._dataGridView.GridColor), new Rectangle(_columnsData[i].Left, yPosition, _columnsData[i].Width, rowHeight));


				// Draws the text
				e.Graphics.DrawString(col.HeaderText, col.InheritedStyle.Font, new SolidBrush(col.InheritedStyle.ForeColor), new RectangleF(_columnsData[i].Left, yPosition, _columnsData[i].Width, rowHeight), _stringFormat);
			}
		}

		private class ColumnData
		{
			// Stores lefthand position of each cell in the DataGridView
			public Int32 Left;
			// Stores the width of each cell in the DataGridView
			public Int32 Width;
			// Stores the column types of each cell in the DataGridView

			public System.Type Type;

			public ColumnData(Int32 columnLeft, Int32 columnWidth, System.Type columnType)
			{
				Left = columnLeft;
				Width = columnWidth;
				Type = columnType;

			}

		}

	}

}

