using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CO5227_J790290_1919281.Data
{
    public class CheckoutItems
    {
        [Key, Required]
        public int ID { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        [Required]
        public int Quantity { get; set; }

    }
}
