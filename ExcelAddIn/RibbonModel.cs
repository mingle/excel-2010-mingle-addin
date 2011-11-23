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
using System.Diagnostics;
using System.Linq;
using ThoughtWorksCoreLib;
using WpfControls;

namespace ExcelAddIn
{
    /// <summary>
    /// RobbonModel class
    /// </summary>
    public class RibbonModel
    {
        private readonly Dictionary<string, Queries> _sheets = new Dictionary<string, Queries>();
        private readonly string _defaultQuery;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultQuery"></param>
        public RibbonModel (string defaultQuery)
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "constructed.");
            Projects = new List<Project>();
            _defaultQuery = defaultQuery;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Project> Projects { get; set; }

        /// <summary>
        /// Click fetch cards Action
        /// </summary>
        public Action OpenFetchCards { get; set; }

        /// <summary>
        /// Run all he queries in order
        /// </summary>
        public Action Run { get; set; }

        /// <summary>
        /// Save all the queries
        /// </summary>
        public Action Save { get; set; }

        /// <summary>
        /// Enable the FetchCards button
        /// </summary>
        public Action EnableFetchButton { get; set; }

        /// <summary>
        /// Dusable the FetchCards button
        /// </summary>
        public Action DisableFetchButton { get; set; }

        /// <summary>
        /// Return the queries
        /// </summary>
        public IEnumerable<Query> Queries
        {
            get { return _sheets.SelectMany(x => x.Value); }
        }

        /// <summary>
        /// Set selected project
        /// </summary>
        /// <param name="name"></param>
        public void SetSelectedProject(String name)
        {
            Projects.Find(p => p.HasName(name));
        }

        /// <summary>
        /// Performs the action on each project
        /// </summary>
        /// <param name="action"></param>
        public void EachProject(Action<Project> action)
        {
            Projects.ForEach(action);
        }

        /// <summary>
        /// Refresh the project list 
        /// </summary>
        /// <param name="projects"></param>
        public void ReplaceProjects(List<Project> projects)
        {
            Projects = projects;
        }

        /// <summary>
        /// Returns queries for a particular worksheet
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public Queries QueriesForSheet(string sheet)
        {
            if(!_sheets.ContainsKey(sheet))
            {
                var queries = new Queries(sheet, Projects, _defaultQuery);
                queries.Fetch += Run;
                queries.Save += Save;
                _sheets[sheet] = queries;
            }

            return _sheets[sheet];              
        }

        /// <summary>
        /// Clear the Sheets collection
        /// </summary>
        public void ClearSheets()
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Clearing " + _sheets.Count + " sheets.");
            _sheets.Clear();
        }
    }
}