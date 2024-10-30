using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.Utilities
{
    public static class ImageHelper
    {
        public static string ConvertImageToBase64String(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return null;

            return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        }
    }
}
