using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SmartSaver.WPF;
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Services.SavingMethodSuggestion;
using SmartSaver.Domain.Services.TransactionsCounter;

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
            GenerateSuggestions(_user.Account);
            UpdateHistoryTable();
            UpdateBalanceLabel();
        }

        private void UpdateBalanceLabel()
        {
            decimal v = TransactionsCounter.SavedSum(_user.Account.Transactions, DateTime.MinValue, DateTime.MaxValue);
            balanceLabel.Content = v.ToString("C");
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
            GenerateSuggestions(_user.Account);

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
            GenerateSuggestions(_user.Account);
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

        private void SortByDateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateHistoryTable();
        }

        private void SortByCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            List<Transaction> transactions =
                _context.Transactions.Where(t => t.AccountId.Equals(_user.Account.Id)).OrderByDescending(t => t.Category).ToList();

            HistoryTable.ItemsSource = transactions;
        }

        private void GenerateSuggestions(Account acc)
        {
            if (acc.GoalEndDate > DateTime.Now)
            {
                mainSuggestions.Text = SuggestionsForUser.CompareExpenses(acc);
                savedSum.Text = "Taupymo laikotarpiu sutaupyta suma: " + TransactionsCounter.SavedSum(acc.Transactions, acc.GoalStartDate, acc.GoalEndDate).ToString("C");
                moneyToSpend.Text = "Pinigų suma, kurią galite skirti papildomoms išlaidoms: ";
                if(MoneyCounter.AmountLeftToSpend(acc) != -1)
                { 
                    moneyToSpend.Text = moneyToSpend.Text + Math.Round(MoneyCounter.AmountLeftToSpend(acc), 2).ToString("C");
                }
                estimatedTime.Text = MoneyCounter.EstimatedTime(acc);
                amountToSave.Text = "Kiekvieną mėnesį turėtumėte sutaupyti: " + Math.Round(MoneyCounter.AmountToSaveAMonth(acc), 2).ToString("C");
                timeInDays.Text = "Iki tikslo pabaigos jums liko " + MoneyCounter.DaysLeft(acc) + " dienos. ";
                //gal dar reikia, kiek tą mėnesį žmogus sutaupė/išleido?;
            }

            else if (acc.Goal == 0)
            {
                mainSuggestions.Text = "Šiuo metu nesate pasirinkę jokio taupymo režimo";
                MakeDefaultTextBoxes();
            }

            else if (TransactionsCounter.SavedSum(_user.Account.Transactions, _user.Account.GoalStartDate, _user.Account.GoalEndDate) < _user.Account.Goal)
            {
                mainSuggestions.Text = "Sutaupyti sumos laiku nesugebėjote";
                MakeDefaultTextBoxes();
            }

            else
            {
                mainSuggestions.Text = "Sveikiname, sugebėjote pasiekti savo tikslą laiku!" +
                    "Sutaupyta: " + Math.Round(TransactionsCounter.SavedSum(_user.Account.Transactions, _user.Account.GoalStartDate, _user.Account.GoalEndDate), 2).ToString("C");
                MakeDefaultTextBoxes();
                RemoveUserGoal();
            }
        }

        public void MakeDefaultTextBoxes()
        {
            savedSum.Text = "Taupymo laikotarpiu sutaupyta suma:";
            moneyToSpend.Text = "Pinigų suma, kurią galite skirti išlaidoms:";
            estimatedTime.Text = "Taip taupydami, savo tikslą pasieksite:";
            amountToSave.Text = "Kiekvieną mėnesį turėtumėte sutaupyti:";
            timeInDays.Text = "Iki tikslo pabaigos jums liko:";
        }

        public void RemoveUserGoal()
        {
            _user.Account.GoalStartDate = DateTime.MinValue;
            _user.Account.GoalStartDate = DateTime.MinValue;
            _user.Account.Goal = 0;
        }
    }
}
