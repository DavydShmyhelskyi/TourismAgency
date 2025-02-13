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
    public partial class ContractsOp : Window
    {
        private readonly TAContext _context;
        private readonly Repos.ContractRepository _contractRepository;
        private readonly Repos.ContractStatusRepository _contractStatusRepository;
        private readonly Repos.OrderRepository _orderRepository;
        private readonly Repos.ClientRepository _clientRepository;
        private Contracts _selectedContract;
        private int _loggedInUserId;

        public ContractsOp()
        {
            InitializeComponent();
            _context = new TAContext();
            _contractRepository = new Repos.ContractRepository(_context);
            _contractStatusRepository = new Repos.ContractStatusRepository(_context);
            _orderRepository = new Repos.OrderRepository(_context);
            _clientRepository = new Repos.ClientRepository(_context);

            LoadUserId();
            LoadContracts();
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

        // Завантаження всіх контрактів
        private void LoadContracts()
        {
            ContractListBox.Items.Clear();
            var contracts = _contractRepository.Get().ToList();
            foreach (var contract in contracts)
            {
                ContractListBox.Items.Add(contract);
            }
        }

        // Обробка вибору контракту
        private void ContractListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContractListBox.SelectedItem is Contracts contract)
            {
                _selectedContract = contract;
            }
        }

        // Завершення контракту (лише для Active)
        // Завершення контракту (лише для Active)
        private void TerminateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedContract == null)
            {
                MessageBox.Show("Please select a contract.");
                return;
            }

            if (_selectedContract.ContractStatus.ContractStatusName != "Active")
            {
                MessageBox.Show("Only active contracts can be terminated.");
                return;
            }

            ChangeContractStatus("Terminated");
            SaveContractToFile(_selectedContract);
        }

        // Продовження контракту (лише для Terminated)
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedContract == null)
            {
                MessageBox.Show("Please select a contract.");
                return;
            }

            if (_selectedContract.ContractStatus.ContractStatusName != "Terminated")
            {
                MessageBox.Show("Only terminated contracts can be continued.");
                return;
            }

            ChangeContractStatus("Active");
            SaveContractToFile(_selectedContract);
        }

        // Запис контракту у файл
        private void SaveContractToFile(Core.Contracts contract)
        {
            string filePath = @"D:\Study\C#_2\TourismAgency\ClientInterface\bin\Debug\net9.0-windows\ContractHistory.txt";

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


        // Логіка зміни статусу контракту
        private void ChangeContractStatus(string newStatus)
        {
            var status = _contractStatusRepository.Get().FirstOrDefault(s => s.ContractStatusName == newStatus);
            if (status != null)
            {
                _selectedContract.ContractStatusID = status.ContractStatusID;

                _contractRepository.Update(_selectedContract);
                _contractRepository.SaveChanges();

                ContractListBox.Items.Clear();  // Очищуємо список після зміни
                MessageBox.Show($"Contract status changed to '{newStatus}'.");
            }
            else
            {
                MessageBox.Show($"Status '{newStatus}' not found in the database.");
            }
        }

        // Пошук контрактів за клієнтом
        private void SearchByClientButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = NameTextBox_Copy6.Text.ToLower();
            string lastName = NameTextBox_Copy7.Text.ToLower();

            var clients = _clientRepository.Get().Where(c =>
                (string.IsNullOrWhiteSpace(firstName) || c.FirstName.ToLower().Contains(firstName)) &&
                (string.IsNullOrWhiteSpace(lastName) || c.LastName.ToLower().Contains(lastName))
            ).Select(c => c.ClientID).ToList();

            var orders = _orderRepository.Get().Where(o => clients.Contains(o.ClientID)).Select(o => o.OrderID).ToList();
            var contracts = _contractRepository.Get().Where(c => orders.Contains(c.OrderID)).ToList();

            ContractListBox.Items.Clear();
            foreach (var contract in contracts)
            {
                ContractListBox.Items.Add(contract);
            }
        }

        // Фільтрація контрактів за статусом "Terminated"
        private void GetTerminatedButton_Click(object sender, RoutedEventArgs e)
        {
            FilterContractsByStatus("Terminated");
        }

        // Фільтрація контрактів за статусом "Active"
        private void GetContuniedButton_Click(object sender, RoutedEventArgs e)
        {
            FilterContractsByStatus("Active");
        }

        // Завантаження всіх контрактів
        private void GetAllButton_Click(object sender, RoutedEventArgs e)
        {
            LoadContracts();
        }

        // Завантаження контрактів для авторизованого користувача
        private void GetMyButton_Click(object sender, RoutedEventArgs e)
        {
            var orders = _orderRepository.Get().Where(o => o.UserID == _loggedInUserId).Select(o => o.OrderID).ToList();
            var contracts = _contractRepository.Get().Where(c => orders.Contains(c.OrderID)).ToList();

            ContractListBox.Items.Clear();
            foreach (var contract in contracts)
            {
                ContractListBox.Items.Add(contract);
            }
        }

        // Повернення до головного меню
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            this.Close();
        }

        // Фільтрація контрактів за статусом
        private void FilterContractsByStatus(string statusName)
        {
            var status = _contractStatusRepository.Get().FirstOrDefault(s => s.ContractStatusName == statusName);
            if (status != null)
            {
                var contracts = _contractRepository.Get().Where(c => c.ContractStatusID == status.ContractStatusID).ToList();
                ContractListBox.Items.Clear();
                foreach (var contract in contracts)
                {
                    ContractListBox.Items.Add(contract);
                }
            }
            else
            {
                MessageBox.Show($"Status '{statusName}' not found in the database.");
            }
        }
    }
}
