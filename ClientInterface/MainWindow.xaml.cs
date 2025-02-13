using System.IO;
using System.Linq;
using System.Windows;
using Core;
using Repositorys;

namespace ClientInterface
{
    public partial class MainWindow : Window
    {
        private readonly TAContext _context;
        private readonly Repos.ClientRepository _clientRepository;

        public MainWindow()
        {
            InitializeComponent();
            _context = new TAContext();
            _clientRepository = new Repos.ClientRepository(_context);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string phoneNumber = PhoneNumberTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Будь ласка, введіть номер телефону та пароль.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Видаляємо зайві пробіли та дефіси для коректного пошуку
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

            // Шукаємо клієнта за номером телефону та паролем
            var client = _clientRepository.Get()
                .FirstOrDefault(c => c.Phone.Trim() == phoneNumber && c.Password.Trim() == password);

            if (client != null)
            {
                // Записуємо ClientID у файл
                string filePath = "ClientIdLogin.txt";

                // Очищення файлу перед записом
                if (File.Exists(filePath))
                {
                    File.WriteAllText(filePath, string.Empty);  // Очистка файлу
                }
                File.WriteAllText(filePath, client.ClientID.ToString());

                // Перенаправлення у вікно меню
                Menu menuWindow = new Menu();
                menuWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Невірний номер телефону або пароль.", "Помилка входу", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Register registerWindow = new Register();
            registerWindow.Show();
            this.Close();
        }
    }
}
