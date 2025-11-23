using Microsoft.EntityFrameworkCore;
using StaffIo.Data.Configs;
using StaffIo.Data.Models;

namespace StaffIo.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Session> Sessions { get; set; }

        public DbSet<Foto> Fotos { get; set; }

        public DbSet<History> Histories { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<User>(new UserConfig());
            modelBuilder.ApplyConfiguration<Account>(new AccountConfig());
            modelBuilder.ApplyConfiguration<Session>(new SessionConfig());
            modelBuilder.ApplyConfiguration<History>(new HistoryConfig());
            modelBuilder.ApplyConfiguration<Foto>(new FotoConfig());
        }
    }
}
