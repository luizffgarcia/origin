using Microsoft.EntityFrameworkCore;
using OriginDatabase.Models;

namespace OriginDatabase
{
    public class UserManagementDbContext : DbContext
    {
        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Employer> Employers { get; set; }
    }
}
