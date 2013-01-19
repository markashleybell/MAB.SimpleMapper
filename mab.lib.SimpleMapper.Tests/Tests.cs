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
        private List<Entity>_testEntities;
        private Entity2 _testEntity2;
        private Model _testModel;

        [SetUp]
        public void Setup()
        {
            Mapper.ClearMappings();

            _testEntities = new List<Entity> {
                new Entity { 
                    Int = 1,
                    IntNullable = null,
                    String = "TEST ENTITY 1",
                    String2 = null,
                    Bool = true,
                    BoolNullable = null,
                    Decimal = 10.00M,
                    DecimalNullable = null,
                    Double = 1234.5678,
                    DoubleNullable = null,
                    Short = 20,
                    ShortNullable = null,
                    Date = new DateTime(2000, 01, 01),
                    DateNullable = null,
                    Entity2 = null,
                    ListOfStrings = null,
                    ListOfEntities = null,
                },
                new Entity { 
                    Int = 2,
                    IntNullable = null,
                    String = "TEST ENTITY 2",
                    String2 = null,
                    Bool = false,
                    BoolNullable = null,
                    Decimal = 20.00M,
                    DecimalNullable = null,
                    Double = 9876.5432,
                    DoubleNullable = null,
                    Short = 30,
                    ShortNullable = null,
                    Date = new DateTime(2000, 01, 02),
                    DateNullable = null,
                    Entity2 = null,
                    ListOfStrings = null,
                    ListOfEntities = null,
                }
            };
       
            _testEntity2 = new Entity2 {
                ID = 1,
                Title = "ENTITY 2",
                Body = "DESCRIPTION OF ENTITY 2"
            };

            _testModel = new Model {
                Int = 10,
                String = "TEST MODEL",
                ExtraField = "NOT MODIFIED"
            };
        }

        [Test]
        public void Map_Single_Object_To_Existing_Object_By_Convention()
        {
            _testEntities[1].MapTo(_testModel);

            _testModel.Int.ShouldEqual(2);
            _testModel.IntNullable.ShouldEqual(null);
            _testModel.String.ShouldEqual("TEST ENTITY 2");
            _testModel.String2.ShouldEqual(null);
            _testModel.Bool.ShouldEqual(false);
            _testModel.BoolNullable.ShouldEqual(null);
            _testModel.Decimal.ShouldEqual(20.00M);
            _testModel.DecimalNullable.ShouldEqual(null);
            _testModel.Double.ShouldEqual(9876.5432);
            _testModel.DoubleNullable.ShouldEqual(null);
            _testModel.Short.ShouldEqual((short)30);
            _testModel.ShortNullable.ShouldEqual(null);
            _testModel.Date.ShouldEqual(new DateTime(2000, 01, 02));
            _testModel.DateNullable.ShouldEqual(null);
            _testModel.Entity2.ShouldEqual(null);
            _testModel.ListOfStrings.ShouldEqual(null);
            _testModel.ListOfEntities.ShouldEqual(null);
            _testModel.ExtraField.ShouldEqual("NOT MODIFIED");
        }

        [Test]
        public void Map_Single_Object_To_Existing_Object_By_Specification()
        {
            Mapper.AddMapping<Entity, Model>((s, d) => {
                d.Int = s.Int;
                d.String = s.String;
                d.String2 = "SHORT VALUE WAS: " + s.Short.ToString();
            });

            _testEntities[0].MapTo(_testModel);

            _testModel.Int.ShouldEqual(1);
            _testModel.IntNullable.ShouldEqual(null);
            _testModel.String.ShouldEqual("TEST ENTITY 1");
            _testModel.String2.ShouldEqual("SHORT VALUE WAS: 20");
            _testModel.Bool.ShouldEqual(false);
            _testModel.BoolNullable.ShouldEqual(null);
            _testModel.Decimal.ShouldEqual(0M);
            _testModel.DecimalNullable.ShouldEqual(null);
            _testModel.Double.ShouldEqual(0);
            _testModel.DoubleNullable.ShouldEqual(null);
            _testModel.Short.ShouldEqual((short)0);
            _testModel.ShortNullable.ShouldEqual(null);
            _testModel.Date.ShouldEqual(DateTime.MinValue);
            _testModel.DateNullable.ShouldEqual(null);
            _testModel.Entity2.ShouldEqual(null);
            _testModel.ListOfStrings.ShouldEqual(null);
            _testModel.ListOfEntities.ShouldEqual(null);
            _testModel.ExtraField.ShouldEqual("NOT MODIFIED");
        }

        [Test]
        public void Map_Single_Object_To_New_Object_By_Convention()
        {
            var model = _testEntities[1].MapTo<Model>();

            model.Int.ShouldEqual(2);
            model.IntNullable.ShouldEqual(null);
            model.String.ShouldEqual("TEST ENTITY 2");
            model.String2.ShouldEqual(null);
            model.Bool.ShouldEqual(false);
            model.BoolNullable.ShouldEqual(null);
            model.Decimal.ShouldEqual(20.00M);
            model.DecimalNullable.ShouldEqual(null);
            model.Double.ShouldEqual(9876.5432);
            model.DoubleNullable.ShouldEqual(null);
            model.Short.ShouldEqual((short)30);
            model.ShortNullable.ShouldEqual(null);
            model.Date.ShouldEqual(new DateTime(2000, 01, 02));
            model.DateNullable.ShouldEqual(null);
            model.Entity2.ShouldEqual(null);
            model.ListOfStrings.ShouldEqual(null);
            model.ListOfEntities.ShouldEqual(null);
        }

        [Test]
        public void Map_Single_Object_To_New_Object_By_Specification()
        {
            Mapper.AddMapping<Entity, Model>((s, d) =>
            {
                d.Int = s.Int;
                d.String = s.String;
                d.String2 = "SHORT VALUE WAS: " + s.Short.ToString();
            });

            var model = _testEntities[0].MapTo<Model>();

            model.Int.ShouldEqual(1);
            model.IntNullable.ShouldEqual(null);
            model.String.ShouldEqual("TEST ENTITY 1");
            model.String2.ShouldEqual("SHORT VALUE WAS: 20");
            model.Bool.ShouldEqual(false);
            model.BoolNullable.ShouldEqual(null);
            model.Decimal.ShouldEqual(0M);
            model.DecimalNullable.ShouldEqual(null);
            model.Double.ShouldEqual(0);
            model.DoubleNullable.ShouldEqual(null);
            model.Short.ShouldEqual((short)0);
            model.ShortNullable.ShouldEqual(null);
            model.Date.ShouldEqual(DateTime.MinValue);
            model.DateNullable.ShouldEqual(null);
            model.Entity2.ShouldEqual(null);
            model.ListOfStrings.ShouldEqual(null);
            model.ListOfEntities.ShouldEqual(null);
        }

        [Test]
        public void Map_List_Of_Object_To_New_List_Of_Object_By_Convention()
        {
            var list = _testEntities.MapToList<Model>();

            list[0].Int.ShouldEqual(1);
            list[0].IntNullable.ShouldEqual(null);
            list[0].String.ShouldEqual("TEST ENTITY 1");
            list[0].String2.ShouldEqual(null);
            list[0].Bool.ShouldEqual(true);
            list[0].BoolNullable.ShouldEqual(null);
            list[0].Decimal.ShouldEqual(10.00M);
            list[0].DecimalNullable.ShouldEqual(null);
            list[0].Double.ShouldEqual(1234.5678);
            list[0].DoubleNullable.ShouldEqual(null);
            list[0].Short.ShouldEqual((short)20);
            list[0].ShortNullable.ShouldEqual(null);
            list[0].Date.ShouldEqual(new DateTime(2000, 01, 01));
            list[0].DateNullable.ShouldEqual(null);
            list[0].Entity2.ShouldEqual(null);
            list[0].ListOfStrings.ShouldEqual(null);
            list[0].ListOfEntities.ShouldEqual(null);
           
            list[1].Int.ShouldEqual(2);
            list[1].IntNullable.ShouldEqual(null);
            list[1].String.ShouldEqual("TEST ENTITY 2");
            list[1].String2.ShouldEqual(null);
            list[1].Bool.ShouldEqual(false);
            list[1].BoolNullable.ShouldEqual(null);
            list[1].Decimal.ShouldEqual(20.00M);
            list[1].DecimalNullable.ShouldEqual(null);
            list[1].Double.ShouldEqual(9876.5432);
            list[1].DoubleNullable.ShouldEqual(null);
            list[1].Short.ShouldEqual((short)30);
            list[1].ShortNullable.ShouldEqual(null);
            list[1].Date.ShouldEqual(new DateTime(2000, 01, 02));
            list[1].DateNullable.ShouldEqual(null);
            list[1].Entity2.ShouldEqual(null);
            list[1].ListOfStrings.ShouldEqual(null);
            list[1].ListOfEntities.ShouldEqual(null);
        }

        [Test]
        public void Map_List_Of_Object_To_New_List_Of_Object_By_Specification()
        {
            Mapper.AddMapping<Entity, Model>((s, d) =>
            {
                d.Int = s.Int;
                d.String = s.String;
                d.String2 = "SHORT VALUE WAS: " + s.Short.ToString();
            });

            var list = _testEntities.MapToList<Model>();

            list[0].Int.ShouldEqual(1);
            list[0].IntNullable.ShouldEqual(null);
            list[0].String.ShouldEqual("TEST ENTITY 1");
            list[0].String2.ShouldEqual("SHORT VALUE WAS: 20");
            list[0].Bool.ShouldEqual(false);
            list[0].BoolNullable.ShouldEqual(null);
            list[0].Decimal.ShouldEqual(0M);
            list[0].DecimalNullable.ShouldEqual(null);
            list[0].Double.ShouldEqual(0);
            list[0].DoubleNullable.ShouldEqual(null);
            list[0].Short.ShouldEqual((short)0);
            list[0].ShortNullable.ShouldEqual(null);
            list[0].Date.ShouldEqual(DateTime.MinValue);
            list[0].DateNullable.ShouldEqual(null);
            list[0].Entity2.ShouldEqual(null);
            list[0].ListOfStrings.ShouldEqual(null);
            list[0].ListOfEntities.ShouldEqual(null);

            list[1].Int.ShouldEqual(2);
            list[1].IntNullable.ShouldEqual(null);
            list[1].String.ShouldEqual("TEST ENTITY 2");
            list[1].String2.ShouldEqual("SHORT VALUE WAS: 30");
            list[1].Bool.ShouldEqual(false);
            list[1].BoolNullable.ShouldEqual(null);
            list[1].Decimal.ShouldEqual(0M);
            list[1].DecimalNullable.ShouldEqual(null);
            list[1].Double.ShouldEqual(0);
            list[1].DoubleNullable.ShouldEqual(null);
            list[1].Short.ShouldEqual((short)0);
            list[1].ShortNullable.ShouldEqual(null);
            list[1].Date.ShouldEqual(DateTime.MinValue);
            list[1].DateNullable.ShouldEqual(null);
            list[1].Entity2.ShouldEqual(null);
            list[1].ListOfStrings.ShouldEqual(null);
            list[1].ListOfEntities.ShouldEqual(null);
        }
    }
}
