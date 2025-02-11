using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvroMessage
{
    public  class Order
    {
        public int OrderId { get; set; }
        public int OrderTime { get; set; }
        public string OrderAddress { get; set; }
    }
}
