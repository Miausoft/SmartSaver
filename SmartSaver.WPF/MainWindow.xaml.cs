using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartSaver.Domain.Services;
using SmartSaver.WPF;
using System.Collections.Specialized;
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.Domain.Services.EmailServices;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAuthenticationService _auth;
        private User _user;

        public MainWindow(IAuthenticationService auth)
        {
            InitializeComponent();
            _auth = auth;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) //REGISTER launch button
        {
            RegisterWindow registerW = new RegisterWindow(_auth);
            registerW.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            statusTab.IsEnabled = true;
            historyTab.IsEnabled = true;
            savingPlansTab.IsEnabled = true;
            entriesTab.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e) // LOG IN button
        {
            // _mailer.SendEmail("Povilasleka@gmail.com", "Test", "Hello"); Komanda siuncia email vartotojui i pasta.
            if (_auth.Login(usernameTextbox.Text, passwordTextbox.Password) != null)
            {
                _user = _auth.Login(usernameTextbox.Text, passwordTextbox.Password); // returning the user for database if data matches
                // Enable navigation tabs
                statusTab.IsEnabled = true;
                historyTab.IsEnabled = true;
                savingPlansTab.IsEnabled = true;
                entriesTab.IsEnabled = true;
            }
            else
                MessageBox.Show("Invalid data. Try again."); // If user with such credentials doesn't exist
        }
    }
}
