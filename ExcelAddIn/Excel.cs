/*
   Copyright 2011 ThoughtWorks, Inc.

   Licensed under the Apache License, Version 2.0 (the "License"); 
   you may not use this file except in compliance with the License. 
   You may obtain a copy of the License at:

   http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software 
   distributed under the License is distributed on an "AS IS" BASIS, 
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
   See the License for the specific language governing permissions and 
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using Action = System.Action;

namespace ExcelAddIn
{
    /// <summary>
    /// Excel interface
    /// </summary>
    public interface IAmExcel
    {
        /// <summary>
        /// Performs action when workbook is opened
        /// </summary>
        /// <param name="action"></param>
        void OnWorkbookOpened(Action action);
        /// <summary>
        /// Returns the name of the ActiveSheet
        /// </summary>
        string GetNameActiveSheet();
        /// <summary>
        /// Pupolates sheet with sheet
        /// </summary>
        /// <param name="sheet"></param>
        void PopulateSheetWith(Sheet sheet);
        /// <summary>
        /// Persists custom properties to the workbook
        /// </summary>
        /// <param name="properties"></param>
        void SaveProperties(IEnumerable<ExcelProperty> properties);
        /// <summary>
        /// Returns customer properties
        /// </summary>
        IEnumerable<ExcelProperty> Properties { get; }
    }

    /// <summary>
    /// Excel class
    /// </summary>
    public class Excel : IAmExcel
    {
        private readonly ActiveSheet _activeSheet;
        private Action _workbookOpened;
        private readonly WorksheetProperties _worksheetProperties;

        /// <summary>
        /// Constructs an Excel application
        /// </summary>
        public Excel()
        {
            _activeSheet = new ActiveSheet();
            Globals.ThisAddIn.Application.WorkbookActivate += (wb) => _workbookOpened();
            _worksheetProperties = new WorksheetProperties(this);
        }

        /// <summary>
        /// Returns the name of the ActiveSheet
        /// </summary>
        /// <returns></returns>
        public string GetNameActiveSheet()
        {
            return _activeSheet.Name;
        }

        /// <summary>
        /// Invoked after the workbook is open
        /// </summary>
        /// <param name="action"></param>
        public void OnWorkbookOpened(Action action)
        {
            _workbookOpened += action;
        }

        /// <summary>
        /// Pupulate sheet with a sheet
        /// </summary>
        /// <param name="sheet"></param>
        public void PopulateSheetWith(Sheet sheet)
        {
            _activeSheet.Clear();
            sheet.Populate(_activeSheet);
        }

        /// <summary>
        /// Save properties to Excel customer properties in the workbook
        /// </summary>
        /// <param name="properties"></param>
        public void SaveProperties(IEnumerable<ExcelProperty> properties)
        {
            _worksheetProperties.Save(properties);
        }

        /// <summary>
        /// Return the properties collection
        /// </summary>
        public IEnumerable<ExcelProperty> Properties
        {
            get { return _worksheetProperties; }
        }

        /// <summary>
        /// Interface supporting a worksheet
        /// </summary>
        public interface IAmAWorksheet
        {
            /// <summary>
            /// Get a cell at particular row/colum coordinates
            /// </summary>
            /// <param name="rowIndex"></param>
            /// <param name="columnIndex"></param>
            /// <returns></returns>
            dynamic Cell(int rowIndex, int columnIndex);

            /// <summary>
            /// Clear the sheet
            /// </summary>
            void Clear();
        }

        /// <summary>
        /// Worksheet object
        /// </summary>
        public class WorkSheet : IAmAWorksheet
        {
            private readonly dynamic _worksheet;

            /// <summary>
            /// Constructs a new WorkSheet
            /// </summary>
            /// <param name="worksheet"></param>
            public WorkSheet(dynamic worksheet)
            {
                _worksheet = worksheet;
            }

            /// <summary>
            /// Clears the UsedRange of a WorkSheet
            /// </summary>
            public void Clear()
            {
                _worksheet.UsedRange.Clear();
            }

            /// <summary>
            /// Returns one cell from a WorkSheet
            /// </summary>
            /// <param name="rowIndex"></param>
            /// <param name="columnIndex"></param>
            /// <returns></returns>
            public dynamic Cell(int rowIndex, int columnIndex)
            {
                return _worksheet.Cells(rowIndex, columnIndex);
            }

            /// <summary>
            /// Tells the WorkSheet to be very hidden
            /// </summary>
            public void BeVeryHidden()
            {
                _worksheet.Visible = XlSheetVisibility.xlSheetVeryHidden;
            }

            /// <summary>
            /// Gets and Sets the CalculationType
            /// </summary>
            public int CalculationType
            {
                get
                {
                    return _worksheet.Application.Calculation;
                }
                set
                {
                    if (Enum.IsDefined(typeof(XlCalculation), value))
                    {
                        _worksheet.Application.Calculation = value;
                    }
                    else
                    {
                        throw new ArgumentException("Invalid value for CalculationType. Allowed values -4135,-4105 and 2");
                    }
                }
            }
        }

        /// <summary>
        /// Class for the ActiveSheet
        /// </summary>
        public class ActiveSheet : IAmAWorksheet
        {
            /// <summary>
            /// Clear the sheet
            /// </summary>
            public void Clear()
            {
                GetActiveSheet().UsedRange.Clear();
            }

            /// <summary>
            /// Get a cell at particular row/colum coordinates
            /// </summary>
            /// <param name="rowIndex"></param>
            /// <param name="columnIndex"></param>
            /// <returns></returns>
            public dynamic Cell(int rowIndex, int columnIndex)
            {
                return GetActiveSheet().Cells(rowIndex, columnIndex);
            }

            /// <summary>
            /// Returns the name of the ActiveSheet
            /// </summary>
            public string Name { get { return ((Worksheet) GetActiveSheet()).Name; } }

            private static dynamic GetActiveSheet()
            {
                return Globals.ThisAddIn.Application.ActiveSheet;
            }
        }

        /// <summary>
        /// Creates a WorkSheet
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WorkSheet CreateSheet(string name)
        {
            Worksheet newSheet = ActiveWorkbook.Sheets.Add(ActiveWorkbook.Sheets[1], Type.Missing, Type.Missing, Type.Missing);
            newSheet.Name = name;
            return new WorkSheet(newSheet);
        }

        /// <summary>
        /// Returns a WorkSheet by name
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public WorkSheet GetWorksheet(string sheetName)
        {
            return (from Worksheet sheet in ActiveWorkbook.Sheets
                    where sheet.Name.Equals(sheetName)
                    select new Excel.WorkSheet(sheet)).FirstOrDefault();
        }

        private static Workbook ActiveWorkbook
        {
            get { return Globals.ThisAddIn.Application.ActiveWorkbook; }
        }
    }

    /// <summary>
    /// ExcelProperty - used to save queries as custom properties in a workbook
    /// </summary>
    public class ExcelProperty
    {
        private readonly string _name;
        private readonly string _value;

        /// <summary>
        /// Constructs an ExcelProperty
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ExcelProperty(string name, string value)
        {
            _name = string.IsNullOrEmpty(name) ? string.Empty : name;
            _value = string.IsNullOrEmpty(value) ? string.Empty : value;
        }

        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Value of the property
        /// </summary>
        public string Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Returns a name:value pair 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "\"" + Name + "\" : \"" + Value + "\"";
        }
    }
}