using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseTaskLib
{
    public class CustomerStatistics
    {
        public string Name { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
