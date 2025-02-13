using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Statuses
    {
        [Key]
        public int StatusID { get; set; }
        public string StatusName { get; set; }

        // допоміжні поля
        public ICollection<Orders> Orders { get; set; }
    }
}
