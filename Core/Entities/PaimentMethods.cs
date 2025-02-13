using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class PaimentMethods
    {
        [Key]
        public int PaimentMethodID { get; set; }
        public string PaimentMethodName { get; set; }

        // допоміжні поля

        public ICollection<Transactions> Transactions { get; set; } 
    }
}
