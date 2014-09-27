using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data.Entity.Core.Objects;

namespace mab.lib.SimpleMapper.Tests
{
    public class DbInitializer : CreateDatabaseIfNotExists<TestDbContext>
    {
        protected override void Seed(TestDbContext context)
        {
            context.TestDbEntities.AddRange(new List<TestDbEntity> {
                new TestDbEntity {
                    Text1 = "TEST 1 TEXT 1",
                    Text2 = "TEST 1 TEXT 2",
                    Text3 = "TEST 1 TEXT 3",
                    Price = 231.00M,
                    Created = DateTime.Now
                },
                new TestDbEntity {
                    Text1 = "TEST 2 TEXT 1",
                    Text2 = "TEST 2 TEXT 2",
                    Text3 = "TEST 2 TEXT 3",
                    Price = 12.00M,
                    Created = DateTime.Now
                }
            });

            context.SaveChanges();
        }
    }

    [TestFixture]
    public class IntegrationTests
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
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
        public void Projection_Only_Selects_Fields_In_DTO()
        {
            using(var _db = new TestDbContext())
            {
                var entities = _db.TestDbEntities.OrderBy(x => x.TestDbEntityID).AsQueryable();

                var log = new List<string>();

                _db.Database.Log = s => log.Add(s);

                var dtos = entities.Project<TestDbEntity, TestDbEntityDTO>().ToList();

                var selectSql = log.FirstOrDefault(x => x.ToLower().Contains("select"));

                selectSql.ToLower().IndexOf("text2").ShouldEqual(-1);
                selectSql.ToLower().IndexOf("text3").ShouldEqual(-1);
                selectSql.ToLower().IndexOf("created").ShouldEqual(-1);

                dtos.Count.ShouldEqual(2);
                dtos[0].TestDbEntityID.ShouldEqual(1);
                dtos[0].Text1.ShouldEqual("TEST 1 TEXT 1");
                dtos[0].Price.ShouldEqual(231.00M);
                dtos[1].TestDbEntityID.ShouldEqual(2);
                dtos[1].Text1.ShouldEqual("TEST 2 TEXT 1");
                dtos[1].Price.ShouldEqual(12.00M);
            }
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
