using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Core;
using Repositorys;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for UsersOp.xaml
    /// </summary>
    public partial class UsersOp : Window
    {
        private readonly TAContext _context;
        private readonly Repos.UserRepository _userRepository;
        private readonly Repos.RoleRepository _roleRepository;
        private Users _selectedUser;
        private int _adminUserId;

        public UsersOp()
        {
            InitializeComponent();
            _context = new TAContext();
            _userRepository = new Repos.UserRepository(_context);
            _roleRepository = new Repos.RoleRepository(_context);

            LoadAdminUserId();
            LoadUsers();
            LoadRoles();
        }

        private void LoadAdminUserId()
        {
            string filePath = "UserIdLogin.txt";
            if (File.Exists(filePath))
            {
                _adminUserId = int.Parse(File.ReadAllText(filePath));
            }
        }

        private void LoadUsers()
        {
            UsersListBox.Items.Clear();
            var users = _userRepository.Get().ToList();
            foreach (var user in users)
            {
                UsersListBox.Items.Add(user);
            }
        }

        private void LoadRoles()
        {
            RoleComboBox.Items.Clear();
            var roles = _roleRepository.Get().ToList();
            foreach (var role in roles)
            {
                RoleComboBox.Items.Add(role.RoleName);
            }
        }

        private void UsersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersListBox.SelectedItem is Users user)
            {
                _selectedUser = user;
                Username.Text = user.UserName;
                Password.Text = user.Password;
                Email.Text = user.Email;
                FirstName.Text = user.FirstName;
                LastName.Text = user.LastName;
                Phone.Text = user.Phone;
                RoleComboBox.SelectedItem = user.Role?.RoleName;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            var newUser = new Users
            {
                UserName = Username.Text,
                Password = Password.Text,
                Email = Email.Text,
                FirstName = FirstName.Text,
                LastName = LastName.Text,
                Phone = Phone.Text,
                RoleID = _roleRepository.Get().FirstOrDefault(r => r.RoleName == RoleComboBox.Text)?.RoleID ?? 0
            };

            _userRepository.Create(newUser);
            _userRepository.SaveChanges();

            LogOperation("Added", newUser);
            LoadUsers();
            MessageBox.Show("User added successfully!");
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Please select a user to edit.");
                return;
            }

            if (!ValidateInputs()) return;

            string beforeChanges = GetUserDataString(_selectedUser);

            _selectedUser.UserName = Username.Text;
            _selectedUser.Password = Password.Text;
            _selectedUser.Email = Email.Text;
            _selectedUser.FirstName = FirstName.Text;
            _selectedUser.LastName = LastName.Text;
            _selectedUser.Phone = Phone.Text;
            _selectedUser.RoleID = _roleRepository.Get().FirstOrDefault(r => r.RoleName == RoleComboBox.Text)?.RoleID ?? 0;

            _userRepository.Update(_selectedUser);
            _userRepository.SaveChanges();

            LogOperation("Edited", _selectedUser, beforeChanges);
            LoadUsers();
            MessageBox.Show("User edited successfully!");
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Please select a user to delete.");
                return;
            }

            string beforeChanges = GetUserDataString(_selectedUser);

            _userRepository.Delete(_selectedUser.UserID);
            _userRepository.SaveChanges();

            LogOperation("Deleted", _selectedUser, beforeChanges);
            LoadUsers();
            MessageBox.Show("User deleted successfully!");
        }

        private void SearchUserButton_Click(object sender, RoutedEventArgs e)
        {
            string firstNameFilter = FirstNameTextBox.Text.ToLower();
            string lastNameFilter = LastNameTextBox.Text.ToLower();

            var users = _userRepository.Get().Where(u =>
                (string.IsNullOrWhiteSpace(firstNameFilter) || u.FirstName.ToLower().Contains(firstNameFilter)) &&
                (string.IsNullOrWhiteSpace(lastNameFilter) || u.LastName.ToLower().Contains(lastNameFilter))
            ).ToList();

            UsersListBox.Items.Clear();
            foreach (var user in users)
            {
                UsersListBox.Items.Add(user);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            this.Close();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(Username.Text) ||
                string.IsNullOrWhiteSpace(Password.Text) ||
                string.IsNullOrWhiteSpace(Email.Text) ||
                string.IsNullOrWhiteSpace(FirstName.Text) ||
                string.IsNullOrWhiteSpace(LastName.Text) ||
                string.IsNullOrWhiteSpace(Phone.Text) ||
                string.IsNullOrWhiteSpace(RoleComboBox.Text))
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

        private string GetUserDataString(Users user)
        {
            return $"Username: {user.UserName}, Password: {user.Password}, Email: {user.Email}, " +
                   $"First Name: {user.FirstName}, Last Name: {user.LastName}, Phone: {user.Phone}, Role ID: {user.RoleID}";
        }

        private void LogOperation(string operation, Users user, string before = null)
        {
            string filePath = "UsersOperationsHistory.txt";
            string logEntry = $"# Admin ID: {_adminUserId} performed {operation} on user ID:{user.UserID} {DateTime.Now:dd.MM.yyyy}\n";

            if (before != null)
            {
                logEntry += $"Before: {before}\n";
            }

            logEntry += $"After: {GetUserDataString(user)}\n\n";

            File.AppendAllText(filePath, logEntry);
        }
    }
}
