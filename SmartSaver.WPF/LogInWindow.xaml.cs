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

namespace SmartSaver.WPF
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        public LogInWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //usernameTextbox.Text;
            //passwordTextbox.Text;
            // situs ikeliame kai norime kad grazintu norima useri
            // man kolkas neveikia normaliai .domain, tad palieku tokioj stadijoj
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // REGISTER launch button
        {
            RegisterWindow registerW = new RegisterWindow();
            registerW.Show();
        }
    }
}
