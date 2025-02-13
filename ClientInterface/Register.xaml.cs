using System;
using System.Linq;
using System.Windows;
using Core;
using Microsoft.EntityFrameworkCore;

namespace ClientInterface
{
    public partial class Register : Window
    {
        private readonly TAContext _context = new TAContext();

        public Register()
        {
            InitializeComponent();
            LoadCountries();
        }

        private void LoadCountries()
        {
            var countries = _context.Countries.Select(c => c.CountryName).ToList();
            CountryComboBox.ItemsSource = countries;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Отримуємо дані з форми
                string email = Email.Text.Trim();
                string password = Password.Password.Trim();
                string firstName = FirstName.Text.Trim();
                string lastName = LastName.Text.Trim();
                string phone = Phone.Text.Trim();
                DateTime? birthDate = myDatePicker.SelectedDate;

                string countryName = CountryComboBox.SelectedItem?.ToString();
                string cityName = CityTextBox.Text.Trim();
                string street = StreetTextBox.Text.Trim();
                string building = BuildingTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                    string.IsNullOrWhiteSpace(phone) || birthDate == null ||
                    string.IsNullOrWhiteSpace(countryName) || string.IsNullOrWhiteSpace(cityName) ||
                    string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(building))
                {
                    MessageBox.Show("Будь ласка, заповніть усі поля!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // Перевіряємо, чи існує країна
                        var country = _context.Countries.FirstOrDefault(c => c.CountryName == countryName);
                        if (country == null)
                        {
                            MessageBox.Show("Оберіть країну зі списку!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Перетворюємо назву міста так, щоб воно починалося з великої літери
                        cityName = char.ToUpper(cityName[0]) + cityName.Substring(1).ToLower();

                        // Перевіряємо, чи існує місто
                        var city = _context.Cities.FirstOrDefault(c => c.CityName == cityName && c.CountryID == country.CountryID);
                        if (city == null)
                        {
                            city = new Cities { CityName = cityName, CountryID = country.CountryID };
                            _context.Cities.Add(city);
                            _context.SaveChanges();
                        }

                        // Перевіряємо, чи існує адреса
                        var address = _context.Addresses.FirstOrDefault(a =>
                            a.CityID == city.CityID && a.Street == street && a.Building == building);

                        if (address == null)
                        {
                            address = new Addresses
                            {
                                CityID = city.CityID,
                                Street = street,
                                Building = building
                            };
                            _context.Addresses.Add(address);
                            _context.SaveChanges();
                        }

                        // Додаємо нового клієнта
                        var newClient = new Clients
                        {
                            Email = email,
                            Password = password,
                            FirstName = firstName,
                            LastName = lastName,
                            Phone = phone,
                            BirthDate = birthDate.Value,
                            AddressID = address.AddressID
                        };

                        _context.Clients.Add(newClient);
                        _context.SaveChanges();

                        transaction.Commit();
                        MessageBox.Show("Реєстрація успішна!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Несподівана помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
        }
    }
}
