using customer_support_app.DAL.Helpers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Helpers.Concrete
{
    public class CustomFileHelper:ICustomFileHelper
    {
        public string ConvertFileToBase64(string fileName, string filePath)
        {

            // Dosyanın var olup olmadığını kontrol et
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Dosya bulunamadı.", fileName);
            }

            // Dosya uzantısını al
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            // Dosyayı byte dizisine oku
            byte[] fileBytes = File.ReadAllBytes(filePath);

            // Uzantıya göre base64 string oluştur
            string base64String = Convert.ToBase64String(fileBytes);
            string dataUri = extension switch
            {
                ".jpg" or ".jpeg" => $"data:image/jpeg;base64,{base64String}",
                ".png" => $"data:image/png;base64,{base64String}",
                ".pdf" => $"data:application/pdf;base64,{base64String}",
                _ => throw new NotSupportedException($"Dosya türü desteklenmiyor: {extension}")
            };

            return dataUri;
        }
    }
}
