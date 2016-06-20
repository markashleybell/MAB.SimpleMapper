using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data.Entity.Core.Objects;

namespace MAB.SimpleMapper.Test
{
    public class DbInitializer : CreateDatabaseIfNotExists<TestDbContext>
    {
        protected override void Seed(TestDbContext context)
        {
            var childEntities = new List<TestDbChildEntity> {
                new TestDbChildEntity {
                    Name = "CHILD 1"
                },
                new TestDbChildEntity {
                    Name = "CHILD 2"
                }
            };

            context.TestDbChildEntities.AddRange(childEntities);

            context.SaveChanges();

            context.TestDbEntities.AddRange(new List<TestDbEntity> {
                new TestDbEntity {
                    Text1 = "TEST 1 TEXT 1",
                    Text2 = "TEST 1 TEXT 2",
                    Text3 = "TEST 1 TEXT 3",
                    Price = 231.00M,
                    Created = DateTime.Now,
                    Child = childEntities[0]
                },
                new TestDbEntity {
                    Text1 = "TEST 2 TEXT 1",
                    Text2 = "TEST 2 TEXT 2",
                    Text3 = "TEST 2 TEXT 3",
                    Price = 12.00M,
                    Created = DateTime.Now,
                    Child = childEntities[1]
                }
            });

            context.SaveChanges();
        }
    }

    [TestFixture]
    public class IntegrationTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Database.SetInitializer<TestDbContext>(new DbInitializer());

            using(var _db = new TestDbContext())
            {
                if(!_db.Database.Exists())
                    _db.Database.Initialize(false);
            }
        }

        [SetUp]
        public void Setup()
        {
            Mapper.ClearMappings();
        }

        [Test]
        public void Map_Single_Proxy_Object_To_New_Object_By_Convention()
        {
            using (var _db = new TestDbContext())
            {
                var entity = _db.TestDbEntities.Find(1);

                var model = entity.MapTo<TestDbEntityDTO>();

                model.TestDbEntityID.ShouldEqual(1);
                model.Text1.ShouldEqual("TEST 1 TEXT 1");
                model.Price.ShouldEqual(231.00M);
            }
        }

        [Test]
        public void Map_Single_Proxy_Object_To_New_Object_By_Specification()
        {
            Mapper.AddMapping<TestDbEntity, TestDbEntityDTO>((s, d) =>
            {
                d.TestDbEntityID = s.TestDbEntityID;
                d.Text1 = "THIS WAS MAPPED: " + s.Text1;
                d.Price = s.Price + 10M;
            });

            using (var _db = new TestDbContext())
            {
                var entity = _db.TestDbEntities.Find(2);

                var model = entity.MapTo<TestDbEntityDTO>();

                model.TestDbEntityID.ShouldEqual(2);
                model.Text1.ShouldEqual("THIS WAS MAPPED: TEST 2 TEXT 1");
                model.Price.ShouldEqual(22.00M);
            }
        }
    }
}
