using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SmartSaver.Domain.Services.TipManager;

namespace SmartSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAuthenticationService _auth;
        private readonly ApplicationDbContext _context;
        private User _user;

        private WindowObjectCollection<TabItem> _windowObjectColl;

        /// <summary>
        /// Initializes component, injects _auth and _context objects.
        /// Prepares the data 
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="context"></param>
        public MainWindow(IAuthenticationService auth, ApplicationDbContext context)
        {
            InitializeComponent();
            _auth = auth;
            _context = context;

            PrepareData();
            RegisterTabs();
        }

        private void RegisterTabs()
        {
            TabItem[] tabs = new TabItem[]
            {
                statusTab,
                historyTab,
                savingPlansTab,
                entriesTab,
                accountTab,
                logInTab
            };

            _windowObjectColl = new WindowObjectCollection<TabItem>(tabs);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e) //REGISTER launch button
        {
            RegisterWindow registerW = new RegisterWindow(_auth);
            registerW.Show();
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
            UpdateAccountDisplay();
            UpdateHistoryTable();
            UpdateBalanceLabel();
            GenerateTipOfTheDay();
        }

        private void GenerateTipOfTheDay()
        {
            TipOfTheDayLabel.Content = Tips.DayBasedTip();
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
                Amount = amount,
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
            EnableTabs();
            MessageBox.Show("Account details have been updated");
        }

        private void UpdateHistoryTable()
        {
            List<Transaction> transactions =
                _context.Transactions.Where(t => t.AccountId.Equals(_user.Account.Id)).OrderByDescending(t => t.ActionTime).ToList();

            HistoryTable.ItemsSource = transactions;
        }

        /// <summary>
        /// If user is logged in, but account info is not set,
        /// the only active tab is accountTab, in other case
        /// he can use all tabs.
        /// </summary>
        private void EnableTabs()
        {
            if (!_user.Account.IsValid())
            {
                accountTab.IsEnabled = true;
                accountTab.IsSelected = true;
                return;
            }

            statusTab.IsSelected = true;

            foreach (TabItem tab in _windowObjectColl)
            {
                tab.IsEnabled = true;
            }
        }

        /// <summary>
        /// When called, updates Account information in "Account" tab.
        /// </summary>
        private void UpdateAccountDisplay()
        {
            var goal = _user.Account.Goal;
            var date = _user.Account.GoalEndDate;

            if (goal != 0)
            {
                goalBox.Text = $"{_user.Account.Goal}";
            }

            // TODO: Ta pati padaryt su data.
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
