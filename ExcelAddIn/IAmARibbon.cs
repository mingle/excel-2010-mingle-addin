/*
 * (c) 2011 ThoughtWorks, Inc.
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