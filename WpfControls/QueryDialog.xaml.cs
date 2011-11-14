/*
 * (c) 2011 ThoughtWorks, Inc.
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ThoughtWorksCoreLib;

namespace WpfControls
{
    /// <summary>
    /// Interaction logic for Query.xaml
    /// </summary>
    public partial class QueryDialog : Window
    {
        private Queries _queries;
        private readonly ResourceDictionary _resources = new ResourceDictionary();

        /// <summary>
        /// Creates a new Query Window
        /// </summary>
        /// <param name="queries">Queries object</param>
        public QueryDialog(Queries queries)
        {
            _queries = queries;
            InitializeComponent();
            _resources.Add("Cancel", "Cancel");
            _resources.Add("Instructions", "Type in an MQL query and click Run Queries");
            _resources.Add("RemoveQuery", "Remove Query");
            _resources.Add("RunQueries", "Run");
            _resources.Add("SaveQueries", "Save");
            _resources.Add("WindowTitle", "Run Queries");
            Draw();
        }

        /// <summary>
        /// Capture MQL entered by the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonQueryClick(object sender, RoutedEventArgs e)
        {
            _queries.Save();
            _queries.Fetch();
            Close();
        }

        /// <summary>
        /// Cancel the query
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Called when the "+" (add query) button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonAddQueryClick(object sender, RoutedEventArgs e)
        {
            // We are handing focus to the Run Queries button here to 
            // prevent the bitmap from "flashing" on the Add Query button
            // if it is left with focus. This is a work-around
            buttonQuery.Focus();
            _queries.AddQuery();
            Draw();
        }

        /// <summary>
        /// Create the query dialog for this sheet
        /// </summary>
        /// <remarks>We only want to include queries for the ActiveSheet</remarks>
        private void Draw()
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Drawing the window.");
            foreach (StackPanel e in panelQueries.Children)
                e.Children.RemoveRange(0, e.Children.Count);

            panelQueries.Children.RemoveRange(0, panelQueries.Children.Count);

            _queries.EachQuery(q => 
                panelQueries.Children.Add(CreateQueryStackPanel(q)));

            buttonCancel.Content = _resources["Cancel"];
            buttonQuery.Content = _resources["RunQueries"];
            buttonSaveQueries.Content = _resources["SaveQueries"];
            this.Title = (string)_resources["WindowTitle"];
        }

        /// <summary>
        /// Create a TextBox for an Query object
        /// </summary>
        /// <param name="q">Query object</param>
        /// <returns>TextBox</returns>
        private StackPanel CreateQueryStackPanel(Query q)
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Creating a StackPanel for " + q.Name);
            var queryControlsBuilder = new QueryControlsBuilder(new QueryTemplate(this), q);

            var panelQuery = queryControlsBuilder.CreatePanel();

            var toolBar = queryControlsBuilder.CreateToolBar();

            toolBar.Children.Add(queryControlsBuilder.CreateLabel());
            toolBar.Children.Add(queryControlsBuilder.CreateNameBox(OnNameBoxTextChanged));
            toolBar.Children.Add(queryControlsBuilder.CreateProjectList(_queries.ProjectNames, OnProjectListComboBoxSelectionChanged));
            toolBar.Children.Add(queryControlsBuilder.CreateQueryDelete(OnButtonDeleteQueryClick));

            panelQuery.Children.Add(toolBar);
            panelQuery.Children.Add(queryControlsBuilder.CreateQueryTextBox(OnTextQueryTextChanged));

            panelQueries.Orientation = Orientation.Vertical;

            return panelQuery;
        }

        public class QueryTemplate
        {
            private readonly QueryDialog _queryDialog;

            public QueryTemplate(QueryDialog queryDialog)
            {
                _queryDialog = queryDialog;
            }

            public TextBox CreateNameBox(string text, string name)
            {
                return new TextBox
                           {
                               Width = _queryDialog.textBoxName.Width,
                               Height = _queryDialog.textBoxName.Height,
                               FontSize = _queryDialog.textBoxName.FontSize,
                               Margin = _queryDialog.textBoxName.Margin,
                               Text = text,
                               Name = name
                           };
            }

            public ComboBox CreateProjectList(string name, string selectedItem, IEnumerable<string> itemsSource)
            {
                return new ComboBox
                           {
                               Name = name,
                               Height = _queryDialog.projectListTemplate.Height,
                               Width = _queryDialog.projectListTemplate.Width,
                               FontSize = _queryDialog.projectListTemplate.FontSize,
                               SelectedItem = selectedItem,
                               ItemsSource = itemsSource,
                           };
            }

            public Button CreateDeleteButton(string name)
            {
                return new Button
                           {
                               Name = name,
                               Height = _queryDialog.queryDeleteButtonTemplate.Height,
                               Margin = _queryDialog.queryDeleteButtonTemplate.Margin,
                               FontSize = _queryDialog.queryDeleteButtonTemplate.FontSize,
                               Content = _queryDialog._resources["RemoveQuery"]
                           };
            }

            public TextBox CreateQueryTextBox(string text, string name)
            {
                return new TextBox
                           {
                               Text = text,
                               Name = name,
                               Background = _queryDialog.textQuery.Background,
                               TextWrapping = _queryDialog.textQuery.TextWrapping,
                               Height = _queryDialog.textQuery.Height,
                               Width = _queryDialog.textQuery.Width,
                               VerticalScrollBarVisibility = _queryDialog.textQuery.VerticalScrollBarVisibility,
                               FontSize = _queryDialog.textQuery.FontSize,
                               AcceptsReturn = _queryDialog.textQuery.AcceptsReturn,
                               AutoWordSelection = _queryDialog.textQuery.AutoWordSelection,
                               AcceptsTab = _queryDialog.textQuery.AcceptsTab,
                               Margin = _queryDialog.textQuery.Margin,
                               VerticalAlignment = _queryDialog.textQuery.VerticalAlignment,
                               MinWidth = _queryDialog.textQuery.MinWidth
                           };
            }

            public StackPanel CreateToolBar(string name)
            {
                return new StackPanel
                           {
                               Name = name,
                               Orientation = _queryDialog.toolBarTemplate.Orientation,
                               Height = _queryDialog.toolBarTemplate.Height
                           };
            }

            public StackPanel CreatePanel(string name)
            {
                return new StackPanel
                           {
                               Name = name,
                               Orientation = _queryDialog.panelQueryTemplate.Orientation,
                               HorizontalAlignment = _queryDialog.panelQueryTemplate.HorizontalAlignment,
                               Margin = _queryDialog.panelQueryTemplate.Margin
                           };
            }

            public TextBlock CreateLabel()
            {
                return new TextBlock
                           {
                               Width = _queryDialog.queryNameLabelTemplate.Width,
                               Height = _queryDialog.queryNameLabelTemplate.Height,
                               Text = _queryDialog.queryNameLabelTemplate.Text,
                               FontSize = _queryDialog.queryNameLabelTemplate.FontSize,
                           };
            }
        }

        private void OnNameBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = ((TextBox) sender);
            Debug.Assert(_queries != null, "_queries != null");
            _queries.ChangeName(textBox.Name, textBox.Text);
        }

        /// <summary>
        /// Called each time the query TextBox changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextQueryTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = ((TextBox)sender);
            Debug.Assert(_queries != null, "_queries != null");
            _queries.ChangeQuery(textBox.Name, textBox.Text);
        }

        private void OnButtonDeleteQueryClick(object sender, RoutedEventArgs e)
        {
            // Do some type checking so guarantee we do not throw an exception
            if (sender.GetType() != typeof(Button)) return;
            _queries.Remove(((Button)sender).Name);
            Draw();
        }

        /// <summary>
        /// Called when Save button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonSaveQueriesClick(object sender, RoutedEventArgs e)
        {
            _queries.Save();
        }

        private void OnProjectListComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender.GetType() != typeof(ComboBox)) return;
            var c = (ComboBox) sender;
            _queries.ChangeProject(c.Name, c.SelectedItem.ToString());
        }

        public class QueryControlsBuilder
        {
            private readonly QueryTemplate _template;
            private readonly Query _query;

            public QueryControlsBuilder(QueryTemplate template, Query query)
            {
                _template = template;
                _query = query;
            }

            public TextBox CreateNameBox(TextChangedEventHandler onTextChanged)
            {
                var nameBox = _template.CreateNameBox(_query.Name, _query.NameBoxName);
                nameBox.TextChanged += onTextChanged;
                return nameBox;
            }

            public ComboBox CreateProjectList(IEnumerable<string> projects, SelectionChangedEventHandler onSelectionChanged)
            {
                var projectList = _template.CreateProjectList(_query.ProjectsName, _query.Project.Name, projects);
                projectList.SelectionChanged += onSelectionChanged;
                return projectList;
            }

            public Button CreateQueryDelete(RoutedEventHandler onClick)
            {
                var queryDelete = _template.CreateDeleteButton(_query.DeleteButtonName);
                queryDelete.Click += onClick;
                return queryDelete;
            }

            public TextBox CreateQueryTextBox(TextChangedEventHandler onTextChanged)
            {
                var textBox = _template.CreateQueryTextBox(_query.Value, _query.TextBoxName);
                textBox.TextChanged += onTextChanged;
                return textBox;
            }

            public StackPanel CreateToolBar()
            {
                return _template.CreateToolBar(_query.ControlName("Tools"));
            }

            public StackPanel CreatePanel()
            {
                return _template.CreatePanel(_query.ControlName("Panel"));
            }

            public TextBlock CreateLabel()
            {
                return _template.CreateLabel();
            }
        }
    }
}
