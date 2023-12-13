using ChatApp.Model;
using ChatApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Main(Object Sender, StartupEventArgs e)
        {
            MainWindow main_window = new MainWindow();
            main_window.Title = "Test";
            main_window.Show();
        }
    }
}
