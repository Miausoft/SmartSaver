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
        
        public List<Category> categoryList = new List<Category>()
        {
            new Category(){ Id = 0, Title = "Accomodation"},
            new Category(){ Id = 1, Title = "Food"},
            new Category(){ Id = 2, Title = "Cloting"},
            new Category(){ Id = 3, Title = "Fun"},
            new Category(){ Id = 4, Title = "Partying"},
            new Category(){ Id = 5, Title = "Other"},
        };

        public MainWindow(IAuthenticationService auth)
        {
            InitializeComponent();
            _auth = auth;
            categoryBox.ItemsSource = categoryList.Select(s => s.Title);
            categoryBox.IsEnabled = false;

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
            accountTab.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e) // LOG IN button
        {
            // _mailer.SendEmail("Povilasleka@gmail.com", "Test", "Hello"); Komanda siuncia email vartotojui i pasta.
            if (_auth.Login(usernameTextbox.Text, passwordTextbox.Password) != null)
            {
                _user = _auth.Login(usernameTextbox.Text, passwordTextbox.Password); // returning the user for database if data matches
                // Enable navigation tabs
                statusTab.IsSelected = true;
                statusTab.IsEnabled = true;
                historyTab.IsEnabled = true;
                savingPlansTab.IsEnabled = true;
                entriesTab.IsEnabled = true;
                accountTab.IsEnabled = true;
                logInTab.IsEnabled = false;

                // Initializing components used after login
                

            }
            else
                MessageBox.Show("Invalid data. Try again."); // If user with such credentials doesn't exist
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) // Add transaction
        {
            int selectedIndex = categoryBox.SelectedIndex;
            Object selectedItem = categoryBox.SelectedItem;

            if((bool)(Double.Parse(amountBox.Text) > 0 &
                (selectedIndex != -1 & spendingsCheckBox.IsChecked) | selectedIndex == -1 & earningsCheckBox.IsChecked))
            {

                _user.Account.Transactions.Add(new Transaction() // Creating a new transaction !!!!!
                {
                    Amount = double.Parse(amountBox.Text),
                    ActionTime = DateTime.Now,
                    CategoryId = selectedIndex,
                    Category = new Category() { Id = selectedIndex, Title = (string)selectedItem }
                });

                MessageBox.Show("Transaction added!");

                // Cleaning fields
                amountBox.Text = "0.00";
                categoryBox.SelectedItem = null;
                earningsCheckBox.IsChecked = false;
                spendingsCheckBox.IsChecked = false;

            } else
            {
                MessageBox.Show("The transaction information was entered incorrectly");
            }
               


        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) // Spendings chackbox
        {
            categoryBox.IsEnabled = true;
            earningsCheckBox.IsChecked = false;
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e) // Earnings chackbox
        {
            categoryBox.IsEnabled = false;
            categoryBox.SelectedItem = null;
            spendingsCheckBox.IsChecked = false;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e) // Account information submission
        {
            if(goalBox.Text != null)
                _user.Account.Goal = double.Parse(goalBox.Text);

            _user.Account.GoalStartDate = DateTime.Now;

            if (goalDateBox.Text != null)
                _user.Account.GoalEndDate = (DateTime)goalDateBox.SelectedDate;

            MessageBox.Show("Account details updated");
        }
    }
}
