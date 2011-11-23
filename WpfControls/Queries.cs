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
using System.Linq;

namespace WpfControls
{
    public class Queries : IEnumerable<Query>
    {
        private readonly string _sheet;
        private readonly List<Project> _projects;
        private readonly List<Query> _queries = new List<Query>();
        private readonly string _defaultQuery;
        private int _index = 0;

        public Action Fetch { get; set; }

        public Action Save { get; set; }

        public Queries(string sheet, List<Project> projects, string defaultQuery)
        {
            _sheet = sheet;
            _projects = projects;
            _defaultQuery = defaultQuery;
            Fetch += () => {};
        }

        public Query this[int index]
        {
            get { return _queries[index]; }
        }

        public IEnumerable<Project> Projects
        {
            get { return _projects; }
        }

        public IEnumerator<Query> GetEnumerator()
        {
            return _queries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void EachQuery(Action<Query> each)
        {
            _queries.ForEach(each);
        }

        public Query AddDefaultQuery()
        {
            if (Projects.Count() < 1) throw new Exception("There are no available projects in Mingle. This may happen if the user's login name is not authorized for any projects.");
            return AddQuery(Projects.First(), _defaultQuery);
        }

        public Query AddQuery()
        {
            if (Projects.Count() < 1) throw new Exception("There are no available projects in Mingle. This may happen if the user's login name is not authorized for any projects.");
            return AddQuery(Projects.First(), "");
        }

        private Query AddQuery(Project project, string query)
        {
            return AddQuery(project, query, "New query");
        }

        public Query AddQuery(Project project, string query, string name)
        {
            var newQuery = new Query(_index) { Name = name, Value = query, Project = project, SheetName = _sheet };
            _queries.Add(newQuery);
            _index++;
            return newQuery;
        }

        public void Clear()
        {
            _queries.Clear();
        }

        public void Remove(string deleteButtonName)
        {
            _queries.RemoveAll(q => q.DeleteButtonName.Equals(deleteButtonName));
        }

        public string ChangeName(string name, string value)
        {
            return _queries.Find(q => q.NameBoxName.Equals(name)).Name = value;
        }

        public string ChangeQuery(string name, string query)
        {
            return _queries.Find(q => q.TextBoxName.Equals(name)).Value = query;
        }

        public IEnumerable<string> ProjectNames
        {
            get { return Projects.Select(p => p.Name); }
        }

        public Project GetProject(string name)
        {
            return Projects.First(p => p.HasName(name));
        }

        public void ChangeProject(string name, string project)
        {
            _queries.Find(q => q.ProjectsName.Equals(name)).Project = GetProject(project);
        }
    }

    public class Query
    {
        private readonly int _index;

        public Query(int index)
        {
            _index = index;
        }

        public string SheetName { get; set; }

        public Project Project { get; set; }

        public string Value { get; set; }

        public string Name { get; set; }

        public string NameBoxName
        {
            get { return ControlName("textQueryName"); }
        }

        public string TextBoxName
        {
            get { return ControlName("text"); }
        }

        public string DeleteButtonName
        {
            get { return ControlName("deleteButton"); }
        }

        public string ProjectsName
        {
            get { return ControlName("projectList"); }
        }

        public string ControlName(string control)
        {
            return control + "Query" + _index;
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}:{2}", Name, Value, Project);
        }

        public bool IsForSheet(string sheetName)
        {
            return SheetName.Equals(sheetName);
        }
    }
}