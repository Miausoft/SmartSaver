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
        private ApplicationDbContext _context;

        public MainWindow(IAuthenticationService auth, ApplicationDbContext context)
        {
            InitializeComponent();
            _auth = auth;
            _context = context;

            PrepareData();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) //REGISTER launch button
        {
            RegisterWindow registerW = new RegisterWindow(_auth);
            registerW.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            EnableTabs();
        }

        private void Button_Click(object sender, RoutedEventArgs e) // LOG IN button
        {
            _user = _auth.Login(usernameTextbox.Text, passwordTextbox.Password);
            if (_user == null)
            {
                MessageBox.Show("Invalid data. Try again."); // If user with such credentials doesn't exist
                return;
            }

            EnableTabs();
            UpdateHistoryTable();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) // Add transaction
        {
            int selectedIndex = categoryBox.SelectedIndex;
            object selectedItem = categoryBox.SelectedItem;

            if((bool)(double.Parse(amountBox.Text) > 0 &
                (selectedIndex != -1 & spendingsCheckBox.IsChecked) | selectedIndex == -1 & earningsCheckBox.IsChecked))
            {
                double amount = double.Parse(amountBox.Text);
                int categoryId = selectedIndex + 1;
                if (selectedIndex > 0)
                {
                    amount *= -1;
                }
                else
                {
                    categoryId = 1;
                }

                Transaction transaction = new Transaction()
                {
                    Amount = amount,
                    ActionTime = DateTime.UtcNow,
                    CategoryId = categoryId,
                    AccountId = _user.Account.Id
                };

                _context.Transactions.Add(transaction);

                _context.SaveChanges();

                MessageBox.Show("Transaction added!");
                UpdateHistoryTable();

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
            if (goalDateBox.Text == null) return;
            if (goalDateBox.SelectedDate == null) return;

            _user.Account.Goal = double.Parse(goalBox.Text);
            _user.Account.GoalStartDate = DateTime.UtcNow;
            _user.Account.GoalEndDate = (DateTime)goalDateBox.SelectedDate;
            _context.SaveChanges(); // Eilute įrašo į duomenų bazę.
        }

        private void UpdateHistoryTable()
        {
            List<Transaction> transactions =
                _context.Transactions.Where(t => t.AccountId.Equals(_user.Account.Id)).ToList();

            HistoryTable.ItemsSource = transactions;
        }

        private void EnableTabs()
        {
            statusTab.IsSelected = true;
            statusTab.IsEnabled = true;
            historyTab.IsEnabled = true;
            savingPlansTab.IsEnabled = true;
            entriesTab.IsEnabled = true;
            accountTab.IsEnabled = true;
            logInTab.IsEnabled = false;
        }

        private void PrepareData()
        {
            categoryBox.ItemsSource = _context.Categories.ToList().Select(s => s.Title);
            categoryBox.IsEnabled = false;
        }
    }


}
