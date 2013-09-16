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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ExcelAddIn
{
    /// <summary>
    /// Sheet class
    /// </summary>
    public class Sheet : IEnumerable<Row>
    {
        private readonly List<Row> _rows = new List<Row>();

        /// <summary>
        /// Add a row to a Sheet
        /// </summary>
        /// <returns></returns>
        public Row AddRow()
        {
            var row = new Row(_rows.Count + 1, this);
            _rows.Add(row);
            return row;
        }

        /// <summary>
        /// Populates a worksheet with data
        /// </summary>
        /// <param name="activeSheet"></param>
        public void Populate(Excel.IAmAWorksheet activeSheet)
        {
            foreach (var column in _rows.SelectMany(row => row))
            {
                column.Populate(activeSheet);
            }
        }

        /// <summary>
        /// Returns a row enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Row> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        /// <summary>
        /// Imeplements IEnumerable.GetEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a Sheet from a Worksheet
        /// </summary>
        /// <param name="worksheet"></param>
        /// <returns></returns>
        public static Sheet FromWorksheet(Excel.IAmAWorksheet worksheet)
        {
            var sheet = new Sheet();
            var index = 1;
            while(!String.IsNullOrWhiteSpace((string) (worksheet.Cell(index, 1).Value)))
            {
                var row = sheet.AddRow();
                row.AddColumn().Value = worksheet.Cell(index, 1).Value;
                row.AddColumn().Value = worksheet.Cell(index, 2).Value;
                index++;
            }
            return sheet;
        }
    }

    /// <summary>
    /// Column class
    /// </summary>
    public class Column
    {
        private readonly int _rowIndex;
        private readonly int _index;
        private readonly Row _row;

        /// <summary>
        /// Constructs a column
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="index"></param>
        /// <param name="row"></param>
        public Column(int rowIndex, int index, Row row)
        {
            Debug.Assert(row != null, "row = null");
            _rowIndex = rowIndex;
            _index = index;
            _row = row;
        }

        /// <summary>
        /// Column value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Adds the next column
        /// </summary>
        /// <returns></returns>
        public Column AddNext()
        {
            return _row.AddColumn();
        }

        /// <summary>
        /// Sets the value of a cell in a row
        /// </summary>
        /// <param name="sheet"></param>
        public void Populate(Excel.IAmAWorksheet sheet)
        {
            sheet.Cell(_rowIndex, _index).Value = Value;
        }
    }

    /// <summary>
    /// Row class
    /// </summary>
    public class Row : IEnumerable<Column>
    {
        private readonly int _index;
        private readonly Sheet _sheet;
        private readonly List<Column> _columns = new List<Column>();

        /// <summary>
        /// Constructs a new row
        /// </summary>
        /// <param name="index"></param>
        /// <param name="sheet"></param>
        public Row(int index, Sheet sheet)
        {
            Debug.Assert(sheet != null, "sheet = null");
            _index = index;
            _sheet = sheet;
        }

        /// <summary>
        /// Adds a new row
        /// </summary>
        /// <returns></returns>
        public Row AddNext()
        {
            return _sheet.AddRow();
        }

        /// <summary>
        /// Adds a column
        /// </summary>
        /// <returns></returns>
        public Column AddColumn()
        {
            var column = new Column(_index, _columns.Count + 1, this);
            _columns.Add(column);
            return column;
        }

        /// <summary>
        /// Returns a column enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Column> GetEnumerator()
        {
            return _columns.GetEnumerator();
        }

        /// <summary>
        /// Implements IEnumerable.GetEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a Column nusing an integer
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public Column this[int col]
        {
            get { return _columns[col]; }
        }
    }
}