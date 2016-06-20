using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace MAB.SimpleMapper.Test
{
    public class TestDbContext : DbContext
    {
        public DbSet<TestDbEntity> TestDbEntities { get; set; }
        public DbSet<TestDbChildEntity> TestDbChildEntities { get; set; }
    }
}
