using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Globalization;

namespace mab.lib.SimpleMapper.Tests
{
    [TestFixture]
    public class Tests
    {
        #region Entities

        private Entity GetTestEntity()
        {
            return new Entity {
                ID = 1,
                Title = "Test Title",
                Body = "This is the body text",
                Publish = true,
                Date = new DateTime(2011, 7, 5),
                DateNullable = new DateTime(2012, 8, 10),
                Total = 12.00M,
                Distance = 12345.12345,
                Distance2 = 342.231,
                ShortDistance = 10,
                ShortDistance2 = 12,
                DecimalNullable = 55.25M,
                BoolNullable = false,
                IntNullable = 5
            };
        }

        private List<Entity> GetTestEntityList()
        {
            return new List<Entity> {
                new Entity { 
                    ID = 2,
                    Title = "Test Child 1",
                    Body = "This is the body text",
                    Publish = true,
                    Date = new DateTime(2011, 2, 1),
                    DateNullable = null,
                    Total = 12.00M,
                    Distance = 1234.1234,
                    Distance2 = 5493.23211,
                    ShortDistance = 10,
                    ShortDistance2 = null,
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
                    Distance = 321.321,
                    Distance2 = null,
                    ShortDistance = 10,
                    ShortDistance2 = null,
                    DecimalNullable = 13.6M,
                    BoolNullable = true,
                    IntNullable = 7,
                    Tags = new List<string> {
                        "News",
                        "Errata"
                    }
                }
            };
        }

        private Entity2 GetTestEntity2()
        {
            return new Entity2 {
                ID = 1,
                Title = "ENTITY 2",
                Body = "DESCRIPTION OF ENTITY 2"
            };
        }

        private Model GetTestModel()
        {
            return new Model {
                ID = 7,
                Title = "Test Model",
                Body = "This should be the new Body"
            };
        }

        #endregion Entities

