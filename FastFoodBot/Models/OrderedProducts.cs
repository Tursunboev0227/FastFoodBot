using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodBot.Models
{
    public class OrderedProducts
    {
        public int Id { get; set; }
        public string OrderedStatus { get; set; }
        public double OrderCost { get; set; }
        public string OrdersPayType { get; set; }
    }
}
