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
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.EntityFrameworkCore;

namespace SmartSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AuthenticationServices _auth;

        public MainWindow()
        {
            InitializeComponent();
            _auth = new AuthenticationServices();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Veikia
            // _auth.Register("povilasl", "HaK8N!Mat!8", "+370625111220");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Veikia
            //var user = _auth.Login("povilasl", "HaK8N!Mat!8");
        }
    }
}