        [Test] 
        public void Map_A_String()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity());
            model.Title.ShouldEqual("Test Title");
        }

        [Test]
        public void Map_An_Integer()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity());
            model.ID.ShouldEqual(1);
        }

        [Test]
        public void Map_A_Nullable_Integer()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity());
            model.IntNullable.ShouldEqual(5);
        }

        [Test]
        public void Map_A_Decimal()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity());
            model.Total.ShouldEqual(12.00M);
        }

        [Test]
        public void Map_A_Nullable_Decimal()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity());
            model.DecimalNullable.ShouldEqual(55.25M);
        }

        [Test]
        public void Map_A_Short()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity());
            model.ShortDistance.ShouldEqual((short)10);
        }

        [Test]
        public void Map_A_Nullable_Short()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity());
            model.ShortDistance2.ShouldEqual((short)12);

            var model2 = Mapper.Map<Entity, Model>(GetTestEntityList()[0]);
            model2.ShortDistance2.ShouldEqual(null);
        }

        [Test]
        public void Map_A_Double()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity());
            model.Distance.ShouldEqual(12345.12345);
        }

        [Test]
        public void Map_A_Nullable_Double()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity());
            model.Distance2.ShouldEqual(342.231);
        }

        [Test]
        public void Map_A_Boolean()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity());
            model.Publish.ShouldEqual(true);
        }

        [Test]
        public void Map_A_Nullable_Boolean()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity());
            model.BoolNullable.ShouldEqual(false);
        }

        [Test]
        public void Map_A_DateTime()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity());
            model.Date.ShouldEqual(new DateTime(2011, 7, 5));
        }

        [Test]
        public void Map_A_Nullable_DateTime()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity());
            model.DateNullable.ShouldEqual(new DateTime(2012, 8, 10));
        }

        [Test]
        public void Map_A_List()
        {
            var models = Mapper.MapList<Entity, Model>(GetTestEntityList());

            models[0].Title.ShouldEqual("Test Child 1");
            models[1].Title.ShouldEqual("Test Child 2");
        }

        [Test]
        public void Map_To_An_Object_With_Constructor_Parameters()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity(), new object[] { 20, "TEST" });

            model._int.ShouldEqual(20);
            model._string.ShouldEqual("TEST");
        }

        [Test]
        public void Map_A_List_To_A_List_Of_Objects_With_Constructor_Parameters()
        {
            var models = Mapper.MapList<Entity, Model>(GetTestEntityList(), new object[] { 50, "TEST2" });

            models[0]._int.ShouldEqual(50);
            models[0]._string.ShouldEqual("TEST2");
            models[1]._int.ShouldEqual(50);
            models[1]._string.ShouldEqual("TEST2");
        }

        // We don't seem to be able to do this because the method signature is ambiguous
        //[Test]
        //public void Map_To_An_Object_With_Null_Constructor_Parameters()
        //{
        //    var model = ObjectMapper.Map<Entity, Model>(GetTestEntity(), null, new List<String>(), x =>
        //    {
        //        return x;
        //    }, null);

        //    model._int.ShouldEqual(20);
        //    model._string.ShouldEqual("TEST");
        //}

        [Test]
        public void Map_To_An_Object_With_Null_Excludes()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity(), new object[] { 20, "TEST" }, null);

            model._int.ShouldEqual(20);
            model._string.ShouldEqual("TEST");
        }

        [Test]
        public void Map_To_An_Object_With_Null_Processing_Function()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity(), new object[] { 20, "TEST" }, null, null);

            model._int.ShouldEqual(20);
            model._string.ShouldEqual("TEST");
        }

        [Test]
        public void Map_To_An_Object_With_Null_Processing_Excludes()
        {
            var model = Mapper.Map<Entity, Model>(GetTestEntity(), new object[] { 20, "TEST" }, null, x => {
                return x;
            }, null);

            model._int.ShouldEqual(20);
            model._string.ShouldEqual("TEST");
        }

        [Test]
        public void Map_Object_To_Properties_Of_Existing_Object_Prefixed_With_Source_Type_Name()
        {
            var entity = GetTestEntity2();

            var model = new Model();
            
            Mapper.MapToPrefixed<Entity2, Model>(entity, model);

            model.Entity2_ID.ShouldEqual(1);
            model.Entity2_Title.ShouldEqual("ENTITY 2");
            model.Entity2_Body.ShouldEqual("DESCRIPTION OF ENTITY 2");

            model.Title.ShouldEqual(null);
            model.Body.ShouldEqual(null);
        }

        [Test]
        public void Map_Properties_Of_Object_Prefixed_With_Source_Type_Name_To_Existing_Object()
        {
            var model = new Model {
                ID = 1,
                Title = "JKLGAJKAKLGJL",
                Body = ";sdklgj;asklgjd;aksdlgj;aksld;gjlasgj;",
                Entity2_ID = 1,
                Entity2_Title = "ENTITY 2",
                Entity2_Body = "ENTITY 2 BODY"
            };

            var entity2 = new Entity2();

            Mapper.MapFromPrefixed<Model, Entity2>(model, entity2);

            entity2.ID.ShouldEqual(1);
            entity2.Title.ShouldEqual("ENTITY 2");
            entity2.Body.ShouldEqual("ENTITY 2 BODY");
        }

        [Test]
        public void Map_Object_To_Properties_Of_New_Object_Prefixed_With_Source_Type_Name()
        {
            var entity = GetTestEntity2();
            
            var model = Mapper.MapToPrefixed<Entity2, Model>(entity);

            model.Entity2_ID.ShouldEqual(1);
            model.Entity2_Title.ShouldEqual("ENTITY 2");
            model.Entity2_Body.ShouldEqual("DESCRIPTION OF ENTITY 2");

            model.Title.ShouldEqual(null);
            model.Body.ShouldEqual(null);
        }

        [Test]
        public void Map_Properties_Of_Object_Prefixed_With_Source_Type_Name_To_New_Object()
        {
            var model = new Model {
                ID = 1,
                Title = "JKLGAJKAKLGJL",
                Body = ";sdklgj;asklgjd;aksdlgj;aksld;gjlasgj;",
                Entity2_ID = 1,
                Entity2_Title = "ENTITY 2",
                Entity2_Body = "ENTITY 2 BODY"
            };

            var entity2 = Mapper.MapFromPrefixed<Model, Entity2>(model);

            entity2.ID.ShouldEqual(1);
            entity2.Title.ShouldEqual("ENTITY 2");
            entity2.Body.ShouldEqual("ENTITY 2 BODY");
        }

        [Test]
        public void Extension_Map_To_Existing_Object()
        {
            var entity = GetTestEntity();
            var model = GetTestModel();

            model.MapTo(entity);

            entity.ID.ShouldEqual(7);
            entity.Title.ShouldEqual("Test Model");
            entity.Body.ShouldEqual("This should be the new Body");
        }

        [Test]
        public void Extension_Map_To_Existing_Object_Excluding_Properties_By_Regex()
        {
            var entity = GetTestEntity();
            var model = GetTestModel();

            model.MapTo(entity, new List<string> { "^.*ID$" });

            entity.ID.ShouldEqual(1);
            entity.Title.ShouldEqual("Test Model");
            entity.Body.ShouldEqual("This should be the new Body");
        }

        [Test]
        public void Extension_Map_To_Existing_Object_Applying_Processing_Function()
        {
            var entity = GetTestEntity();
            var model = new Model {
                ID = 7,
                Title = "test model",
                Body = "thiS shoulD BE the new Body",
                Other = "this should stay lower case"
            };

            var cultureInfo = CultureInfo.CurrentCulture.TextInfo;

            model.MapTo(entity, new List<string>(), x => {

                if(x != null && x.GetType() == typeof(System.String))
                    return cultureInfo.ToTitleCase(x.ToString());

                return x;

            }, new List<string> { "Other" });

            entity.Title.ShouldEqual("Test Model");
            entity.Body.ShouldEqual("This Should BE The New Body");
            entity.Other.ShouldEqual("this should stay lower case");
        }

        [Test]
        public void Extension_Map_To_New_Object()
        {
            var entity = GetTestEntity();
            var model = entity.MapTo<Model>();

            model.Title.ShouldEqual("Test Title");
            model.Total.ShouldEqual(12.00M);
        }

        [Test]
        public void Extension_Map_To_New_Object_With_Constructor_Params()
        {
            var entity = GetTestEntity();
            var model = entity.MapTo<Model>(new object[] { 20, "TEST" });

            model._int.ShouldEqual(20);
            model._string.ShouldEqual("TEST");
        }

        [Test]
        public void Extension_Map_To_New_Object_Excluding_Properties_By_Regex()
        {
            var entity = GetTestEntity();
            var model = GetTestModel();

            model.MapTo(entity, new List<string> { "^.*ID$" });

            entity.ID.ShouldEqual(1);
            entity.Title.ShouldEqual("Test Model");
            entity.Body.ShouldEqual("This should be the new Body");
        }

        [Test]
        public void Extension_Map_To_New_Object_Applying_Processing_Function()
        {
            var entity = GetTestEntity();
            var model = new Model {
                ID = 7,
                Title = "test model",
                Body = "thiS shoulD BE the new Body",
                Other = "this should stay lower case"
            };

            var cultureInfo = CultureInfo.CurrentCulture.TextInfo;

            model.MapTo(entity, new List<string>(), x => {

                if(x != null && x.GetType() == typeof(System.String))
                    return cultureInfo.ToTitleCase(x.ToString());

                return x;

            }, new List<string> { "Other" });

            entity.Title.ShouldEqual("Test Model");
            entity.Body.ShouldEqual("This Should BE The New Body");
            entity.Other.ShouldEqual("this should stay lower case");
        }

        [Test]
        public void Extension_Map_To_New_List()
        {
            var entities = GetTestEntityList();
            var models = entities.MapToList<Model>();

            models[0].Title.ShouldEqual("Test Child 1");
            models[1].Title.ShouldEqual("Test Child 2");
        }

        [Test]
        public void Extension_Map_To_New_List_With_Constructor_Params()
        {
            var entities = GetTestEntityList();
            var models = entities.MapToList<Model>(new object[] { 50, "TEST2" });

            models[0]._int.ShouldEqual(50);
            models[0]._string.ShouldEqual("TEST2");
            models[1]._int.ShouldEqual(50);
            models[1]._string.ShouldEqual("TEST2");
        }

        [Test]
        public void Extension_Map_To_New_List_Excluding_Properties_By_Regex()
        {
            var entities = GetTestEntityList();

            var models = entities.MapToList<Model>(null, new List<string> { "^.*ID$" });

            models[0].ID.ShouldEqual(0);
            models[0].Title.ShouldEqual("Test Child 1");
            models[0].Body.ShouldEqual("This is the body text");
            models[1].ID.ShouldEqual(0);
            models[1].Title.ShouldEqual("Test Child 2");
            models[1].Body.ShouldEqual("This is the body text");
        }

        [Test]
        public void Extension_Map_To_New_List_Applying_Processing_Function()
        {
            var entities = GetTestEntityList();
            var models = entities.MapToList<Model>(null, new List<string>(), x => {

                if(x != null && x.GetType() == typeof(System.String))
                    return x.ToString().ToUpper();

                return x;

            }, new List<string> { "Body" });

            models[0].Title.ShouldEqual("TEST CHILD 1");
            models[0].Body.ShouldEqual("This is the body text");
            models[1].Title.ShouldEqual("TEST CHILD 2");
            models[1].Body.ShouldEqual("This is the body text");
        }

        [Test]
        public void Extension_Map_Null_Object()
        {
            Entity entity = null;

            var model = entity.MapTo<Model>();

            model.ShouldEqual(null);
        }

        [Test]
        public void Extension_Map_Null_List()
        {
            List<Entity> emptyList = null;

            var models = emptyList.MapToList<Model>();

            models.ShouldEqual(null);
        }

        [Test]
        public void Extension_Map_To_An_Object_With_Null_Constructor_Parameters()
        {
            var model = GetTestEntity().MapTo<Model>(null, new List<String>(), x =>
            {
                return x;
            }, null);

            model.Title.ShouldEqual("Test Title");
        }

        [Test]
        public void Extension_Map_To_An_Object_With_Null_Excludes()
        {
            var model = GetTestEntity().MapTo<Model>(new object[] { 20, "TEST" }, null);

            model.Title.ShouldEqual("Test Title");
        }

        [Test]
        public void Extension_Map_To_An_Object_With_Null_Processing_Function()
        {
            var model = GetTestEntity().MapTo<Model>(new object[] { 20, "TEST" }, null, null);

            model.Title.ShouldEqual("Test Title");
        }

        [Test]
        public void Extension_Map_To_An_Object_With_Null_Processing_Excludes()
        {
            var model = GetTestEntity().MapTo<Model>(new object[] { 20, "TEST" }, null, x =>
            {
                return x;
            }, null);

            model.Title.ShouldEqual("Test Title");
        }

        [Test]
        public void Extension_Map_Object_To_Properties_Of_New_Object_Prefixed_With_Source_Type_Name()
        {
            var entity = GetTestEntity2();

            var model = entity.MapToPrefixed<Model>();

            model.Entity2_ID.ShouldEqual(1);
            model.Entity2_Title.ShouldEqual("ENTITY 2");
            model.Entity2_Body.ShouldEqual("DESCRIPTION OF ENTITY 2");

            model.Title.ShouldEqual(null);
            model.Body.ShouldEqual(null);
        }

        [Test]
        public void Extension_Map_Properties_Of_Object_Prefixed_With_Source_Type_Name_To_New_Object()
        {
            var model = new Model {
                ID = 1,
                Title = "JKLGAJKAKLGJL",
                Body = ";sdklgj;asklgjd;aksdlgj;aksld;gjlasgj;",
                Entity2_ID = 1,
                Entity2_Title = "ENTITY 2",
                Entity2_Body = "ENTITY 2 BODY"
            };

            var entity2 = model.MapFromPrefixed<Entity2>();

            entity2.ID.ShouldEqual(1);
            entity2.Title.ShouldEqual("ENTITY 2");
            entity2.Body.ShouldEqual("ENTITY 2 BODY");
        }

        [Test]
        public void Extension_Map_Object_To_Properties_Of_Existing_Object_Prefixed_With_Source_Type_Name()
        {
            var entity = GetTestEntity2();
            
            var model = new Model();
            
            entity.MapToPrefixed(model, null, null, null);

            model.Entity2_ID.ShouldEqual(1);
            model.Entity2_Title.ShouldEqual("ENTITY 2");
            model.Entity2_Body.ShouldEqual("DESCRIPTION OF ENTITY 2");

            model.Title.ShouldEqual(null);
            model.Body.ShouldEqual(null);
        }

        [Test]
        public void Extension_Map_Properties_Of_Object_Prefixed_With_Source_Type_Name_To_Existing_Object()
        {
            var model = new Model {
                ID = 1,
                Title = "JKLGAJKAKLGJL",
                Body = ";sdklgj;asklgjd;aksdlgj;aksld;gjlasgj;",
                Entity2_ID = 1,
                Entity2_Title = "ENTITY 2",
                Entity2_Body = "ENTITY 2 BODY"
            };

            var entity2 = new Entity2();
            
            model.MapPrefixedTo(entity2, null, null, null);

            entity2.ID.ShouldEqual(1);
            entity2.Title.ShouldEqual("ENTITY 2");
            entity2.Body.ShouldEqual("ENTITY 2 BODY");
        }
    }
}
