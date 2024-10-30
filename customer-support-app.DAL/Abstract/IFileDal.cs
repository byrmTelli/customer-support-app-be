using customer_support_app.CORE.DataAccess;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.Results.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Abstract
{
    public interface IFileDal: IEntityRepository<FileAttachment>
    {
        Task<bool> UploadFilesAsync(int ticketId, List<IFormFile> files);
    }
}
