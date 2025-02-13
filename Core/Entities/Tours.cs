using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Tours
    {
        [Key]
        public int TourID { get; set; }
        public string TourName { get; set; }
        public string TourDescription { get; set; }
        public int LocationID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Price { get; set; }
        public int AvailableSeats { get; set; }

        // Navigation properties
        public Locations Location { get; set; }
        public ICollection<Orders> Orders { get; set; }
        public override string ToString()
        {
            return $"{TourName} ({StartDate.ToShortDateString()} - {EndDate.ToShortDateString()})";
        }

    }
}
