/*
 * (c) 2011 ThoughtWorks, Inc.
 */

using System;
using System.Windows;

namespace WpfControls
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly LoginDetails _loginDetails;

        public LoginWindow(LoginDetails loginDetails)
        {
            _loginDetails = loginDetails;
            InitializeComponent();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            loginControl.textBoxHostUrl.Text = _loginDetails.Host;
            loginControl.textBoxLoginName.Text = _loginDetails.Username;
            loginControl.textBoxPassword.Password = _loginDetails.Password;
        }

        private void OnButtonConnectClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnButtonCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
                        
        public void AskUserForDetails(Action<LoginDetails> action)
        {
            this.buttonConnect.Click += (e,s) => action(new LoginDetails(loginControl.textBoxHostUrl.Text,
                                                            loginControl.textBoxLoginName.Text,
                                                            loginControl.textBoxPassword.Password));
            ShowDialog();
        }

        public class LoginDetails
        {
            private readonly string _host;
            private readonly string _username;
            private readonly string _password;

            public LoginDetails(string host, string username, string password)
            {
                _host = host;
                _username = username;
                _password = password;
            }

            public string Host
            {
                get { return _host; }
            }

            public string Username
            {
                get { return _username; }
            }

            public string Password
            {
                get { return _password; }
            }
        }
    }
}
