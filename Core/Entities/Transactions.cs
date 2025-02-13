using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Transactions
    {
        [Key]
        public int TransactionID { get; set; }
        public int OrderID { get; set; }
        public int TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public int PaimentMethodID { get; set; }

        // допоміжні поля
        public PaimentMethods PaimentMethod { get; set; }
        public Orders Order { get; set; }

    }
}
