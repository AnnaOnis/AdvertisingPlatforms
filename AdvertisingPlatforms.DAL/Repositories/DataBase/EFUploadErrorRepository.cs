using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;

namespace AdvertisingPlatforms.DAL.Repositories.DataBase
{
    public class EFUploadErrorRepository : EFRepository<UploadErrorDb>, IUploadErrorRepository
    {
        public EFUploadErrorRepository(AdvertisingPlatformsDbContext dbContext) : base(dbContext) { }
    }
}
