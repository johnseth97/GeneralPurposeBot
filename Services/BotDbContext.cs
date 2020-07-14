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
        public DbSet<ServerProperties> ServerProperties { get; set; }
        public DbSet<ServerModule> ServerModules { get; set; }
    }
}
