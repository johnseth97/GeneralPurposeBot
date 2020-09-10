using GeneralPurposeBot.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralPurposeBot.Services
{
    public class BotDbContext : DbContext
    {
        public BotDbContext(DbContextOptions<BotDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // query with `.Find(serverId, userId)`
            builder.Entity<UserMoney>()
                .HasKey(um => new { um.ServerId, um.UserId });
            builder.Entity<UserItem>()
                .HasKey(um => new { um.ServerId, um.UserId, um.ItemName });
        }

        public DbSet<ServerProperties> ServerProperties { get; set; }
        public DbSet<ServerModule> ServerModules { get; set; }
        public DbSet<AssignableRole> AssignableRoles { get; set; }
        public DbSet<UserMoney> UserMoney { get; set; }
        public DbSet<UserItem> UserItems { get; set; }
    }
}
