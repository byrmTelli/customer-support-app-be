using customer_support_app.CORE.DataAccess.EntityFramework;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Context.DbContext;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Concrete
{
    public class FileDal:EfEntityRepositoryBase<FileAttachment,AppDbContext>,IFileDal
    {
        private readonly AppDbContext _context;
        private static readonly List<string> AllowedExtensions = new List<string> { ".jpg", ".png", ".pdf" };
        public FileDal(AppDbContext context):base(context)
        {
            _context = context; 
        }
        private string GenerateHashedFileName(string fileName)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var fileExtension = Path.GetExtension(fileName);
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(fileName + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")));
                var hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

                return hashString + fileExtension;
            }
        }
        public async Task<bool> UploadFilesAsync(int ticketId, List<IFormFile> files)
        {
            try
            {
                string uploadPath = Path.Combine("uploads", ticketId.ToString());
                Directory.CreateDirectory(uploadPath);

                var fileAttachments = new List<FileAttachment>();

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {

                        string fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (!AllowedExtensions.Contains(fileExtension))
                        {
                            throw new InvalidOperationException($"file extension is not permitted: {fileExtension}");
                        }

                        string hashedFileName = GenerateHashedFileName(file.FileName);
                        var filePath = Path.Combine(uploadPath, hashedFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var attachment = new FileAttachment
                        {
                            TicketId = ticketId,
                            FileName = hashedFileName,
                            OriginalName = filePath,
                            FileType =file.ContentType,
                            FileSize = file.Length,
                        };

                        fileAttachments.Add(attachment);
                    }
                }


                await _context.FileAttechments.AddRangeAsync(fileAttachments);
                await _context.SaveChangesAsync();


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
