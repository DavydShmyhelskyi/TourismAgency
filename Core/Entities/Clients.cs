﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Clients
    {
        [Key]
        public int ClientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int AddressID { get; set; }
        public string Password { get; set; }
        //допоміжні поля
        public Addresses Address { get; set; }
        public ICollection<Orders> Orders { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName} ({Email})";
        }

    }
}
