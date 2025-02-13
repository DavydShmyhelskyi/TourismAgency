using System;
using System.IO;
using System.Linq;
using System.Windows;
using Core;
using Repositorys;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Word = DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;

namespace UserInterface
{
    public partial class Menu : Window
    {
        private readonly TAContext _context;
        private readonly Repos.UserRepository _userRepository;
        private readonly Repos.ContractRepository _contractRepository;
        private readonly Repos.OrderRepository _orderRepository;
        private Users _loggedInUser;
        private SeedData _seedData;

        public Menu()
        {
            InitializeComponent();
            _context = new TAContext();
            _userRepository = new Repos.UserRepository(_context);
            _contractRepository = new Repos.ContractRepository(_context);
            _orderRepository = new Repos.OrderRepository(_context);
            _seedData = new SeedData();

            LoadLoggedInUser();
            ConfigureMenuAccess();
        }

        // Завантаження інформації про авторизованого користувача
        private void LoadLoggedInUser()
        {
            try
            {
                string userIdFile = "UserIdLogin.txt";
                if (File.Exists(userIdFile))
                {
                    string userIdText = File.ReadAllText(userIdFile);
                    if (int.TryParse(userIdText, out int userId))
                    {
                        _loggedInUser = _userRepository.Get().FirstOrDefault(u => u.UserID == userId);
                        if (_loggedInUser == null)
                        {
                            MessageBox.Show("User not found.");
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid User ID in file.");
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("User ID file not found.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user: {ex.Message}");
                this.Close();
            }
        }

        // Налаштування доступу до пунктів меню відповідно до RoleID користувача
        private void ConfigureMenuAccess()
        {
            if (_loggedInUser != null)
            {
                int roleId = _loggedInUser.RoleID;

                // Доступ лише для RoleID 1 (Owner) та RoleID 2 (System Administrator)
                bool hasAdminAccess = roleId == 1 || roleId == 2;

                // Налаштування видимості кнопок відповідно до ролі
                SeedData.Visibility = hasAdminAccess ? Visibility.Visible : Visibility.Collapsed;
                Users.Visibility = hasAdminAccess ? Visibility.Visible : Visibility.Collapsed;
                ClientTechSupport.Visibility = hasAdminAccess ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // Функції для відкриття вікон
        private void SeedData_Click(object sender, RoutedEventArgs e)
        {
            _seedData.ParseCountries();
            _seedData.ParseCapitals();
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

        private void Orderes_Click(object sender, RoutedEventArgs e)
        {
            OrdersOp ordersOpWindow = new OrdersOp();
            ordersOpWindow.Show();
            this.Close();
        }

        private void Contracts_Click(object sender, RoutedEventArgs e)
        {
            ContractsOp contractsOpWindow = new ContractsOp();
            contractsOpWindow.Show();
            this.Close();
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            UsersOp usersOpWindow = new UsersOp();
            usersOpWindow.Show();
            this.Close();
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            Account accountWindow = new Account();
            accountWindow.Show();
            this.Close();
        }

        private void Tours_Click(object sender, RoutedEventArgs e)
        {
            ToursOp toursOpWindow = new ToursOp();
            toursOpWindow.Show();
            this.Close();
        }

        private void Client_Click(object sender, RoutedEventArgs e)
        {
            ThechSupport techSupportWindow = new ThechSupport();
            techSupportWindow.Show();
            this.Close();
        }

        // Створення звіту про продажі у Word
        private void Report_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Word Documents (*.docx)|*.docx",
                    Title = "Оберіть місце для збереження звіту",
                    FileName = "SalesReport.docx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    GenerateSalesReport(saveFileDialog.FileName);
                    MessageBox.Show("Звіт успішно створено!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка створення звіту: {ex.Message}");
            }
        }

        private void GenerateSalesReport(string filePath)
        {
            var orders = _orderRepository.Get().ToList();

            DateTime today = DateTime.Today;
            DateTime weekAgo = today.AddDays(-7);
            DateTime monthAgo = today.AddMonths(-1);
            DateTime yearAgo = today.AddYears(-1);

            decimal weeklyRevenue = orders.Where(o => DateTime.Parse(o.OrderDate) >= weekAgo).Sum(o => o.TotalAmount);
            decimal monthlyRevenue = orders.Where(o => DateTime.Parse(o.OrderDate) >= monthAgo).Sum(o => o.TotalAmount);
            decimal yearlyRevenue = orders.Where(o => DateTime.Parse(o.OrderDate) >= yearAgo).Sum(o => o.TotalAmount);
            decimal totalRevenue = orders.Sum(o => o.TotalAmount);

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Word.Document();
                Word.Body body = mainPart.Document.AppendChild(new Word.Body());

                body.Append(CreateParagraph("Звіт про продажі", true, 24));
                body.Append(CreateParagraph($"Дата формування звіту: {today:dd.MM.yyyy}", false, 14));

                body.Append(CreateParagraph("Доходи компанії:", true, 18));
                body.Append(CreateParagraph($"За останній тиждень: {weeklyRevenue} грн", false, 14));
                body.Append(CreateParagraph($"За останній місяць: {monthlyRevenue} грн", false, 14));
                body.Append(CreateParagraph($"За останній рік: {yearlyRevenue} грн", false, 14));
                body.Append(CreateParagraph($"За весь час: {totalRevenue} грн", false, 14));

                mainPart.Document.Save();
            }
        }

        private Word.Paragraph CreateParagraph(string text, bool isBold, int fontSize)
        {
            Word.Run run = new Word.Run();
            Word.RunProperties runProperties = new Word.RunProperties();
            runProperties.Append(new Word.FontSize() { Val = (fontSize * 2).ToString() });

            if (isBold)
                runProperties.Append(new Word.Bold());

            run.Append(runProperties);
            run.Append(new Word.Text(text));

            return new Word.Paragraph(run);
        }
    }
}
