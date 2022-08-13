using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CO5227_J790290_1919281.Data
{
    public class Meal
    {
        [Key]
        public int ID { get; set; } // ID

        [StringLength(30)]
        public string Name { get; set; } // Name
        public string Origin { get; set; } // Origin
        public string Type { get; set; } // Type

        public decimal Price { get; set; } // Price
        public Boolean Active { get; set; } // Active Boolean (True / False or (1 / 0)
        public string ImageDescription { get; set; } // Image Description
        public byte[] ImageData { get; set; } // Image Data

    }
}
