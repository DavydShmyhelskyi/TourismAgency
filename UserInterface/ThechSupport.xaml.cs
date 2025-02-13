using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Core;
using Repositorys;

namespace UserInterface
{
    public partial class ThechSupport : Window
    {
        private readonly TAContext _context;
        private readonly Repos.ClientRepository _clientRepository;
        private Clients _selectedClient;

        public ThechSupport()
        {
            InitializeComponent();
            _context = new TAContext();
            _clientRepository = new Repos.ClientRepository(_context);

            LoadClients();
        }

        private void LoadClients()
        {
            TermListBox.Items.Clear();
            var clients = _clientRepository.Get().ToList();
            foreach (var client in clients)
            {
                TermListBox.Items.Add(client);
            }
        }

        private void TermListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TermListBox.SelectedItem is Clients client)
            {
                _selectedClient = client;
                Email.Text = client.Email;
                FirstName.Text = client.FirstName;
                LastName.Text = client.LastName;
                Phone.Text = client.Phone;
                myDatePicker.SelectedDate = client.BirthDate;
                Password.Text = client.Password;  // Підтягування пароля
            }
        }


        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedClient == null)
            {
                MessageBox.Show("Please select a client to edit.");
                return;
            }

            if (!ValidateInputs()) return;

            _selectedClient.Email = Email.Text;
            _selectedClient.FirstName = FirstName.Text;
            _selectedClient.LastName = LastName.Text;
            _selectedClient.Phone = Phone.Text;
            _selectedClient.BirthDate = myDatePicker.SelectedDate ?? DateTime.Now;
            _selectedClient.Password = Password.Text;  // Оновлення пароля

            _clientRepository.Update(_selectedClient);
            _clientRepository.SaveChanges();

            LoadClients();
            MessageBox.Show("Client information updated successfully!");
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedClient == null)
            {
                MessageBox.Show("Please select a client to delete.");
                return;
            }

            _clientRepository.Delete(_selectedClient.ClientID);
            _clientRepository.SaveChanges();

            LoadClients();
            ClearFields();
            MessageBox.Show("Client deleted successfully!");
        }

        private void SearchClientButton_Click(object sender, RoutedEventArgs e)
        {
            string firstNameFilter = FirsnNameSearch.Text.ToLower();
            string lastNameFilter = LastNameSearch.Text.ToLower();

            var clients = _clientRepository.Get().Where(c =>
                (string.IsNullOrWhiteSpace(firstNameFilter) || c.FirstName.ToLower().Contains(firstNameFilter)) &&
                (string.IsNullOrWhiteSpace(lastNameFilter) || c.LastName.ToLower().Contains(lastNameFilter))
            ).ToList();

            TermListBox.Items.Clear();
            foreach (var client in clients)
            {
                TermListBox.Items.Add(client);
            }
        }

        private void ChangeAddressButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedClient == null)
            {
                MessageBox.Show("Please select a client to change address.");
                return;
            }
            // Відкриваємо вікно зміни адреси з передачею ClientID
            ChangeAddress changeAddressWindow = new ChangeAddress(_selectedClient.ClientID);
            changeAddressWindow.ShowDialog();  // Використовуємо ShowDialog, щоб дочекатися закриття вікна
            LoadClients();  // Оновлюємо список клієнтів після зміни адреси
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            this.Close();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(Email.Text) ||
                string.IsNullOrWhiteSpace(FirstName.Text) ||
                string.IsNullOrWhiteSpace(LastName.Text) ||
                string.IsNullOrWhiteSpace(Phone.Text) ||
                string.IsNullOrWhiteSpace(Password.Text) ||  // Перевірка пароля
                myDatePicker.SelectedDate == null)
            {
                MessageBox.Show("All fields must be filled out.");
                return false;
            }

            if (!Regex.IsMatch(Email.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format.");
                return false;
            }

            if (!Regex.IsMatch(Phone.Text, @"^\+380\d{9}$"))
            {
                MessageBox.Show("Phone number must be in the format +380XXXXXXXXX.");
                return false;
            }

            if (Password.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.");
                return false;
            }

            return true;
        }


        private void ClearFields()
        {
            Email.Text = "";
            FirstName.Text = "";
            LastName.Text = "";
            Phone.Text = "";
            myDatePicker.SelectedDate = null;
        }
    }
}
