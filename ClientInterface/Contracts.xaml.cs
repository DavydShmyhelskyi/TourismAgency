using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Core;  // Підключення моделей БД
using Repositorys; // Підключення репозиторіїв
using static Repositorys.Repos; // Для скорочення назв класів репозиторіїв

namespace ClientInterface
{
    public partial class Contracts : Window
    {
        private readonly TAContext _context;
        private readonly ContractRepository _contractRepo;
        private readonly OrderRepository _orderRepo;
        private int _clientId;

        public Contracts()
        {
            InitializeComponent();
            _context = new TAContext();
            _contractRepo = new ContractRepository(_context);
            _orderRepo = new OrderRepository(_context);
            LoadClientId();
            LoadContracts();
        }

        private void LoadClientId()
        {
            try
            {
                _clientId = int.Parse(File.ReadAllText("ClientIdLogin.txt"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження ClientId: " + ex.Message);
                Close();
            }
        }

        private void LoadContracts()
        {
            OrdersListBox.Items.Clear();

            var orders = _orderRepo.Get().Where(o => o.ClientID == _clientId).Select(o => o.OrderID).ToList();
            var contracts = _contractRepo.Get().Where(c => orders.Contains(c.OrderID)).ToList();

            foreach (var contract in contracts)
            {
                if (contract.SigningDate < DateTime.Today && contract.ContractStatusID == 1)
                {
                    contract.ContractStatusID = 3;
                    _contractRepo.Update(contract);
                }

                OrdersListBox.Items.Add(contract);
            }

            _contractRepo.SaveChanges();
        }

        private void TermListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersListBox.SelectedItem is Core.Contracts selectedContract) // Використовуємо повний шлях
            {
                TerminateContractButton.IsEnabled = selectedContract.ContractStatusID == 1;
            }
        }

        private void TerminateContractButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersListBox.SelectedItem is Core.Contracts selectedContract) // Використовуємо повний шлях
            {
                selectedContract.ContractStatusID = 2; // Розірваний контракт
                _contractRepo.Update(selectedContract);
                _contractRepo.SaveChanges();

                // Запис оновленої інформації у файл
                SaveContractToFile(selectedContract);

                LoadContracts();
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Menu menuWindow = new Menu();
            menuWindow.Show();
            this.Close();
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

    }
}
