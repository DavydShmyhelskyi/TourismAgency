using System;
using System.Linq;
using System.Windows;
using Core;
using Repositorys;

namespace UserInterface
{
    public partial class ChangeAddress : Window
    {
        private readonly TAContext _context;
        private readonly Repos.ClientRepository _clientRepository;
        private readonly Repos.AddressRepository _addressRepository;
        private readonly Repos.CityRepository _cityRepository;
        private readonly Repos.CountryRepository _countryRepository;
        private Clients _client;
        private Addresses _address;

        public ChangeAddress(int clientId)
        {
            InitializeComponent();
            _context = new TAContext();
            _clientRepository = new Repos.ClientRepository(_context);
            _addressRepository = new Repos.AddressRepository(_context);
            _cityRepository = new Repos.CityRepository(_context);
            _countryRepository = new Repos.CountryRepository(_context);

            LoadClientAddress(clientId);
            LoadCountries();  // Завантаження списку країн
        }

        private void LoadCountries()
        {
            var countries = _countryRepository.Get().ToList();
            foreach (var country in countries)
            {
                CountryComboBox.Items.Add(country.CountryName);
            }
        }

        private void LoadClientAddress(int clientId)
        {
            _client = _clientRepository.Get().FirstOrDefault(c => c.ClientID == clientId);
            if (_client == null)
            {
                MessageBox.Show("Client not found.");
                this.Close();
                return;
            }

            _address = _addressRepository.Get().FirstOrDefault(a => a.AddressID == _client.AddressID);
            if (_address != null)
            {
                var city = _cityRepository.Get().FirstOrDefault(c => c.CityID == _address.CityID);
                CityTextBox.Text = city?.CityName ?? "";
                var country = _countryRepository.Get().FirstOrDefault(c => c.CountryID == city?.CountryID);
                CountryComboBox.SelectedItem = country?.CountryName ?? "";
                StreetTextBox.Text = _address.Street ?? "";
                BuildingTextBox.Text = _address.Building ?? "";
            }
            else
            {
                MessageBox.Show("Address not found.");
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_client == null)
                {
                    MessageBox.Show("Client not loaded.");
                    return;
                }

                string cityName = CityTextBox.Text.Trim();
                string countryName = CountryComboBox.Text.Trim();

                if (string.IsNullOrEmpty(cityName) || string.IsNullOrEmpty(countryName))
                {
                    MessageBox.Show("Please enter both City and select Country.");
                    return;
                }

                // Перевіряємо існування країни
                var country = _countryRepository.Get().FirstOrDefault(c => c.CountryName == countryName);
                if (country == null)
                {
                    MessageBox.Show($"Country '{countryName}' does not exist. Please add it before proceeding.");
                    return;
                }

                // Перевіряємо існування міста
                var city = _cityRepository.Get().FirstOrDefault(c => c.CityName == cityName && c.CountryID == country.CountryID);
                if (city == null)
                {
                    MessageBoxResult addCityResult = MessageBox.Show(
                        $"City '{cityName}' does not exist in {countryName}. Do you want to add it?",
                        "City Not Found",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (addCityResult == MessageBoxResult.Yes)
                    {
                        city = new Cities { CityName = cityName, CountryID = country.CountryID };
                        _cityRepository.Create(city);
                        _context.SaveChanges();
                        MessageBox.Show("City added successfully.");
                    }
                    else
                    {
                        return;
                    }
                }

                // Запит, чи змінити існуючу адресу або додати нову
                MessageBoxResult changeOrAddResult = MessageBox.Show(
                    "Do you want to update the existing address (Yes) or add a new one (No)?",
                    "Update or Add Address",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.No);

                if (changeOrAddResult == MessageBoxResult.Yes)
                {
                    // Оновлюємо існуючу адресу
                    _address.CityID = city.CityID;
                    _address.Street = StreetTextBox.Text;
                    _address.Building = BuildingTextBox.Text;

                    _addressRepository.Update(_address);
                    _addressRepository.SaveChanges();

                    MessageBox.Show("Address updated successfully!");
                }
                else if (changeOrAddResult == MessageBoxResult.No)
                {
                    // Додаємо нову адресу і оновлюємо ID клієнта
                    var newAddress = new Addresses
                    {
                        CityID = city.CityID,
                        Street = StreetTextBox.Text,
                        Building = BuildingTextBox.Text
                    };

                    _addressRepository.Create(newAddress);
                    _context.SaveChanges();

                    // Прив'язуємо нову адресу до клієнта
                    _client.AddressID = newAddress.AddressID;
                    _clientRepository.Update(_client);
                    _clientRepository.SaveChanges();

                    MessageBox.Show("New address added and linked to the client successfully!");
                }
                else
                {
                    // Користувач натиснув Cancel
                    MessageBox.Show("Operation cancelled.");
                }

                this.Close();  // Закриваємо вікно після завершення операції
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
