using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ContractStatuses
    {
        [Key]
        public int ContractStatusID { get; set; }
        public string ContractStatusName { get; set; }

        // допоміжні поля
        public ICollection<Contracts> Contracts { get; set; }
    }
}
