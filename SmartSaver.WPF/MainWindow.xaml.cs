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

namespace SmartSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LogInWindow loginW = new LogInWindow(); 
            loginW.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) //REGISTER launch button
        {
            RegisterWindow registerW = new RegisterWindow();
            registerW.Show();
        }
    }
}
