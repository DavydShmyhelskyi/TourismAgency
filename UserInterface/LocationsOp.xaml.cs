using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Core;
using Repositorys;

namespace UserInterface
{
    public partial class LocationsOp : Window
    {
        private readonly TAContext _context;
        private readonly Repos.LocationRepository _locationRepository;
        private readonly Repos.CityRepository _cityRepository;
        private readonly Repos.CountryRepository _countryRepository;
        private Locations _selectedLocation;

        public LocationsOp()
        {
            InitializeComponent();
            _context = new TAContext();
            _locationRepository = new Repos.LocationRepository(_context);
            _cityRepository = new Repos.CityRepository(_context);
            _countryRepository = new Repos.CountryRepository(_context);

            LoadCountries();
            LoadLocations();
        }

        private void LoadCountries()
        {
            var countries = _countryRepository.Get().ToList();
            CountryComboBox.Items.Clear();
            foreach (var country in countries)
            {
                CountryComboBox.Items.Add(country.CountryName);
            }
        }

        private void CountryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadCities();
        }

        private void LoadCities()
        {
            CityComboBox.Items.Clear();
            var selectedCountry = CountryComboBox.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedCountry))
            {
                var country = _countryRepository.Get().FirstOrDefault(c => c.CountryName == selectedCountry);
                if (country != null)
                {
                    var cities = _cityRepository.Get().Where(c => c.CountryID == country.CountryID).ToList();
                    foreach (var city in cities)
                    {
                        CityComboBox.Items.Add(city.CityName);
                    }
                }
            }
        }

        private void AddCityButton_Click(object sender, RoutedEventArgs e)
        {
            if (CountryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a country before adding a city.");
                return;
            }

            string newCityName = Microsoft.VisualBasic.Interaction.InputBox("Enter new city name:", "Add City", "");
            if (string.IsNullOrWhiteSpace(newCityName))
            {
                MessageBox.Show("City name cannot be empty.");
                return;
            }

            var selectedCountry = CountryComboBox.SelectedItem.ToString();
            var country = _countryRepository.Get().FirstOrDefault(c => c.CountryName == selectedCountry);

            if (country == null)
            {
                MessageBox.Show("Selected country does not exist.");
                return;
            }

            // Перевіряємо, чи місто вже існує
            var existingCity = _cityRepository.Get().FirstOrDefault(c => c.CityName == newCityName && c.CountryID == country.CountryID);
            if (existingCity != null)
            {
                MessageBox.Show("City already exists in the selected country.");
                return;
            }

            // Додаємо нове місто
            var newCity = new Cities
            {
                CityName = newCityName,
                CountryID = country.CountryID
            };

            _cityRepository.Create(newCity);
            _context.SaveChanges();

            MessageBox.Show($"City '{newCityName}' added successfully to {selectedCountry}.");
            LoadCities();
            CityComboBox.SelectedItem = newCityName;
        }

        private void LoadLocations()
        {
            LocationsListBox.Items.Clear();
            var locations = _locationRepository.Get().ToList();
            foreach (var location in locations)
            {
                LocationsListBox.Items.Add(location);
            }
        }

        private void LocationsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LocationsListBox.SelectedItem is Locations location)
            {
                _selectedLocation = location;
                LocationNameTextBox.Text = location.LocationName;
                LocationDescriptionTextBox.Text = location.LocotionDescription;

                var city = _cityRepository.Get().FirstOrDefault(c => c.CityID == location.CityID);
                if (city != null)
                {
                    CityComboBox.SelectedItem = city.CityName;
                    var country = _countryRepository.Get().FirstOrDefault(c => c.CountryID == city.CountryID);
                    CountryComboBox.SelectedItem = country?.CountryName;
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            var cityName = CityComboBox.Text;
            var city = _cityRepository.Get().FirstOrDefault(c => c.CityName == cityName);

            if (city == null)
            {
                MessageBox.Show("Selected city does not exist.");
                return;
            }

            var newLocation = new Locations
            {
                LocationName = LocationNameTextBox.Text,
                LocotionDescription = LocationDescriptionTextBox.Text,
                CityID = city.CityID
            };

            _locationRepository.Create(newLocation);
            _locationRepository.SaveChanges();

            LoadLocations();
            MessageBox.Show("Location added successfully!");
            ClearFields();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(LocationNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LocationDescriptionTextBox.Text) ||
                CountryComboBox.SelectedItem == null ||
                CityComboBox.SelectedItem == null)
            {
                MessageBox.Show("All fields must be filled out, and country and city must be selected.");
                return false;
            }
            return true;
        }

        private void ClearFields()
        {
            LocationNameTextBox.Text = "";
            LocationDescriptionTextBox.Text = "";
            CountryComboBox.SelectedItem = null;
            CityComboBox.Items.Clear();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedLocation == null)
            {
                MessageBox.Show("Please select a location to edit.");
                return;
            }

            if (!ValidateInputs()) return;

            var cityName = CityComboBox.Text;
            var city = _cityRepository.Get().FirstOrDefault(c => c.CityName == cityName);

            if (city == null)
            {
                MessageBox.Show("Selected city does not exist.");
                return;
            }

            _selectedLocation.LocationName = LocationNameTextBox.Text;
            _selectedLocation.LocotionDescription = LocationDescriptionTextBox.Text;
            _selectedLocation.CityID = city.CityID;

            _locationRepository.Update(_selectedLocation);
            _locationRepository.SaveChanges();

            LoadLocations();
            MessageBox.Show("Location updated successfully!");
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedLocation == null)
            {
                MessageBox.Show("Please select a location to delete.");
                return;
            }

            _locationRepository.Delete(_selectedLocation.LocationID);
            _locationRepository.SaveChanges();

            LoadLocations();
            ClearFields();
            MessageBox.Show("Location deleted successfully!");
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchName = SearchTextBox.Text.ToLower();

            var locations = _locationRepository.Get().Where(l =>
                string.IsNullOrWhiteSpace(searchName) || l.LocationName.ToLower().Contains(searchName)).ToList();

            LocationsListBox.Items.Clear();
            foreach (var location in locations)
            {
                LocationsListBox.Items.Add(location);
            }
        }
    }
}
