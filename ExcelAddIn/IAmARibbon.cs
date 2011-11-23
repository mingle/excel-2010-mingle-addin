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
using WpfControls;

namespace ExcelAddIn
{
    /// <summary>
    /// IAmRibbon interface
    /// </summary>
    public interface IAmARibbon
    {
        /// <summary>
        /// Show a message to the user
        /// </summary>
        /// <param name="message"></param>
        void AlertUser(string message);

        /// <summary>
        /// Ask the user the his/her Mingle credentials
        /// </summary>
        /// <param name="loginDetails"></param>
        /// <param name="action"></param>
        void AskUserForLoginDetails(LoginWindow.LoginDetails loginDetails, Action<LoginWindow.LoginDetails> action);
        /// <summary>
        /// Show the query editor window
        /// </summary>
        /// <param name="queries"></param>
        void ShowQueryWindow(Queries queries);
        /// <summary>
        /// Set the ribbon model
        /// </summary>
        /// <param name="ribbonModel"></param>
        void SetModel(RibbonModel ribbonModel);

        /// <summary>
        /// Enables the fetch query button
        /// </summary>
        void EnableFetchQueryButton();
        /// <summary>
        /// Disables the fetch query button
        /// </summary>
        void DisableFetchQueryButton();
    }
}