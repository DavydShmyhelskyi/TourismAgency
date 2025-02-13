using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UserInterface
{
    public class SeedData
    {
        public void ParseCountries()
        {
            string filePath = @"D:\\Study\\C#_2\\TourismAgency\\UserInterface\\OtherStaff\\countries.txt";

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Файл не знайдено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var context = new TAContext())
                {
                    var existingCountries = context.Countries.ToDictionary(c => c.CountryName, c => c.CountryID);
                    int addedCountries = 0;

                    foreach (var line in File.ReadLines(filePath))
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                            continue;

                        var parts = line.Split('\t');

                        if (parts.Length >= 6)
                        {
                            string countryName = parts[4];

                            if (!existingCountries.ContainsKey(countryName))
                            {
                                var country = new Countries { CountryName = countryName };
                                context.Countries.Add(country);
                                addedCountries++;
                            }
                        }
                    }

                    context.SaveChanges();
                    MessageBox.Show($"Додано {addedCountries} країн.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка парсингу країн: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ParseCapitals()
        {
            string filePath = @"D:\\Study\\C#_2\\TourismAgency\\UserInterface\\OtherStaff\\countries.txt";

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Файл не знайдено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var context = new TAContext())
                {
                    var existingCountries = context.Countries.ToDictionary(c => c.CountryName, c => c.CountryID);
                    var existingCities = context.Cities.Select(c => c.CityName).ToHashSet();
                    int addedCapitals = 0;

                    foreach (var line in File.ReadLines(filePath))
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                            continue;

                        var parts = line.Split('\t');

                        if (parts.Length >= 6)
                        {
                            string countryName = parts[4];
                            string capitalName = parts[5];

                            if (existingCountries.TryGetValue(countryName, out int countryID) && !existingCities.Contains(capitalName))
                            {
                                var city = new Cities
                                {
                                    CityName = capitalName,
                                    CountryID = countryID
                                };

                                context.Cities.Add(city);
                                existingCities.Add(capitalName);
                                addedCapitals++;
                            }
                        }
                    }

                    context.SaveChanges();
                    MessageBox.Show($"Додано {addedCapitals} столиць.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка парсингу столиць: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void AddStatuses()
        {
            using (var context = new TAContext())
            {
                if (!context.Statuses.Any()) // Перевіряємо, чи є вже статуси в базі
                {
                    var statuses = new List<Statuses>
            {
                new Statuses { StatusName = "Pending Confirmation" },
                new Statuses { StatusName = "Accepted" },
                new Statuses { StatusName = "Rejected" },
                new Statuses { StatusName = "Pending Payment" }
            };

                    context.Statuses.AddRange(statuses);
                    context.SaveChanges();
                    MessageBox.Show("Статуси замовлень успішно додані!", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Статуси замовлень вже існують у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public void AddPaymentMethods()
        {
            using (var context = new TAContext())
            {
                if (!context.PaimentMethods.Any()) // Перевіряємо, чи є вже методи оплати
                {
                    var paymentMethods = new List<PaimentMethods>
            {
                new PaimentMethods { PaimentMethodName = "Card" },
                new PaimentMethods { PaimentMethodName = "Cash" }
            };

                    context.PaimentMethods.AddRange(paymentMethods);
                    context.SaveChanges();
                    MessageBox.Show("Методи оплати успішно додані!", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Методи оплати вже існують у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public void AddContractStatuses()
        {
            using (var context = new TAContext())
            {
                if (!context.ContractStatuses.Any()) // Перевіряємо, чи є вже статуси контрактів
                {
                    var contractStatuses = new List<ContractStatuses>
            {
                new ContractStatuses { ContractStatusName = "Active" },
                new ContractStatuses { ContractStatusName = "Terminated" },
                new ContractStatuses { ContractStatusName = "Expired" }
            };

                    context.ContractStatuses.AddRange(contractStatuses);
                    context.SaveChanges();
                    MessageBox.Show("Статуси контрактів успішно додані!", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Статуси вже існують у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public void AddRoles()
        {
            using (var context = new TAContext())
            {
                if (!context.Roles.Any()) // Перевіряємо, чи є вже ролі в базі
                {
                    var roles = new List<Roles>
            {
                new Roles { RoleName = "Owner" },
                new Roles { RoleName = "System Administrator" },
                new Roles { RoleName = "Manager" },
                new Roles { RoleName = "BuferManager" }
            };

                    context.Roles.AddRange(roles);
                    context.SaveChanges();

                    MessageBox.Show("Ролі успішно додані!", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ролі вже існують у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public void AddLocations()
        {
            using (var context = new TAContext())
            {
                if (!context.Locations.Any()) // Перевіряємо, чи є вже локації в базі
                {
                    var locationData = new List<(string CityName, string LocationName, string LocationDescription)>
            {
                ("Paris", "Eiffel Tower", "One of the most famous landmarks in the world."),
                ("Rome", "Colosseum", "Ancient amphitheater and one of the seven wonders."),
                ("London", "Big Ben", "Iconic clock tower in the heart of London."),
                ("New York", "Statue of Liberty", "Symbol of freedom and democracy."),
                ("Beijing", "Great Wall of China", "Historical fortifications stretching thousands of miles.")
            };

                    var locations = new List<Locations>();

                    foreach (var (cityName, locationName, locationDescription) in locationData)
                    {
                        var city = context.Cities.FirstOrDefault(c => c.CityName == cityName);
                        if (city != null)
                        {
                            locations.Add(new Locations
                            {
                                CityID = city.CityID,
                                LocationName = locationName,
                                LocotionDescription = locationDescription
                            });
                        }
                    }

                    if (locations.Any())
                    {
                        context.Locations.AddRange(locations);
                        context.SaveChanges();
                        MessageBox.Show("Локації успішно додані!", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Жодної локації не було додано. Перевірте наявність міст у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Локації вже існують у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public void AddTours()
        {
            using (var context = new TAContext())
            {
                if (!context.Tours.Any()) // Перевіряємо, чи є вже тури в базі
                {
                    var tourData = new List<(string LocationName, string TourName, string TourDescription, DateTime StartDate, DateTime EndDate, int Price, int Seats)>
            {
                ("Eiffel Tower", "Paris Adventure", "Explore the beautiful city of Paris.", DateTime.Parse("2025-06-01"), DateTime.Parse("2025-06-07"), 1200, 15),
                ("Colosseum", "Rome Historical Tour", "Discover ancient Rome.", DateTime.Parse("2025-07-10"), DateTime.Parse("2025-07-17"), 1500, 10),
                ("Big Ben", "London Sightseeing", "Experience the best of London.", DateTime.Parse("2025-08-05"), DateTime.Parse("2025-08-12"), 1300, 20),
                ("Statue of Liberty", "New York City Escape", "Enjoy the iconic sights of NYC.", DateTime.Parse("2025-09-01"), DateTime.Parse("2025-09-07"), 2000, 25),
                ("Great Wall of China", "Great Wall Adventure", "Walk along the Great Wall of China.", DateTime.Parse("2025-10-10"), DateTime.Parse("2025-10-17"), 1800, 12)
            };

                    var tours = new List<Tours>();

                    foreach (var (locationName, tourName, tourDescription, startDate, endDate, price, seats) in tourData)
                    {
                        var location = context.Locations.FirstOrDefault(l => l.LocationName == locationName);
                        if (location != null)
                        {
                            tours.Add(new Tours
                            {
                                LocationID = location.LocationID,
                                TourName = tourName,
                                TourDescription = tourDescription,
                                StartDate = startDate,
                                EndDate = endDate,
                                Price = price,
                                AvailableSeats = seats
                            });
                        }
                    }

                    if (tours.Any())
                    {
                        context.Tours.AddRange(tours);
                        context.SaveChanges();
                        MessageBox.Show("Тури успішно додані!", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Жодного туру не було додано. Перевірте наявність локацій у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Тури вже існують у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public void AddAddresses()
        {
            using (var context = new TAContext())
            {
                if (!context.Addresses.Any()) // Перевіряємо, чи є вже адреси в базі
                {
                    var addressData = new List<(string CityName, string Street, string Building)>
            {
                ("Paris", "Champs-Élysées", "15A"),
                ("Rome", "Via del Corso", "10"),
                ("London", "Baker Street", "221B"),
                ("New York", "Fifth Avenue", "500"),
                ("Beijing", "Wangfujing Street", "88")
            };

                    var addresses = new List<Addresses>();

                    foreach (var (cityName, street, building) in addressData)
                    {
                        var city = context.Cities.FirstOrDefault(c => c.CityName == cityName);
                        if (city != null)
                        {
                            addresses.Add(new Addresses
                            {
                                CityID = city.CityID,
                                Street = street,
                                Building = building
                            });
                        }
                    }

                    if (addresses.Any())
                    {
                        context.Addresses.AddRange(addresses);
                        context.SaveChanges();
                        MessageBox.Show("Адреси успішно додані!", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Жодної адреси не було додано. Перевірте наявність міст у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Адреси вже існують у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public void AddClients()
        {
            using (var context = new TAContext())
            {
                if (!context.Clients.Any()) // Перевіряємо, чи є вже клієнти в базі
                {
                    var clientData = new List<(string FirstName, string LastName, DateTime BirthDate, string Email, string Phone, string CityName, string Street, string Building)>
            {
                ("John", "Doe", DateTime.Parse("1990-05-15"), "john.doe@example.com", "+123456789", "Paris", "Champs-Élysées", "15A"),
                ("Maria", "Rossi", DateTime.Parse("1985-09-22"), "maria.rossi@example.com", "+390123456789", "Rome", "Via del Corso", "10"),
                ("James", "Smith", DateTime.Parse("1992-12-10"), "james.smith@example.com", "+44123456789", "London", "Baker Street", "221B"),
                ("Emily", "Johnson", DateTime.Parse("1988-07-03"), "emily.johnson@example.com", "+12123456789", "New York", "Fifth Avenue", "500"),
                ("Li", "Wei", DateTime.Parse("1995-03-28"), "li.wei@example.com", "+86123456789", "Beijing", "Wangfujing Street", "88")
            };

                    var clients = new List<Clients>();

                    foreach (var (firstName, lastName, birthDate, email, phone, cityName, street, building) in clientData)
                    {
                        var address = context.Addresses.FirstOrDefault(a => a.Street == street && a.Building == building && a.City.CityName == cityName);
                        if (address != null)
                        {
                            clients.Add(new Clients
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                BirthDate = birthDate,
                                Email = email,
                                Phone = phone,
                                AddressID = address.AddressID,
                                Password = "123456789"
                            });
                        }
                    }

                    if (clients.Any())
                    {
                        context.Clients.AddRange(clients);
                        context.SaveChanges();
                        MessageBox.Show("Клієнти успішно додані!", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Жодного клієнта не було додано. Перевірте наявність адрес у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Клієнти вже існують у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public void AddUsers()
        {
            using (var context = new TAContext())
            {
                if (!context.Users.Any()) // Перевіряємо, чи є вже користувачі в базі
                {
                    var userData = new List<(string UserName, string Password, string Email, string FirstName, string LastName, string Phone, string RoleName)>
            {
                ("owner123", "securepass1", "owner@example.com", "John", "Doe", "+123456789", "Owner"),
                ("admin1", "securepass2", "admin1@example.com", "Alice", "Smith", "+987654321", "System Administrator"),
                ("admin2", "securepass3", "admin2@example.com", "Bob", "Johnson", "+456123789", "System Administrator"),
                ("manager1", "securepass4", "manager1@example.com", "Charlie", "Brown", "+741852963", "Manager"),
                ("manager2", "securepass5", "manager2@example.com", "Emma", "Williams", "+369258147", "Manager"),
                ("Bufer", "securepass6", "Bufer", "Bufer", "Bufer", "Bufer", "BuferManager")
            };

                    var users = new List<Users>();

                    foreach (var (userName, password, email, firstName, lastName, phone, roleName) in userData)
                    {
                        var role = context.Roles.FirstOrDefault(r => r.RoleName == roleName);
                        if (role != null)
                        {
                            users.Add(new Users
                            {
                                UserName = userName,
                                Password = password, // У реальному проєкті зберігай хешований пароль!
                                Email = email,
                                FirstName = firstName,
                                LastName = lastName,
                                Phone = phone,
                                RoleID = role.RoleID
                            });
                        }
                    }

                    if (users.Any())
                    {
                        context.Users.AddRange(users);
                        context.SaveChanges();
                        MessageBox.Show("Користувачі успішно додані!", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Жодного користувача не було додано. Перевірте наявність ролей у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Користувачі вже існують у базі.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

    }
}
