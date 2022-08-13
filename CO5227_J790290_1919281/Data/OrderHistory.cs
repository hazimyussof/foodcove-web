using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CO5227_J790290_1919281.Data
{
    public class OrderHistory
    {
        [Key, Required]
        public int OrderNo { get; set; }
        [Required]
        public string Email { get; set; }
    }

}