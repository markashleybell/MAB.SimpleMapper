using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Globalization;

namespace  mab.lib.SimpleMapper.Test {

    [TestFixture]
    public class Tests
    {
        #region Entities

        private Entity _entity = new Entity
        {
            ID = 1,
            Title = "Test Title",
            Body = "This is the body text",
            Publish = true,
            Date = new DateTime(2011, 7, 5),
            DateNullable = new DateTime(2012, 8, 10),
            Total = 12.00M,
            DecimalNullable = 55.25M,
            BoolNullable = false,
            IntNullable = 5
        };

        private List<Entity> _entities = new List<Entity> {
            new Entity { 
                ID = 2,
                Title = "Test Child 1",
                Body = "This is the body text",
                Publish = true,
                Date = new DateTime(2011, 2, 1),
                DateNullable = null,
                Total = 12.00M,
                DecimalNullable = 19.25M,
                BoolNullable = false,
                IntNullable = 5,
                Tags = new List<string> {
                    "News",
                    "Errata"
                }
            },
            new Entity { 
                ID = 3,
                Title = "Test Child 2",
                Body = "This is the body text",
                Publish = false,
                Date = new DateTime(2014, 8, 12),
                DateNullable = null,
                Total = 5.00M,
                DecimalNullable = 13.6M,
                BoolNullable = true,
                IntNullable = 7,
                Tags = new List<string> {
                    "News",
                    "Errata"
                }
            }
        };

        #endregion Entities

        [Test]
        public void Map_A_String()
        {
            var model = Mapper.Map<Entity, Model>(_entity);
            model.Title.ShouldEqual("Test Title");
        }

        [Test]
        public void Map_An_Integer()
        {
            var model = Mapper.Map<Entity, Model>(_entity);
            model.ID.ShouldEqual(1);
        }

        [Test]
        public void Map_A_Nullable_Integer()
        {
            var model = Mapper.Map<Entity, Model>(_entity);
            model.IntNullable.ShouldEqual(5);
        }

        [Test]
        public void Map_A_Decimal()
        {
            var model = Mapper.Map<Entity, Model>(_entity);
            model.Total.ShouldEqual(12.00M);
        }

        [Test]
        public void Map_A_Nullable_Decimal()
        {
            var model = Mapper.Map<Entity, Model>(_entity);
            model.DecimalNullable.ShouldEqual(55.25M);
        }

        [Test]
        public void Map_A_Boolean()
        {
            var model = Mapper.Map<Entity, Model>(_entity);
            model.Publish.ShouldEqual(true);
        }

        [Test]
        public void Map_A_Nullable_Boolean()
        {
            var model = Mapper.Map<Entity, Model>(_entity);
            model.BoolNullable.ShouldEqual(false);
        }

        [Test]
        public void Map_A_DateTime()
        {
            var model = Mapper.Map<Entity, Model>(_entity);
            model.Date.ShouldEqual(new DateTime(2011, 7, 5));
        }

        [Test]
        public void Map_A_Nullable_DateTime()
        {
            var model = Mapper.Map<Entity, Model>(_entity);
            model.DateNullable.ShouldEqual(new DateTime(2012, 8, 10));
        }

        [Test]
        public void Map_A_List()
        {
            var models = Mapper.MapList<Entity, Model>(_entities);

            models[0].Title.ShouldEqual("Test Child 1");
            models[1].Title.ShouldEqual("Test Child 2");
        }

        [Test]
        public void Map_To_An_Object_With_Constructor_Parameters()
        {
            var model = Mapper.Map<Entity, Model>(_entity, new object[] { 20, "TEST" });

            model._int.ShouldEqual(20);
            model._string.ShouldEqual("TEST");
        }

        [Test]
        public void Map_A_List_To_A_List_Of_Objects_With_Constructor_Parameters()
        {
            var models = Mapper.MapList<Entity, Model>(_entities, new object[] { 50, "TEST2" });

            models[0]._int.ShouldEqual(50);
            models[0]._string.ShouldEqual("TEST2");
            models[1]._int.ShouldEqual(50);
            models[1]._string.ShouldEqual("TEST2");
        }

        [Test]
        public void Copy_Properties_From_One_Object_To_Another()
        {
            var entity = new Entity
            {
                ID = 1,
                Title = "Test Title",
                Body = "This is the body text",
                Publish = true,
                Date = new DateTime(2011, 7, 5),
                DateNullable = new DateTime(2012, 8, 10),
                Total = 12.00M,
                DecimalNullable = 55.25M,
                BoolNullable = false,
                IntNullable = 5
            };

            var model = new Model
            {
                ID = 7,
                Title = "Test Model",
                Body = "This should be the new Body"
            };

            Mapper.CopyProperties<Model, Entity>(model, entity);

            entity.ID.ShouldEqual(7);
            entity.Title.ShouldEqual("Test Model");
            entity.Body.ShouldEqual("This should be the new Body");
        }

        [Test]
        public void Copy_Properties_From_One_Object_To_Another_And_Process_Values()
        {
            var entity = new Entity
            {
                ID = 1,
                Title = "test title",
                Body = "this is the body text",
                Other = "TEST",
                Publish = true,
                Date = new DateTime(2011, 7, 5),
                DateNullable = new DateTime(2012, 8, 10),
                Total = 12.00M,
                DecimalNullable = 55.25M,
                BoolNullable = false,
                IntNullable = 5
            };

            var model = new Model
            {
                ID = 7,
                Title = "test model",
                Body = "thiS shoulD BE the new Body",
                Other = "this should stay lower case"
            };

            var cultureInfo = CultureInfo.CurrentCulture.TextInfo;

            Mapper.CopyProperties<Model, Entity>(model, entity, new List<string>(), x =>
            {

                if (x != null && x.GetType() == typeof(System.String))
                    return cultureInfo.ToTitleCase(x.ToString());

                return x;

            }, new List<string> { "Other" });

            entity.Title.ShouldEqual("Test Model");
            entity.Body.ShouldEqual("This Should BE The New Body");
            entity.Other.ShouldEqual("this should stay lower case");
        }

        [Test]
        public void Map_Object_With_MapToExtension()
        {
            var entity = new Entity {
                ID = 1,
                Title = "test title",
                Body = "this is the body text",
                Other = "TEST",
                Publish = true,
                Date = new DateTime(2011, 7, 5),
                DateNullable = new DateTime(2012, 8, 10),
                Total = 12.00M,
                DecimalNullable = 55.25M,
                BoolNullable = false,
                IntNullable = 5
            };

            var model = entity.MapTo<Model>();

            model.Title.ShouldEqual("test title");
            model.Total.ShouldEqual(12.00M);
            model.Other.ShouldEqual("TEST");
        }
    }

}
