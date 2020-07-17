using System;
using Microsoft.EntityFrameworkCore;

namespace HashedPassword.Data
{
    public class HashedPasswordDbContext : DbContext
    {
        public HashedPasswordDbContext(DbContextOptions<HashedPasswordDbContext> options) : base(options)
        {
        }
        public DbSet<Account> Accounts { get; set; }
    }
}


