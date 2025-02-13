using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Cities
    {
        [Key]
        public int CityID { get; set; }
        public string CityName { get; set; }
        public int CountryID { get; set; }

        //допоміжні властивості
        public Countries Country { get; set; }
        public ICollection<Addresses> Addresses { get; set; }
        public ICollection<Locations> Locations { get; set; }
    }
}
