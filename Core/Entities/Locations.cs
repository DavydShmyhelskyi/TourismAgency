using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Locations
    {
        [Key]
        public int LocationID { get; set; }
        public int CityID { get; set; }
        public string LocotionDescription { get; set; }
        public string LocationName { get; set; }

        //допоміжні поля
        public Cities City { get; set; }
        public ICollection<Tours> Tours { get; set; }

        public override string ToString()
        {
            return $"{LocationName}: {LocotionDescription}";
        }
    }
}
