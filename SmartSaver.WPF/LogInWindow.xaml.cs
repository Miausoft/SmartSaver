using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SmartSaver.Domain.Services.AuthenticationServices;

namespace SmartSaver.WPF
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        private readonly IAuthenticationService _auth;

        public LogInWindow()
        {
            InitializeComponent();
            _auth = new AuthenticationService();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //usernameTextbox.Text;
            //passwordTextbox.Text;

            var user = _auth.Login(usernameTextbox.Text, passwordTextbox.Text);
            // O kaip perduot Account tipo objekta i MainWindow???
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // REGISTER launch button
        {
            RegisterWindow registerW = new RegisterWindow();
            registerW.Show();
        }
    }
}
