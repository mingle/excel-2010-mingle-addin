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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExcelAddIn
{
    /// <summary>
    /// Class used to handle saving of queries to a special hidden worksheet.
    /// </summary>
    public class WorksheetProperties : IEnumerable<ExcelProperty>
    {
        private readonly Excel _excel;

        /// <summary>
        /// Constructs a WorksheetProperties object
        /// </summary>
        /// <param name="excel"></param>
        public WorksheetProperties(Excel excel)
        {
            _excel = excel;
        }

        /// <summary>
        /// Saves Worksheet properties
        /// </summary>
        /// <param name="properties"></param>
        public void Save(IEnumerable<ExcelProperty> properties)
        {
            var queriesSheet = Worksheet;
                                          
            var sheet = PropertiesToSheet(properties);

            queriesSheet.Clear();
            sheet.Populate(queriesSheet);
        }

        /// <summary>
        /// Gets and Sets the Calculation Type
        /// </summary>
        public int CalculationType
        {
            get
            {
                return Worksheet.CalculationType;
            }
            set
            {
                Worksheet.CalculationType = value;
            }
        }

        /// <summary>
        /// Implements IEnumerable GetEnumerator for the list of properties
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ExcelProperty> GetEnumerator()
        {
            return SheetToProperties().GetEnumerator();
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
        /// Retrieves an IEnumerable of properties from a sheet
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ExcelProperty> SheetToProperties()
        {
            return Sheet.FromWorksheet(Worksheet).Select(r => new ExcelProperty(r[0].Value, r[1].Value));
        }

        private Excel.WorkSheet Worksheet
        {
            get
            {
                var worksheet = _excel.GetWorksheet("queries");

                if (worksheet == null)
                {
                    worksheet = _excel.CreateSheet("queries");
                    worksheet.BeVeryHidden();
                }
                return worksheet;
            }
        }

        private static Sheet PropertiesToSheet(IEnumerable<ExcelProperty> properties)
        {
            var sheet = new Sheet();
            foreach (var property in properties)
            {
                var row = sheet.AddRow();
                row.AddColumn().Value = property.Name;
                row.AddColumn().Value = property.Value;
            }
            return sheet;
        }
    }
}