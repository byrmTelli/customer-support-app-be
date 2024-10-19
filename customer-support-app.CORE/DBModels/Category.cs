using customer_support_app.CORE.DBModels.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.DBModels
{
    public class Category:BaseEntity
    {
        public string Name { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
