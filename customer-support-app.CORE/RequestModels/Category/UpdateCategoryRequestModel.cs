using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.RequestModels.Category
{
    public class UpdateCategoryRequestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
