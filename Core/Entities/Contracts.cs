using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Contracts
    {
        [Key]
        public int ContractID { get; set; }
        public int OrderID { get; set; }
        public DateTime SigningDate { get; set; }
        public int ContractStatusID { get; set; }

        // допоміжні поля
        public Orders Order { get; set; }
        public ContractStatuses ContractStatus { get; set; }

        public override string ToString()
        {
            string status = ContractStatus != null ? ContractStatus.ContractStatusName : "Unknown Status";

            return $"Contract #{OrderID} | Signing Date: {SigningDate.ToShortDateString()} | Status: {ContractStatusID}";
        }

    }
}
