using Core;
using Repositorys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : Window
    {
        private readonly TAContext _context;
        private readonly Repos.UserRepository _userRepository;
        private Users _currentUser;

        public Account()
        {
            InitializeComponent();
            _context = new TAContext();
            _userRepository = new Repos.UserRepository(_context);
            LoadUserData(); // Завантаження даних користувача при відкритті вікна
        }
        private void LoadUserData()
        {
            try
            {
                string filePath = "UserIdLogin.txt";

                if (!File.Exists(filePath))
                {
                    MessageBox.Show("User is not logged in.");
                    Close();
                    return;
                }

                int userId = int.Parse(File.ReadAllText(filePath));
                _currentUser = _userRepository.Get().FirstOrDefault(u => u.UserID == userId);

                if (_currentUser != null)
                {
                    // Підтягування даних у текстові поля
                    Username.Text = _currentUser.UserName;
                    Password.Text = _currentUser.Password;
                    Email.Text = _currentUser.Email;
                    FirstName.Text = _currentUser.FirstName;
                    LastName.Text = _currentUser.LastName;
                    Phone.Text = _currentUser.Phone;
                }
                else
                {
                    MessageBox.Show("User not found.");
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}");
            }
        }

        private void ApplyChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("User data is not loaded.");
                return;
            }

            // Валідація введених даних
            if (!ValidateInputs()) return;

            // Збереження даних до змін
            string beforeChanges = GetUserDataString(_currentUser);

            // Оновлення даних користувача
            _currentUser.UserName = Username.Text;
            _currentUser.Password = Password.Text;
            _currentUser.Email = Email.Text;
            _currentUser.FirstName = FirstName.Text;
            _currentUser.LastName = LastName.Text;
            _currentUser.Phone = Phone.Text;

            try
            {
                _userRepository.Update(_currentUser);
                _userRepository.SaveChanges();

                // Збереження даних після змін
                string afterChanges = GetUserDataString(_currentUser);

                // Запис змін у файл історії
                LogChangesToFile(beforeChanges, afterChanges, _currentUser.UserID);

                MessageBox.Show("User information updated successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user data: {ex.Message}");
            }
        }

        private string GetUserDataString(Users user)
        {
            return $"Username: {user.UserName}, Password: {user.Password}, Email: {user.Email}, " +
                   $"First Name: {user.FirstName}, Last Name: {user.LastName}, Phone: {user.Phone}";
        }

        private void LogChangesToFile(string before, string after, int userId)
        {
            string filePath = "UsersChangesHistory.txt";
            string logEntry = $"# Date of changing user id:{userId} {DateTime.Now:dd.MM.yyyy}\n" +
                              $"Before: {before}\n" +
                              $"After: {after}\n\n";

            File.AppendAllText(filePath, logEntry);
        }


        private bool ValidateInputs()
        {
            // Перевірка, що всі поля заповнені
            if (string.IsNullOrWhiteSpace(Username.Text) ||
                string.IsNullOrWhiteSpace(Password.Text) ||
                string.IsNullOrWhiteSpace(Email.Text) ||
                string.IsNullOrWhiteSpace(FirstName.Text) ||
                string.IsNullOrWhiteSpace(LastName.Text) ||
                string.IsNullOrWhiteSpace(Phone.Text))
            {
                MessageBox.Show("All fields must be filled out.");
                return false;
            }

            // Перевірка формату email
            if (!Regex.IsMatch(Email.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format.");
                return false;
            }

            // Перевірка формату телефону (приклад: український формат)
            if (!Regex.IsMatch(Phone.Text, @"^\+380\d{9}$"))
            {
                MessageBox.Show("Phone number must be in the format +380XXXXXXXXX.");
                return false;
            }

            // Перевірка довжини пароля
            if (Password.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.");
                return false;
            }

            return true;
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            // Видаляємо файл логіну та закриваємо вікно
            File.Delete("UserIdLogin.txt");
            MessageBox.Show("Logged out successfully.");
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
            
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            this.Close();
        }
    }
}

