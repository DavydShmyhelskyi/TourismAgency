using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Users
    {
        [Key]
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        //допоміжні поля
        public Roles Role { get; set; }
        public ICollection<Orders> Orders { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName} ({UserName})";
        }
    }
}
