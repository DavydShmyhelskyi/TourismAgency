using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Core;
using static Repositorys.Repos;

namespace ClientInterface
{
    public partial class Orders : Window
    {
        private readonly OrderRepository _orderRepository;
        private readonly ContractRepository _contractRepository;
        private int _clientId;
        private Core.Orders _selectedOrder;

        public Orders()
        {
            InitializeComponent();
            var context = new TAContext();
            _orderRepository = new OrderRepository(context);
            _contractRepository = new ContractRepository(context);

            LoadClientId();
            LoadOrders();
        }

        private void LoadClientId()
        {
            try
            {
                string clientIdStr = System.IO.File.ReadAllText("ClientIdLogin.txt").Trim();
                if (int.TryParse(clientIdStr, out int clientId))
                {
                    _clientId = clientId;
                }
                else
                {
                    MessageBox.Show("Invalid Client ID format.");
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Client ID: " + ex.Message);
                Close();
            }
        }

        private void LoadOrders()
        {
            var orders = _orderRepository
                .Get()
                .Where(o => o.ClientID == _clientId)
                .ToList();

            OrdersListBox.ItemsSource = orders;
        }




        private void TermListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersListBox.SelectedItem is Core.Orders selectedOrder)
            {
                _selectedOrder = selectedOrder;

                // Умови активації кнопок
                RejectOrderButton.IsEnabled = _selectedOrder.StatusID == 1 || _selectedOrder.StatusID == 4;
                PayForOrderButton.IsEnabled = _selectedOrder.StatusID == 4;
            }
        }



        private void RejectOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Please select an order.");
                return;
            }

            try
            {
                _selectedOrder.StatusID = 3; // Rejected
                _orderRepository.Update(_selectedOrder);
                _orderRepository.SaveChanges();

                MessageBox.Show("Order has been rejected.");
                LoadOrders(); // Оновити список замовлень
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error rejecting order: {ex.Message}");
            }
        }

        private void PayForOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Please select an order.");
                return;
            }

            try
            {
                // Оновлення статусу замовлення
                _selectedOrder.StatusID = 2; // Paid
                _orderRepository.Update(_selectedOrder);
                _orderRepository.SaveChanges();

                // Створення контракту
                var newContract = new Core.Contracts
                {
                    OrderID = _selectedOrder.OrderID,
                    SigningDate = DateTime.Now,
                    ContractStatusID = 1 // Active
                };

                _contractRepository.Create(newContract);
                _contractRepository.SaveChanges();

                // Запис у ContractHistory.txt
                SaveContractToFile(newContract);

                MessageBox.Show("Payment successful. Contract created.");
                LoadOrders(); // Оновлення списку замовлень
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}");
            }
        }
        private void SaveContractToFile(Core.Contracts contract)
        {
            string filePath = "ContractHistory.txt";
            string contractData = $@"
###
ContractID: {contract.ContractID}
OrderID: {contract.OrderID}
SigningDate: {contract.SigningDate}
ContractStatusID: {contract.ContractStatusID}
~~~
";

            try
            {
                File.AppendAllText(filePath, contractData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing contract to file: {ex.Message}");
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Menu menuWindow = new Menu();
            menuWindow.Show();
            this.Close();
        }
    }
}
