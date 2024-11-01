using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Helpers.Abstract
{
    public interface ICustomFileHelper
    {
        public string ConvertFileToBase64(string fileName, string filePath);
    }
}
