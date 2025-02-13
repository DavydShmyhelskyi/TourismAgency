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
using System.Windows.Shapes;

namespace ClientInterface
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void Orderes_Click(object sender, RoutedEventArgs e)
        {
            Orders OrdersWindow = new Orders();
            OrdersWindow.Show();
            this.Close();
        }

        private void Contracts_Click(object sender, RoutedEventArgs e)
        {
            Contracts ContractsWindow = new Contracts();
            ContractsWindow.Show();
            this.Close();
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            Account AccountWindow = new Account();
            AccountWindow.Show();
            this.Close();
        }

        private void Tours_Click(object sender, RoutedEventArgs e)
        {
            Tours ToursWindow = new Tours();
            ToursWindow.Show();
            this.Close();
        }
    }
}
