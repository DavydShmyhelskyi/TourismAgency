using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Core;
using static Repositorys.Repos;

namespace ClientInterface
{
    public partial class Tours : Window
    {
        private readonly TourRepository _tourRepository;
        private readonly OrderRepository _orderRepository;
        private readonly ContractRepository _contractRepository;
        private int _clientId;
        private Core.Tours _selectedTour;

        public Tours()
        {
            InitializeComponent();
            var context = new TAContext();
            _tourRepository = new TourRepository(context);
            _orderRepository = new OrderRepository(context);
            _contractRepository = new ContractRepository(context);

            LoadClientId();
            LoadTours();
        }

        private void LoadClientId()
        {
            try
            {
                string clientIdStr = File.ReadAllText("ClientIdLogin.txt").Trim();
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

        private void LoadTours()
        {
            ToursListBox.ItemsSource = _tourRepository.Get().ToList();
            ToursListBox.DisplayMemberPath = "TourName";
        }

        private void SearchByNameButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = NameTextBox_Copy3.Text.Trim();
            ToursListBox.ItemsSource = _tourRepository
                .Get()
                .Where(t => t.TourName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void ToursListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ToursListBox.SelectedItem is Core.Tours selectedTour)
            {
                _selectedTour = selectedTour;
            }
        }

        private void MakeOrderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedTour == null)
                {
                    MessageBox.Show("Please select a tour.");
                    return;
                }

                int bookedSeats = _orderRepository.Get().Count(o => o.TourID == _selectedTour.TourID);

                if (bookedSeats >= _selectedTour.AvailableSeats)
                {
                    MessageBox.Show("No available seats for this tour.");
                    return;
                }

                var newOrder = new Core.Orders
                {
                    StatusID = 1,
                    UserID = GetCurrentUserId(), // Додана функція для отримання поточного UserID
                    ClientID = _clientId,
                    TourID = _selectedTour.TourID,
                    OrderDate = DateTime.Now.ToString(),
                    TotalAmount = _selectedTour.Price,
                    OrderDetails = "Standard booking"
                };

                _orderRepository.Create(newOrder);
                _orderRepository.SaveChanges();

                MessageBox.Show("Order created successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating order: {ex.Message}");
            }
        }

        private int GetCurrentUserId()
        {
            try
            {
                string clientIdStr = File.ReadAllText("ClientIdLogin.txt").Trim();
                if (int.TryParse(clientIdStr, out int clientId))
                {
                    _clientId = clientId;
                    return _clientId;
                }
                else
                {
                    MessageBox.Show("Invalid Client ID format.");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Client ID: " + ex.Message);
                return -1;
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
