using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mychelin.Models;

namespace Mychelin.Data
{
    public class MychelinContext : DbContext
    {
        public MychelinContext (DbContextOptions<MychelinContext> options)
            : base(options)
        {
        }

        public DbSet<Mychelin.Models.Person> Person { get; set; } = default!;
        public DbSet<Mychelin.Models.Shoplist> Shoplist { get; set; } = default!;
    }
}
