using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace mab.lib.SimpleMapper.Tests
{
    public class DbInitializer : DropCreateDatabaseIfModelChanges<TestDbContext>
    {
        protected override void Seed(TestDbContext context)
        {
            context.TestDbEntities.Add(new TestDbEntity {
                Text1 = "TEST TEXT 1",
                Text2 = "TEST TEXT 2",
                Text3 = "TEST TEXT 3",
                Price = 251.00M
            });
        }
    }

    [TestFixture]
    public class IntegrationTests
    {
        [SetUp]
        public void Setup()
        {
            Database.SetInitializer<TestDbContext>(new DbInitializer());

            using(var _db = new TestDbContext())
            {
                _db.Database.Initialize(false);
            }
        }

        [Test]
        public void Projection_Only_Selects_Fields_In_DTO()
        {
            using(var _db = new TestDbContext())
            {
                var entity = _db.TestDbEntities.Where(x => x.TestDbEntityID == 1);
            }
        }
    }
}
