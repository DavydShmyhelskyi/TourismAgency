using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Roles
    {
        [Key]
        public int RoleID { get; set; }
        public string RoleName { get; set; }

        // допоміжні поля
        public ICollection<Users> Users { get; set; }
    }
}
