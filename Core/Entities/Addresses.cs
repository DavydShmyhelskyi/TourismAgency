using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Addresses
    {
        [Key]
        public int AddressID { get; set; }
        public int CityID { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }

        //допоміжні поля
        public Cities City { get; set; }
        public ICollection<Clients> Clients { get; set; }
    }
}
