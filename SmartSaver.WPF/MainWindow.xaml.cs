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
using SmartSaver.Domain.ExtensionMethods;
using SmartSaver.WPF;
using System.Collections.Specialized;
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAuthenticationService _auth;

        public MainWindow()
        {
            InitializeComponent();
            _auth = new AuthenticationService();

            Account acc = new Account();
            List<Transaction> row = new List<Transaction>
            {
                new Transaction() { Date = DateTime.Now, Amount = 500 }
            };

            listView.ItemsSource = row;
        }


        private void Button_Click_2(object sender, RoutedEventArgs e) //REGISTER launch button
        {
            RegisterWindow registerW = new RegisterWindow();
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
            if (_auth.Login(usernameTextbox.Text, passwordTextbox.Password) != null)
            {
                Account user = _auth.Login(usernameTextbox.Text, passwordTextbox.Password); // returning the user for database if data matches


                // Enable navigation tabs
                LogInTab.IsEnabled = false;
                statusTab.IsEnabled = true;
                historyTab.IsEnabled = true;
                savingPlansTab.IsEnabled = true;
                entriesTab.IsEnabled = true;
                statusTab.IsSelected = true;
            }
            else
                MessageBox.Show("Invalid data. Try again."); // If user with such credentials doesn't exist
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //InitializeComponent();
            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
