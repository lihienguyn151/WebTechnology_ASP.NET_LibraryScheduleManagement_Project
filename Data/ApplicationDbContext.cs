using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuanLyLichTruc.Models;

namespace QuanLyLichTruc.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<QuanLyLichTruc.Models.Canbo> Canbo { get; set; } = default!;
        public DbSet<QuanLyLichTruc.Models.Lichtruc> Lichtruc { get; set; } = default!;
        public DbSet<QuanLyLichTruc.Models.Xeplich> Xeplich { get; set; } = default!;
        public DbSet<QuanLyLichTruc.Models.Ngaytruc> Ngaytruc { get; set; } = default!;
        public DbSet<QuanLyLichTruc.Models.Catruc> Catruc { get; set; } = default!;
        public DbSet<QuanLyLichTruc.Models.Lichsudoi> Lichsudoi { get; set; } = default!;
        public DbSet<QuanLyLichTruc.Models.Lichban> Lichban { get; set; } = default!;
    }
}
