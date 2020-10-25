using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SmartSaver.WPF;
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
            UpdateBalanceLabel();
        }

        private void UpdateBalanceLabel()
        {
            decimal v = _context.Transactions.ToList().Sum(x => x.Amount);
            balanceLabel.Content = v.ToString("0.00") + " €";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) // Add transaction
        {
            int selectedIndex = categoryBox.SelectedIndex + 2;
            decimal amount = -decimal.Parse(amountBox.Text);

            if (earningsCheckBox.IsChecked.GetValueOrDefault())
            {
                amount *= -1; // positive amount (income)
                selectedIndex = 1;
            }

            Transaction transaction = new Transaction()
            {
                ActionTime = DateTime.UtcNow,
                Amount = (decimal)amount,
                Account = _user.Account,
                Category = _context.Categories.FirstOrDefault(c => c.Id.Equals(selectedIndex))
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            UpdateHistoryTable();
            UpdateBalanceLabel();

            MessageBox.Show("Transaction added!");
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

            _user.Account.Goal = decimal.Parse(goalBox.Text);
            _user.Account.GoalStartDate = DateTime.UtcNow;
            _user.Account.GoalEndDate = (DateTime)goalDateBox.SelectedDate;
            _context.SaveChanges();
            MessageBox.Show("Account details have been updated");
        }

        private void UpdateHistoryTable()
        {
            List<Transaction> transactions =
                _context.Transactions.Where(t => t.AccountId.Equals(_user.Account.Id)).OrderByDescending(t => t.ActionTime).ToList();

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
            categoryBox.ItemsSource = _context.Categories.ToList().Where(s => s.Id != 1).Select(s => s.Title);
            categoryBox.IsEnabled = false;
        }

        private void sortByDateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateHistoryTable();
        }

        private void sortByCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            List<Transaction> transactions =
                _context.Transactions.Where(t => t.AccountId.Equals(_user.Account.Id)).OrderByDescending(t => t.Category).ToList();

            HistoryTable.ItemsSource = transactions;
        }
    }
}
