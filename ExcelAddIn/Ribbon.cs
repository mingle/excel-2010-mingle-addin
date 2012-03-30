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
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Office.Tools.Ribbon;
using ThoughtWorksCoreLib;
using WpfControls;

namespace ExcelAddIn
{
    public partial class Ribbon : IAmARibbon
    {
        /// <summary>
        /// Instantiated MingleOffice object that wrape netapi
        /// </summary>
        private MingleOffice _mingle;
        private RibbonModel _ribbonModel;

        /// <summary>
        /// Ask user for Mingle authenticaion credentials
        /// </summary>
        /// <param name="model"></param>
        /// <param name="action"></param>
        public void AskUserForLoginDetails(LoginWindow.LoginDetails model, Action<LoginWindow.LoginDetails> action)
        {
            var login = new LoginWindow(model) { Owner = (Window)Container };
            login.AskUserForDetails(action);            
        }

        /// <summary>
        /// Show the query window
        /// </summary>
        /// <param name="queries"></param>
        public void ShowQueryWindow(Queries queries)
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "I see there are " + queries.Count() + " queries here.");
            var query = new QueryDialog(queries);
            query.ShowDialog();
        }

        /// <summary>
        /// Set the ribbonModel
        /// </summary>
        /// <param name="ribbonModel"></param>
        public void SetModel(RibbonModel ribbonModel)
        {
            _ribbonModel = ribbonModel;
        }

        /// <summary>
        /// Enable the FetchCards button
        /// </summary>
        public void EnableFetchQueryButton()
        {
            buttonFetchCards.Enabled = true;
        }

        /// <summary>
        /// Disable the FetchCards button
        /// </summary>
        public void DisableFetchQueryButton()
        {
            buttonFetchCards.Enabled = false;
        }

        /// <summary>
        /// Display a message to the user
        /// </summary>
        /// <param name="message"></param>
        public void AlertUser(string message)
        {
            MessageBox.Show(message);
        }

        /// <summary>
        /// Called when the Login button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonLoginClick(object sender, RibbonControlEventArgs e)
        {
            try
            {
                _mingle.Login();
            }
            catch (Exception ex)
            {
                AlertUser(ex.Message);
            }
        }

        /// <summary>
        /// Called when the Fetch Cards button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonFetchCardsClick(object sender, RibbonControlEventArgs e)
        {
            TraceLog.WriteLine(new StackFrame().GetMethod().Name, "Clicked FetchCards button");
            _ribbonModel.OpenFetchCards();
        }

        /// <summary>
        /// Called after the ribbon is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRibbonLoad(object sender, RibbonUIEventArgs e)
        {
            _mingle = new MingleOffice(this);
            try
            {
                _mingle.LoadProjects();
            }
            catch (Exception ex)
            {
                AlertUser(ex.Message);
                TraceLog.Exception(new StackFrame().GetMethod().Name, ex);
            }
        }
    }
}
