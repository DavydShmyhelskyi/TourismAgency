using System;
using System.IO;
using System.Linq;
using System.Windows;
using Core;
using Microsoft.EntityFrameworkCore;
using Repositorys;

namespace ClientInterface
{
    public partial class Account : Window
    {
        private TAContext _context;
        private Clients _client;
        private Addresses _clientAddress;

        public Account()
        {
            InitializeComponent();
            _context = new TAContext();
            LoadClientData();
        }

        private void LoadClientData()
        {
            try
            {
                // Зчитуємо ClientID з файлу
                string clientIdStr = File.ReadAllText("ClientIdLogin.txt").Trim();
                if (!int.TryParse(clientIdStr, out int clientId))
                {
                    MessageBox.Show("Невірний формат ID клієнта!");
                    return;
                }

                // Отримуємо клієнта
                _client = _context.Clients.FirstOrDefault(c => c.ClientID == clientId);
                if (_client == null)
                {
                    MessageBox.Show("Клієнт не знайдений!");
                    return;
                }

                // Отримуємо адресу клієнта (але не змінюємо її!)
                _clientAddress = _context.Addresses
                    .Include(a => a.City)
                    .ThenInclude(c => c.Country)
                    .FirstOrDefault(a => a.AddressID == _client.AddressID);

                // Заповнюємо поля
                Email.Text = _client.Email;
                FirstName.Text = _client.FirstName;
                LastName.Text = _client.LastName;
                Phone.Text = _client.Phone;
                myDatePicker.SelectedDate = _client.BirthDate;
                Password.Text = _client.Password;

                if (_clientAddress != null)
                {
                    CityTextBox.Text = _clientAddress.City.CityName;
                    StreetTextBox.Text = _clientAddress.Street;
                    BuildingTextBox.Text = _clientAddress.Building;
                    CountryComboBox.ItemsSource = _context.Countries.ToList();
                    CountryComboBox.DisplayMemberPath = "CountryName";
                    CountryComboBox.SelectedItem = _context.Countries.FirstOrDefault(c => c.CountryID == _clientAddress.City.CountryID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка завантаження даних: " + ex.Message);
            }
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Оновлення даних клієнта
                _client.Email = Email.Text;
                _client.FirstName = FirstName.Text;
                _client.LastName = LastName.Text;
                _client.Phone = Phone.Text;
                _client.BirthDate = myDatePicker.SelectedDate ?? _client.BirthDate;
                _client.Password = Password.Text;

                // Отримання нових значень адреси
                string cityName = CityTextBox.Text.Trim();
                string street = StreetTextBox.Text.Trim();
                string building = BuildingTextBox.Text.Trim();
                var selectedCountry = CountryComboBox.SelectedItem as Countries;

                if (!string.IsNullOrEmpty(cityName) && selectedCountry != null)
                {
                    // Шукаємо місто
                    var city = _context.Cities.FirstOrDefault(c => c.CityName == cityName && c.CountryID == selectedCountry.CountryID);
                    if (city == null)
                    {
                        city = new Cities { CityName = cityName, CountryID = selectedCountry.CountryID };
                        _context.Cities.Add(city);
                        _context.SaveChanges();
                    }

                    // Шукаємо чи існує така ж сама адреса
                    var newAddress = _context.Addresses.FirstOrDefault(a => a.CityID == city.CityID && a.Street == street && a.Building == building);

                    if (newAddress == null)
                    {
                        // Якщо такої адреси ще немає – створюємо нову
                        newAddress = new Addresses { CityID = city.CityID, Street = street, Building = building };
                        _context.Addresses.Add(newAddress);
                        _context.SaveChanges();
                    }

                    // Прив'язуємо нову адресу до клієнта (СТАРА НЕ ЗМІНЮЄТЬСЯ!)
                    _client.AddressID = newAddress.AddressID;
                }

                // Збереження змін
                _context.SaveChanges();
                MessageBox.Show("Дані оновлено успішно!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка оновлення даних: " + ex.Message);
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
