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
    public partial class ToursOp : Window
    {
        private readonly TAContext _context;
        private readonly Repos.TourRepository _tourRepository;
        private readonly Repos.LocationRepository _locationRepository;
        private Tours _selectedTour;

        public ToursOp()
        {
            InitializeComponent();
            _context = new TAContext();
            _tourRepository = new Repos.TourRepository(_context);
            _locationRepository = new Repos.LocationRepository(_context);

            LoadLocations();
            LoadTours();
        }

        private void LoadLocations()
        {
            var locations = _locationRepository.Get().ToList();
            LocationComboBox.Items.Clear();
            foreach (var location in locations)
            {
                LocationComboBox.Items.Add(location.LocationName);
            }
        }

        private void LoadTours()
        {
            TermListBox.Items.Clear();
            var tours = _tourRepository.Get().ToList();
            foreach (var tour in tours)
            {
                TermListBox.Items.Add(tour);
            }
        }

        private void TermListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TermListBox.SelectedItem is Tours tour)
            {
                _selectedTour = tour;
                NameTextBox.Text = tour.TourName;
                Description.Text = tour.TourDescription;
                Price.Text = tour.Price.ToString();
                Seats.Text = tour.AvailableSeats.ToString();
                StartDate.SelectedDate = tour.StartDate;
                EndDate.SelectedDate = tour.EndDate;

                var location = _locationRepository.Get().FirstOrDefault(l => l.LocationID == tour.LocationID);
                LocationComboBox.SelectedItem = location?.LocationName;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            var location = _locationRepository.Get().FirstOrDefault(l => l.LocationName == LocationComboBox.Text);
            if (location == null)
            {
                MessageBox.Show("Selected location does not exist.");
                return;
            }

            var newTour = new Tours
            {
                TourName = NameTextBox.Text,
                TourDescription = Description.Text,
                Price = int.Parse(Price.Text),
                AvailableSeats = int.Parse(Seats.Text),
                StartDate = StartDate.SelectedDate.Value,
                EndDate = EndDate.SelectedDate.Value,
                LocationID = location.LocationID
            };

            _tourRepository.Create(newTour);
            _tourRepository.SaveChanges();

            LoadTours();
            MessageBox.Show("Tour added successfully!");
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTour == null)
            {
                MessageBox.Show("Please select a tour to edit.");
                return;
            }

            if (!ValidateInputs()) return;

            var location = _locationRepository.Get().FirstOrDefault(l => l.LocationName == LocationComboBox.Text);
            if (location == null)
            {
                MessageBox.Show("Selected location does not exist.");
                return;
            }

            _selectedTour.TourName = NameTextBox.Text;
            _selectedTour.TourDescription = Description.Text;
            _selectedTour.Price = int.Parse(Price.Text);
            _selectedTour.AvailableSeats = int.Parse(Seats.Text);
            _selectedTour.StartDate = StartDate.SelectedDate.Value;
            _selectedTour.EndDate = EndDate.SelectedDate.Value;
            _selectedTour.LocationID = location.LocationID;

            _tourRepository.Update(_selectedTour);
            _tourRepository.SaveChanges();

            LoadTours();
            MessageBox.Show("Tour updated successfully!");
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTour == null)
            {
                MessageBox.Show("Please select a tour to delete.");
                return;
            }

            _tourRepository.Delete(_selectedTour.TourID);
            _tourRepository.SaveChanges();

            LoadTours();
            ClearFields();
            MessageBox.Show("Tour deleted successfully!");
        }

        private void SearchByNameButton_Click(object sender, RoutedEventArgs e)
        {
            string searchName = NameTextBox_Copy3.Text.ToLower();

            var tours = _tourRepository.Get().Where(t =>
                string.IsNullOrWhiteSpace(searchName) || t.TourName.ToLower().Contains(searchName)).ToList();

            TermListBox.Items.Clear();
            foreach (var tour in tours)
            {
                TermListBox.Items.Add(tour);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(Description.Text) ||
                string.IsNullOrWhiteSpace(Price.Text) ||
                string.IsNullOrWhiteSpace(Seats.Text) ||
                LocationComboBox.SelectedItem == null ||
                !StartDate.SelectedDate.HasValue ||
                !EndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("All fields must be filled out.");
                return false;
            }

            if (!int.TryParse(Price.Text, out int price) || price <= 0)
            {
                MessageBox.Show("Price must be a positive number.");
                return false;
            }

            if (!int.TryParse(Seats.Text, out int seats) || seats <= 0)
            {
                MessageBox.Show("Seats must be a positive number.");
                return false;
            }

            if (StartDate.SelectedDate >= EndDate.SelectedDate)
            {
                MessageBox.Show("Start date must be before end date.");
                return false;
            }

            return true;
        }

        private void ClearFields()
        {
            NameTextBox.Text = "";
            Description.Text = "";
            Price.Text = "";
            Seats.Text = "";
            StartDate.SelectedDate = null;
            EndDate.SelectedDate = null;
            LocationComboBox.SelectedItem = null;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            this.Close();
        }

        private void LocationsButton_Click(object sender, RoutedEventArgs e)
        {
            LocationsOp locationsOp = new LocationsOp();
            locationsOp.ShowDialog();
        }
    }
}
