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
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ThoughtWorksCoreLib;
using ThoughtWorksMingleLib;
using WpfControls;
using Action = System.Action;
using Settings = ExcelAddIn.Properties.Settings;
using Resources = ExcelAddIn.Properties.Resources;

namespace ExcelAddIn
{
    /// <summary>
    /// Domain model for the Excel add-in
    /// </summary>
    public class MingleOffice
    {
        private readonly RibbonModel _ribbonModel = new RibbonModel(Settings.Default.DefaultQuery);

        private readonly ITalkToMingle _mingleProject;
        private readonly IAmExcel _excel;
        private readonly IAmARibbon _ribbon;

        /// <summary>
        /// Interface to this instance's interface to Excel
        /// </summary>
        public IAmExcel Excel { get { return _excel; } }

        /// <summary>
        /// Constructs a new MingleOffice
        /// </summary>
        /// <param name="mingleProject"></param>
        /// <param name="excel"></param>
        /// <param name="ribbon"></param>
        public MingleOffice(ITalkToMingle mingleProject, IAmExcel excel, IAmARibbon ribbon)
        {
            Debug.Assert(mingleProject != null, "mingleProject = null");
            Debug.Assert(excel != null, "excel = null");
            Debug.Assert(ribbon != null, "ribbon = null");
            TraceLog.Initialize("ExcelAddIn2010");
            _mingleProject = mingleProject;
            _excel = excel;
            _ribbon = ribbon;
            _ribbon.SetModel(_ribbonModel);

            _ribbonModel.OpenFetchCards += new Action(OpenQueryDialog);
            _ribbonModel.Run += new Action(RunQueries);
            _ribbonModel.Save += new Action(SaveQueriesToExcel);
            _ribbonModel.EnableFetchButton += new Action(() => _ribbon.EnableFetchQueryButton());
            _ribbonModel.DisableFetchButton += new Action(() => _ribbon.DisableFetchQueryButton());
            
            if(_mingleProject.HasLoginDetails)
            {
                _ribbonModel.EnableFetchButton();
            }
            else
            {
                _ribbonModel.DisableFetchButton();
            }
        }

        /// <summary>
        /// Constructs a new MingleOffice
        /// </summary>
        /// <param name="ribbon"></param>
        public MingleOffice(IAmARibbon ribbon) : this(new Mingle(), new Excel(), ribbon) { TraceLog.WriteLine(new StackFrame().GetMethod().Name, "MingleOffice constructed."); }

        /// <summary>
        /// Returns login credentials
        /// </summary>
        public LoginWindow.LoginDetails LoginDetails    
        {
            get { return new LoginWindow.LoginDetails(MingleSettings.Host, MingleSettings.Login, MingleSettings.Password); }
        }

