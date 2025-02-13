using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Orders
    {
        [Key]
       public int OrderID { get; set; }
        public int StatusID { get; set; }
        public string OrderDate { get; set; }
        public int TourID { get; set; }
        public int UserID { get; set; } //from class Users
        public int ClientID { get; set; } //from class Clients
        public string OrderDetails { get; set; }
        public int TotalAmount { get; set; }       

        // допоміжні поля

        public Statuses Statuses { get; set; }
        public Tours Tours { get; set; }
        public Users Users { get; set; }
        public Clients Clients { get; set; }
        public ICollection<Transactions> Transactions { get; set; }
        public ICollection<Contracts> Contracts { get; set; }

        public override string ToString()
        {
            return $"OrderID: {OrderID}, Date: {OrderDate}, Amount: {TotalAmount} UAH, StatusID: {StatusID}";
        }


    }
}
