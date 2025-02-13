using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Core;
using Repositorys;

namespace UserInterface
{
    public partial class OrdersOp : Window
    {
        private readonly TAContext _context;
        private readonly Repos.OrderRepository _orderRepository;
        private readonly Repos.StatusRepository _statusRepository;
        private readonly Repos.ClientRepository _clientRepository;
        private readonly Repos.UserRepository _userRepository;
        private Orders _selectedOrder;
        private int _loggedInUserId;

        public OrdersOp()
        {
            InitializeComponent();
            _context = new TAContext();
            _orderRepository = new Repos.OrderRepository(_context);
            _statusRepository = new Repos.StatusRepository(_context);
            _clientRepository = new Repos.ClientRepository(_context);
            _userRepository = new Repos.UserRepository(_context);

            LoadUserId();
            LoadOrders();
        }

        // Завантаження UserID з файлу
        private void LoadUserId()
        {
            try
            {
                string userIdFile = "UserIdLogin.txt";
                if (File.Exists(userIdFile))
                {
                    string userIdText = File.ReadAllText(userIdFile);
                    if (int.TryParse(userIdText, out int userId))
                    {
                        _loggedInUserId = userId;
                    }
                    else
                    {
                        MessageBox.Show("Invalid User ID in file.");
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("User ID file not found.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user ID: {ex.Message}");
                this.Close();
            }
        }

        // Завантаження всіх замовлень
        private void LoadOrders()
        {
            ContractListBox.Items.Clear();
            var orders = _orderRepository.Get().ToList();
            foreach (var order in orders)
            {
                ContractListBox.Items.Add(order);
            }
        }

        // Обробка вибору замовлення
        private void ContractListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContractListBox.SelectedItem is Orders order)
            {
                _selectedOrder = order;
            }
        }

        // Підтвердження замовлення (присвоюється авторизованому користувачу)
        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderStatus("Pending Payment", _loggedInUserId);
        }

        // Відхилення замовлення (також присвоюється авторизованому користувачу)
        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderStatus("Rejected", _loggedInUserId);
        }

        // Загальна логіка зміни статусу замовлення та оновлення UserID
        private void ChangeOrderStatus(string newStatus, int userId)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Please select an order to change status.");
                return;
            }

            var status = _statusRepository.Get().FirstOrDefault(s => s.StatusName == newStatus);
            if (status != null)
            {
                _selectedOrder.StatusID = status.StatusID;
                _selectedOrder.UserID = userId;  // Присвоюємо замовлення авторизованому користувачу

                _orderRepository.Update(_selectedOrder);
                _orderRepository.SaveChanges();

                ContractListBox.Items.Clear();  // Очищаємо список після зміни статусу
                MessageBox.Show($"Order status changed to '{newStatus}'.");
            }
            else
            {
                MessageBox.Show($"Status '{newStatus}' not found in the database.");
            }
        }

        // Пошук замовлень за клієнтом
        private void SearchByClientButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = NameTextBox_Copy6.Text.ToLower();
            string lastName = NameTextBox_Copy7.Text.ToLower();

            var clients = _clientRepository.Get().Where(c =>
                (string.IsNullOrWhiteSpace(firstName) || c.FirstName.ToLower().Contains(firstName)) &&
                (string.IsNullOrWhiteSpace(lastName) || c.LastName.ToLower().Contains(lastName))
            ).Select(c => c.ClientID).ToList();

            var orders = _orderRepository.Get().Where(o => clients.Contains(o.ClientID)).ToList();
            ContractListBox.Items.Clear();
            foreach (var order in orders)
            {
                ContractListBox.Items.Add(order);
            }
        }

        // Фільтрація замовлень за статусом "Rejected"
        private void GetTerminatedButton_Click(object sender, RoutedEventArgs e)
        {
            FilterOrdersByStatus("Rejected");
        }

        // Фільтрація замовлень за статусом "Pending Confirmation"
        private void GetContuniedButton_Click(object sender, RoutedEventArgs e)
        {
            var statusIds = _statusRepository.Get()
                .Where(s => s.StatusName == "Pending Confirmation")
                .Select(s => s.StatusID)
                .ToList();

            var orders = _orderRepository.Get().Where(o => statusIds.Contains(o.StatusID)).ToList();
            ContractListBox.Items.Clear();
            foreach (var order in orders)
            {
                ContractListBox.Items.Add(order);
            }
        }

        // Завантаження всіх замовлень
        private void GetAllButton_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        // Завантаження замовлень авторизованого користувача
        private void GetMyButton_Click(object sender, RoutedEventArgs e)
        {
            var orders = _orderRepository.Get().Where(o => o.UserID == _loggedInUserId).ToList();
            ContractListBox.Items.Clear();
            foreach (var order in orders)
            {
                ContractListBox.Items.Add(order);
            }
        }

        // Повернення до головного меню
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            this.Close();
        }

        // Фільтрація замовлень за статусом
        private void FilterOrdersByStatus(string statusName)
        {
            var status = _statusRepository.Get().FirstOrDefault(s => s.StatusName == statusName);
            if (status != null)
            {
                var orders = _orderRepository.Get().Where(o => o.StatusID == status.StatusID).ToList();
                ContractListBox.Items.Clear();
                foreach (var order in orders)
                {
                    ContractListBox.Items.Add(order);
                }
            }
            else
            {
                MessageBox.Show($"Status '{statusName}' not found in the database.");
            }
        }

        private void GetPendingPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            var statusIds = _statusRepository.Get()
                .Where(s => s.StatusName == "Pending Payment")
                .Select(s => s.StatusID)
                .ToList();

            var orders = _orderRepository.Get().Where(o => statusIds.Contains(o.StatusID)).ToList();
            ContractListBox.Items.Clear();
            foreach (var order in orders)
            {
                ContractListBox.Items.Add(order);
            }
        }
    }
}