        /// <summary>
        /// Returns a sorted list of project id/name pairs in id order
        /// </summary>
        /// <returns></returns>
        internal SortedList<string, string> FetchProjects()
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Getting the projects list:");
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "\t\t" + MingleSettings.Host);
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "\t\t" + MingleSettings.Login);

            var projects = new MingleServer(MingleSettings.Host, MingleSettings.Login, MingleSettings.SecurePassword).GetProjectList();
            
            projects.ToList().ForEach(p => TraceLog.WriteLine(new StackFrame().GetMethod().Name, "\t\t" + p.Key));
            
            return projects;         
        }

        /// <summary>
        /// Store Mingle login credentials and host name in User Settings
        /// </summary>
        /// <param name="loginDetails"></param>
        public void SetMingleLoginDetails(LoginWindow.LoginDetails loginDetails)
        {
            Debug.Assert(loginDetails != null, "loginDetails = null");
            Debug.Assert(_ribbonModel != null, "_ribbonModel = null");
            try
            {
                _mingleProject.SetLoginDetails(loginDetails.Host, loginDetails.Username, loginDetails.Password);

            }
            catch (Exception ex)
            {
                _ribbon.AlertUser(ex.Message);
                return;
            } 
            _ribbonModel.EnableFetchButton();
        }

        /// <summary>
        /// Load list of projects on a Mingle host into the projects dropdown list
        /// </summary>
        public void LoadProjects()
        {
            if (string.IsNullOrEmpty(MingleSettings.Host) || string.IsNullOrEmpty(MingleSettings.Login) ||
                string.IsNullOrEmpty(MingleSettings.Password)) return;

            try
            {
                var projects = (from KeyValuePair<string, string> d in FetchProjects()
                                select new Project(d.Value, d.Key)).ToList();
                _ribbonModel.ReplaceProjects(projects);

            }
            catch (Exception ex)
            {
                _ribbon.AlertUser(ex.Message);
                return;
            }
        }

        /// <summary>
        /// Authenticate to Mingle
        /// </summary>
        public void Login()
        {
            try
            {
                _ribbon.AskUserForLoginDetails(LoginDetails,
                                        details =>
                                        {
                                            SetMingleLoginDetails(details);
                                            LoadProjects();
                                        });

            }
            catch (Exception ex)
            {
                _ribbon.AlertUser(ex.Message);
            }
        }

        /// <summary>
        /// Load the queries from custom properties
        /// </summary>
        public void LoadQueries()
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Calling _ribbonModel.ClearSheets()");
            _ribbonModel.ClearSheets();
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Iterating ExcelProperties() and calling AddQuery for each one");
            ExcelProperties().ToList().ForEach(AddQuery);
        }

        private void AddQuery(ExcelQueryProperty p)
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Adding query:\n\r\t\tName = " + p.Name + "\n\r\t\tValue = " + p.Value);
            _ribbonModel.QueriesForSheet(p.Sheet)
                .AddQuery(p.ExtractProject(_ribbonModel.Projects), p.Value, p.QueryName);
        }

        private Queries GetQueriesForActiveSheet()
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Active sheet: " + GetNameActiveSheet());
            return _ribbonModel.QueriesForSheet(GetNameActiveSheet());
        }

        private IEnumerable<ExcelQueryProperty> ExcelProperties()
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Returning all _excelProperties");
            return _excel.Properties.Where(ExcelQueryProperty.Matches)
                .Select(p => new ExcelQueryProperty(p));
        }

        private string GetNameActiveSheet()
        {
            return _excel.GetNameActiveSheet();
        }

        /// <summary>
        /// Open the auery dialog
        /// </summary>
        private void OpenQueryDialog()
        {
            try
            {
                TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Calling LoadQueries()");
                LoadQueries();
                var queries = GetQueriesForActiveSheet();
                TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Found " + queries.Count() + " for active sheet.");
                if (queries.Count() == 0)
                {
                    queries.AddDefaultQuery();
                }

                TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Ready to show the query window.");
                _ribbon.ShowQueryWindow(queries);
            }
            catch (Exception ex)
            {
                TraceLog.WriteLine(new StackFrame().GetMethod().Name, ex.Message);
                _ribbon.AlertUser(ex.Message);
            }
        }

        /// <summary>
        /// Run the queries
        /// </summary>
        private void RunQueries()
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Running queries");
            try
            {
                var cards = new List<XElement>();
                GetQueriesForActiveSheet().EachQuery(
                    q =>
                    {
                        // Guard against an empty MQL statement on the form
                        var rawResults = _mingleProject.ExecMql(q.Project.Id, q.Value);
                        if (null == rawResults) return;
                        TraceLog.WriteLine(new StackFrame().GetMethod().Name, "\n\r\t\t" + q.Value);
                        var results = rawResults.Descendants("result").Select(e => e);
                        results.ToList().ForEach(r => r.AddFirst(new XElement("query", q.Name)));
                        cards.AddRange(results);
                    });
                _excel.PopulateSheetWith(BuildSheetFrom(cards));
                SaveQueriesToExcel();
            }
            catch (Exception ex)
            {
                var message = (string.Format(CultureInfo.InstalledUICulture, "{0}", ex.Message));

                if (ex.Message.ToLower().Contains("(422) unprocessable entity"))
                { 
                    message = string.Format(CultureInfo.InstalledUICulture,"{0}\n\n\r{1}", 
                                                    message, Resources.Html422Elaboration);
                    TraceLog.WriteLine(new StackFrame().GetMethod().Name, message);
                }

                _ribbon.AlertUser(message);
            }
        }

        /// <summary>
        /// Persist queries as Excel custom properties so they are saved with the workbook
        /// </summary>
        private void SaveQueriesToExcel()
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Saving queries");
            var properties = _ribbonModel.Queries.Select(q => new ExcelQueryProperty(q));
            _excel.SaveProperties(properties.ToList());
        }

        /// <summary>
        /// Loads cards into a new Sheet and returns the Sheet object
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private static Sheet BuildSheetFrom(IEnumerable<XElement> cards)
        {
            Debug.Assert(cards != null, "cards = null");

            cards = cards.ToList();

            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "building sheet from " + cards.Count() + " cards.");

            var sheet = new Sheet();

            var row = sheet.AddRow();
            
            // Add a title row with column names. This could be made optional.
            // Avoid potential performance issue with double enumeration by enumerating a list
            foreach (var x in cards.First().Descendants())
            {
                row.AddColumn().Value = x.Name.LocalName;
            }

            foreach (var card in cards)
            {
                AddRow(sheet.AddRow(), card);
            }

            return sheet;
        }

        /// <summary>
        /// Add a row to the AvtiveWorksheet
        /// </summary>
        /// <param name="row"></param>
        /// <param name="card"></param>
        private static void AddRow(Row row, XContainer card)
        {
            Debug.Assert(row != null, "row = null");
            Debug.Assert(card != null, "card = null");

            foreach (var e in card.Elements().Select(a => a))
            {
                row.AddColumn().Value = e.Value;
            }
        }
    }

    /// <summary>
    /// Models an class to handle queryies as ExcelPropety objects
    /// </summary>
    public class ExcelQueryProperty : ExcelProperty
    {
        private const string SEPERATOR = "::";

        /// <summary>
        /// Constructs ExcelQueryProperty
        /// </summary>
        /// <param name="query"></param>
        public ExcelQueryProperty(Query query) : this(query.SheetName, query.Name, query.Project.Id, query.Value) {}

        /// <summary>
        /// Constructs ExcelQueryProperty
        /// </summary>
        /// <param name="id"></param>
        /// <param name="query"></param>
        /// <param name="sheetName"></param>
        /// <param name="name"></param>
        public ExcelQueryProperty(string sheetName, string name, string id, string query) : base(ToName(sheetName, id, name), query) { }

        /// <summary>
        /// Constructs ExcelQueryProperty
        /// </summary>
        public ExcelQueryProperty(ExcelProperty property) : base(property.Name, property.Value) { }

        /// <summary>
        /// Retrievs the query name
        /// </summary>
        public string QueryName
        {
            get { return Split()[3]; }
        }

        /// <summary>
        /// Retrieves the sheet name
        /// </summary>
        public string Sheet
        {
            get { return Split()[1]; }
        }

        /// <summary>
        /// Retrieves the project name
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public Project ExtractProject(List<Project> projects)
        {

            return projects.First(p => p.HasId(Split()[2]));
        }

        /// <summary>
        /// Matches the sheet name
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public bool IsForSheet(string sheet)
        {
            return Sheet.Equals(sheet);
        }

        private string[] Split()
        {
            return Name.Split(new string[] {SEPERATOR}, StringSplitOptions.None);
        }

        private static string ToName(string sheetName, string id, string name)
        {
            return Prefix + SEPERATOR + sheetName + SEPERATOR + id + SEPERATOR + name;
        }

        /// <summary>
        /// Matches an ExcelProperty
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool Matches(ExcelProperty p)
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Matching excel property" + p.Name + "with" + Prefix + SEPERATOR);
            return p.Name.StartsWith(Prefix + SEPERATOR);
        }

        private static string Prefix
        {
            get { return Settings.Default.QueryPrefix; }
        }
    }

    /// <summary>
    /// Class to model Mingle for purposes of using the execute_mql API
    /// </summary>
    class Mingle : ITalkToMingle
    {

        private MingleServer _mingle;

        /// <summary>
        /// Constructs a new Mingle using a given project
        /// </summary>
        public Mingle()
        {
        }


        /// <summary>
        /// Calls the execute_mql API
        /// </summary>
        /// <param name="project"></param>
        /// <param name="mql"></param>
        /// <returns></returns>
        public XDocument ExecMql(string project, string mql)
        {
            return string.IsNullOrEmpty(mql) ? null : _mingle.GetProject(project).ExecMql(mql);
        }

        /// <summary>
        /// Sets the login credentials and creates a MingleServer object
        /// </summary>
        /// <param name="mingleHost"></param>
        /// <param name="mingleUser"></param>
        /// <param name="minglePassword"></param>
        public void SetLoginDetails(string mingleHost, string mingleUser, string minglePassword)
        {
            MingleSettings.Host = mingleHost;
            MingleSettings.Login = mingleUser;
            MingleSettings.Password = minglePassword;
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "\n\r\t\t" + MingleSettings.Host + "\n\r\t\t" + MingleSettings.Login);
            _mingle = new MingleServer(mingleHost, mingleUser, MingleSettings.SecurePassword);
        }

        /// <summary>
        /// True if login credentials exist
        /// </summary>
        public bool HasLoginDetails
        {
            get
            {
                return !String.IsNullOrWhiteSpace(MingleSettings.Host) &&
                !String.IsNullOrWhiteSpace(MingleSettings.Login) &&
                !String.IsNullOrWhiteSpace(MingleSettings.Password);
            }
        }
    }

    /// <summary>
    /// Interface for chatting with Mingle
    /// </summary>
    public interface ITalkToMingle
    {
        /// <summary>
        /// Call execute_mql on the Mingle API
        /// </summary>
        /// <param name="project"></param>
        /// <param name="mql"></param>
        /// <returns></returns>
        XDocument ExecMql(string project, string mql);
        
        /// <summary>
        /// Sets login credentials
        /// </summary>
        /// <param name="mingleHost"></param>
        /// <param name="mingleUser"></param>
        /// <param name="minglePassword"></param>
        void SetLoginDetails(string mingleHost, string mingleUser, string minglePassword);

        /// <summary>
        /// True if login credentials exist
        /// </summary>
        Boolean HasLoginDetails { get; }
    }
}
