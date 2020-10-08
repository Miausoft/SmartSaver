using SmartSaver.Domain.Services.AuthenticationServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.WPF
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly IAuthenticationService _auth;
        string username, phone_number;
        bool informationCorrect;

        public RegisterWindow(IAuthenticationService auth)
        {
            InitializeComponent();
            _auth = auth;
        }

        private void Button_Click(object sender, RoutedEventArgs e) // REGISTER button
        {
            informationCorrect = true;

            if (newUsernameTextbox.Text.Length > 5) // username proofreading
            {
                username = newUsernameTextbox.Text;
            }
            else
            {
                MessageBox.Show("Username is too short!");
                informationCorrect = false;
            }

            if (newPassword1Textbox.Password != newPassword2Textbox.Password) // if passwords match
            {
                MessageBox.Show("Passwords do not match!");
                informationCorrect = false;
            }

            phone_number = newPhoneNumberTextbox.Text;

            if (informationCorrect)
            {
                RegistrationResult registrationResult = _auth.Register(new User()
                {
                    Username = username, 
                    Password = newPassword1Textbox.Password, 
                    PhoneNumber = phone_number
                });

                switch (registrationResult)
                {
                    case RegistrationResult.Success:
                        MessageBox.Show("Accounts successfully created. \n    Try to log in.");
                        registerW.Close();
                        break;

                    case RegistrationResult.UserAlreadyExist:
                        MessageBox.Show("Users alerady exists.");
                        break;

                    case RegistrationResult.BadPasswordFormat:
                        MessageBox.Show("Invalid user object.");
                        break;
                }

            }
        }
    }
}
