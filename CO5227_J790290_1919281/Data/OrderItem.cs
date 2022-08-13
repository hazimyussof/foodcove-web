using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CO5227_J790290_1919281.Data
{
    public class OrderItem
    {
        [Required]
        public int OrderNo { get; set; }
        [Required]
        public int StockID { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
