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

namespace SmartSaver.WPF
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private IAuthenticationServices as1;
        string username, phone_number;
        bool informationCorrect;

        public RegisterWindow()
        {
            InitializeComponent();
            as1 = new AuthenticationServices();
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
                RegistrationResult registrationResult = as1.Register(username, newPassword1Textbox.Password, phone_number);
                switch (registrationResult)
                {
                    case RegistrationResult.Success:
                        MessageBox.Show("Account successfully created. \n    Try to log in.");
                        registerW.Close();
                        break;

                    case RegistrationResult.UserAlreadyExist:
                        MessageBox.Show("User alerady exists.");
                        break;

                    case RegistrationResult.InvalidUserObject:
                        MessageBox.Show("Invalid user object.");
                        break;
                }

            }


        }
    }
}
