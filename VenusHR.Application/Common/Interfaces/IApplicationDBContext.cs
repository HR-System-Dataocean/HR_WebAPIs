using Microsoft.EntityFrameworkCore;
using VenusHR.Core.Master;

namespace VenusHR.Application.Common.Interfaces
{
    public interface IApplicationDBContext
    {
        DbSet<AppSetting> AppSettings { get; set; }
        Task<int> SaveChangesAsync();

    }
}
