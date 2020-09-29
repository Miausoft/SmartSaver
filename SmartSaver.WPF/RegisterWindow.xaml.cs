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
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.WPF
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        /*string username, password;
        int phone_number;
        bool informationCorrect;*/

        private readonly IAuthenticationServices _auth;

        public RegisterWindow()
        {
            InitializeComponent();
            _auth = new AuthenticationServices();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*informationCorrect = true;

            if (newUsernameTextbox.Text.Length > 5) // username proofreading
            {
                username = newUsernameTextbox.Text;
            }
            else {
                MessageBox.Show("Username is too short!");
                informationCorrect = false;
            }

            if (newPassword1Textbox == newPassword2Textbox) // if passwords match
            {
                password = newPassword1Textbox.Text;
            }
            else
            {
                MessageBox.Show("Passwords do not match!");
                informationCorrect = false;
            }

            phone_number = int.Parse(newPhoneNumberTextbox.Text); // no set restrictions yet

            if(informationCorrect)
            {
                // ADD USER CONSTRUCTOR HERE WITH username, password, phone_number
                registerW.Close();
            }*/


            // Į result metodas gražina ar enum reikšmę su kažkokiu pranešimu.
            // Su juo galima tikrint ar registracija pavyko.

            RegistrationResult result = _auth.Register(new User
            {
                Username = newUsernameTextbox.Text,
                Password = newPassword1Textbox.Text,
                PhoneNumber = newPhoneNumberTextbox.Text
            });

        }
    }
}
