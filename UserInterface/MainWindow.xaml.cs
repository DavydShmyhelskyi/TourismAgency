using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Core;
using Repositorys;
using System.Windows;
using static Repositorys.Repos;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SeedData _seedData;
        private readonly TAContext _context;
        private readonly UserRepository _userRepository;
        public MainWindow()
        {
            InitializeComponent();
            _seedData = new SeedData();
            _context = new TAContext();
            _userRepository = new UserRepository(_context);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _seedData.ParseCountries();   // Спочатку парсимо країни
            _seedData.ParseCapitals();    // Потім парсимо столиці
            _seedData.AddRoles();
            _seedData.AddStatuses();
            _seedData.AddPaymentMethods();
            _seedData.AddContractStatuses();
            _seedData.AddLocations();
            _seedData.AddTours();
            _seedData.AddAddresses();
            _seedData.AddClients();
            _seedData.AddUsers();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UserameTextBox.Text;
            string password = PasswordBox.Password;

            // Пошук користувача
            var user = _userRepository.Get().FirstOrDefault(u => u.UserName == username && u.Password == password);

            if (user != null)
            {
                // Якщо користувач знайдений, записуємо UserID у файл
                string filePath = "UserIdLogin.txt";

                // Очищення файлу перед записом
                if (File.Exists(filePath))
                {
                    File.WriteAllText(filePath, string.Empty);  // Очистка файлу
                }

                File.WriteAllText(filePath, user.UserID.ToString());

                // Перенаправлення або інші дії після входу

                Menu menu = new Menu();
                menu.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}