using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Globalization;
using System.Reflection;

namespace MAB.SimpleMapper.Test
{
    [TestFixture]
    public class Tests
    {
        private List<Entity> _testEntities;
        private Entity2 _testEntity2;
        private Model _testModel;
        private Entity _testNullEntity;
        private List<Entity> _testNullEntities;

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
                    Enum = EntityEnum.Value3
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
                    Enum = EntityEnum.Value2
                },
                null
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

            _testNullEntity = null;
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
            _testModel.Enum.ShouldEqual(ModelEnum.Value2);
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
            _testModel.Enum.ShouldEqual(ModelEnum.Value1);
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
            model.Enum.ShouldEqual(ModelEnum.Value2);
        }

        [Test]
        public void Map_Single_Object_To_New_Object_By_Specification()
        {
            Mapper.AddMapping<Entity, Model>((s, d) => {
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
            model.Enum.ShouldEqual(ModelEnum.Value1);
        }

        [Test]
        public void Map_Single_Object_To_New_Object_With_Constructor_Parameters_By_Convention()
        {
            var model = _testEntities[1].MapTo<Model3>(100, "TEST");

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
            model.Enum.ShouldEqual(ModelEnum.Value2);

            model.IDNameTest.ShouldEqual("100: TEST");
        }

        [Test]
        public void Map_Single_Object_To_New_Object_With_Constructor_Parameters_By_Specification()
        {
            Mapper.AddMapping<Entity, Model3>((s, d) => {
                d.Int = s.Int;
                d.String = s.String;
                d.String2 = "SHORT VALUE WAS: " + s.Short.ToString();
            });

            var model = _testEntities[0].MapTo<Model3>(100, "TEST");

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
            model.Enum.ShouldEqual(ModelEnum.Value1);

            model.IDNameTest.ShouldEqual("100: TEST");
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
            list[0].Enum.ShouldEqual(ModelEnum.Value3);

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
            list[1].Enum.ShouldEqual(ModelEnum.Value2);
        }

        [Test]
        public void Map_List_Of_Object_To_New_List_Of_Object_By_Specification()
        {
            Mapper.AddMapping<Entity, Model>((s, d) => {
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
            list[0].Enum.ShouldEqual(ModelEnum.Value1);

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
            list[1].Enum.ShouldEqual(ModelEnum.Value1);
        }

        [Test]
        public void Map_List_Of_Object_To_New_List_Of_Object_With_Constructor_Parameters_By_Convention()
        {
            var list = _testEntities.MapToList<Model3>(100, "TEST");

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
            list[0].Enum.ShouldEqual(ModelEnum.Value3);
            list[0].IDNameTest.ShouldEqual("100: TEST");

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
            list[1].Enum.ShouldEqual(ModelEnum.Value2);
            list[1].IDNameTest.ShouldEqual("100: TEST");
        }

        [Test]
        public void Map_List_Of_Object_To_New_List_Of_Object_With_Constructor_Parameters_By_Specification()
        {
            Mapper.AddMapping<Entity, Model3>((s, d) => {
                d.Int = s.Int;
                d.String = s.String;
                d.String2 = "SHORT VALUE WAS: " + s.Short.ToString();
            });

            var list = _testEntities.MapToList<Model3>(100, "TEST");

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
            list[0].Enum.ShouldEqual(ModelEnum.Value1);
            list[0].IDNameTest.ShouldEqual("100: TEST");

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
            list[1].Enum.ShouldEqual(ModelEnum.Value1);
            list[1].IDNameTest.ShouldEqual("100: TEST");
        }

        [Test]
        public void Map_Enumerable_Of_Object_To_New_Enumerable_Of_Object_By_Convention()
        {
            var list = _testEntities.MapToEnumerable<Model>();

            var first = list.First();

            first.Int.ShouldEqual(1);
            first.IntNullable.ShouldEqual(null);
            first.String.ShouldEqual("TEST ENTITY 1");
            first.String2.ShouldEqual(null);
            first.Bool.ShouldEqual(true);
            first.BoolNullable.ShouldEqual(null);
            first.Decimal.ShouldEqual(10.00M);
            first.DecimalNullable.ShouldEqual(null);
            first.Double.ShouldEqual(1234.5678);
            first.DoubleNullable.ShouldEqual(null);
            first.Short.ShouldEqual((short)20);
            first.ShortNullable.ShouldEqual(null);
            first.Date.ShouldEqual(new DateTime(2000, 01, 01));
            first.DateNullable.ShouldEqual(null);
            first.Entity2.ShouldEqual(null);
            first.ListOfStrings.ShouldEqual(null);
            first.ListOfEntities.ShouldEqual(null);
            first.Enum.ShouldEqual(ModelEnum.Value3);

            var second = list.Skip(1).First();

            second.Int.ShouldEqual(2);
            second.IntNullable.ShouldEqual(null);
            second.String.ShouldEqual("TEST ENTITY 2");
            second.String2.ShouldEqual(null);
            second.Bool.ShouldEqual(false);
            second.BoolNullable.ShouldEqual(null);
            second.Decimal.ShouldEqual(20.00M);
            second.DecimalNullable.ShouldEqual(null);
            second.Double.ShouldEqual(9876.5432);
            second.DoubleNullable.ShouldEqual(null);
            second.Short.ShouldEqual((short)30);
            second.ShortNullable.ShouldEqual(null);
            second.Date.ShouldEqual(new DateTime(2000, 01, 02));
            second.DateNullable.ShouldEqual(null);
            second.Entity2.ShouldEqual(null);
            second.ListOfStrings.ShouldEqual(null);
            second.ListOfEntities.ShouldEqual(null);
            second.Enum.ShouldEqual(ModelEnum.Value2);
        }

        [Test]
        public void Map_Enumerable_Of_Object_To_New_Enumerable_Of_Object_By_Specification()
        {
            Mapper.AddMapping<Entity, Model>((s, d) => {
                d.Int = s.Int;
                d.String = s.String;
                d.String2 = "SHORT VALUE WAS: " + s.Short.ToString();
            });

            var list = _testEntities.MapToEnumerable<Model>();

            var first = list.First();

            first.Int.ShouldEqual(1);
            first.IntNullable.ShouldEqual(null);
            first.String.ShouldEqual("TEST ENTITY 1");
            first.String2.ShouldEqual("SHORT VALUE WAS: 20");
            first.Bool.ShouldEqual(false);
            first.BoolNullable.ShouldEqual(null);
            first.Decimal.ShouldEqual(0M);
            first.DecimalNullable.ShouldEqual(null);
            first.Double.ShouldEqual(0);
            first.DoubleNullable.ShouldEqual(null);
            first.Short.ShouldEqual((short)0);
            first.ShortNullable.ShouldEqual(null);
            first.Date.ShouldEqual(DateTime.MinValue);
            first.DateNullable.ShouldEqual(null);
            first.Entity2.ShouldEqual(null);
            first.ListOfStrings.ShouldEqual(null);
            first.ListOfEntities.ShouldEqual(null);
            first.Enum.ShouldEqual(ModelEnum.Value1);

            var second = list.Skip(1).First();

            second.Int.ShouldEqual(2);
            second.IntNullable.ShouldEqual(null);
            second.String.ShouldEqual("TEST ENTITY 2");
            second.String2.ShouldEqual("SHORT VALUE WAS: 30");
            second.Bool.ShouldEqual(false);
            second.BoolNullable.ShouldEqual(null);
            second.Decimal.ShouldEqual(0M);
            second.DecimalNullable.ShouldEqual(null);
            second.Double.ShouldEqual(0);
            second.DoubleNullable.ShouldEqual(null);
            second.Short.ShouldEqual((short)0);
            second.ShortNullable.ShouldEqual(null);
            second.Date.ShouldEqual(DateTime.MinValue);
            second.DateNullable.ShouldEqual(null);
            second.Entity2.ShouldEqual(null);
            second.ListOfStrings.ShouldEqual(null);
            second.ListOfEntities.ShouldEqual(null);
            second.Enum.ShouldEqual(ModelEnum.Value1);
        }

        [Test]
        public void Map_Enumerable_Of_Object_To_New_Enumerable_Of_Object_With_Constructor_Parameters_By_Convention()
        {
            var list = _testEntities.MapToEnumerable<Model3>(100, "TEST");

            var first = list.First();

            first.Int.ShouldEqual(1);
            first.IntNullable.ShouldEqual(null);
            first.String.ShouldEqual("TEST ENTITY 1");
            first.String2.ShouldEqual(null);
            first.Bool.ShouldEqual(true);
            first.BoolNullable.ShouldEqual(null);
            first.Decimal.ShouldEqual(10.00M);
            first.DecimalNullable.ShouldEqual(null);
            first.Double.ShouldEqual(1234.5678);
            first.DoubleNullable.ShouldEqual(null);
            first.Short.ShouldEqual((short)20);
            first.ShortNullable.ShouldEqual(null);
            first.Date.ShouldEqual(new DateTime(2000, 01, 01));
            first.DateNullable.ShouldEqual(null);
            first.Entity2.ShouldEqual(null);
            first.ListOfStrings.ShouldEqual(null);
            first.ListOfEntities.ShouldEqual(null);
            first.Enum.ShouldEqual(ModelEnum.Value3);
            first.IDNameTest.ShouldEqual("100: TEST");

            var second = list.Skip(1).First();

            second.Int.ShouldEqual(2);
            second.IntNullable.ShouldEqual(null);
            second.String.ShouldEqual("TEST ENTITY 2");
            second.String2.ShouldEqual(null);
            second.Bool.ShouldEqual(false);
            second.BoolNullable.ShouldEqual(null);
            second.Decimal.ShouldEqual(20.00M);
            second.DecimalNullable.ShouldEqual(null);
            second.Double.ShouldEqual(9876.5432);
            second.DoubleNullable.ShouldEqual(null);
            second.Short.ShouldEqual((short)30);
            second.ShortNullable.ShouldEqual(null);
            second.Date.ShouldEqual(new DateTime(2000, 01, 02));
            second.DateNullable.ShouldEqual(null);
            second.Entity2.ShouldEqual(null);
            second.ListOfStrings.ShouldEqual(null);
            second.ListOfEntities.ShouldEqual(null);
            second.Enum.ShouldEqual(ModelEnum.Value2);
            second.IDNameTest.ShouldEqual("100: TEST");
        }

        [Test]
        public void Map_Enumerable_Of_Object_To_New_Enumerable_Of_Object_With_Constructor_Parameters_By_Specification()
        {
            Mapper.AddMapping<Entity, Model3>((s, d) => {
                d.Int = s.Int;
                d.String = s.String;
                d.String2 = "SHORT VALUE WAS: " + s.Short.ToString();
            });

            var list = _testEntities.MapToEnumerable<Model3>(100, "TEST");

            var first = list.First();

            first.Int.ShouldEqual(1);
            first.IntNullable.ShouldEqual(null);
            first.String.ShouldEqual("TEST ENTITY 1");
            first.String2.ShouldEqual("SHORT VALUE WAS: 20");
            first.Bool.ShouldEqual(false);
            first.BoolNullable.ShouldEqual(null);
            first.Decimal.ShouldEqual(0M);
            first.DecimalNullable.ShouldEqual(null);
            first.Double.ShouldEqual(0);
            first.DoubleNullable.ShouldEqual(null);
            first.Short.ShouldEqual((short)0);
            first.ShortNullable.ShouldEqual(null);
            first.Date.ShouldEqual(DateTime.MinValue);
            first.DateNullable.ShouldEqual(null);
            first.Entity2.ShouldEqual(null);
            first.ListOfStrings.ShouldEqual(null);
            first.ListOfEntities.ShouldEqual(null);
            first.Enum.ShouldEqual(ModelEnum.Value1);
            first.IDNameTest.ShouldEqual("100: TEST");

            var second = list.Skip(1).First();

            second.Int.ShouldEqual(2);
            second.IntNullable.ShouldEqual(null);
            second.String.ShouldEqual("TEST ENTITY 2");
            second.String2.ShouldEqual("SHORT VALUE WAS: 30");
            second.Bool.ShouldEqual(false);
            second.BoolNullable.ShouldEqual(null);
            second.Decimal.ShouldEqual(0M);
            second.DecimalNullable.ShouldEqual(null);
            second.Double.ShouldEqual(0);
            second.DoubleNullable.ShouldEqual(null);
            second.Short.ShouldEqual((short)0);
            second.ShortNullable.ShouldEqual(null);
            second.Date.ShouldEqual(DateTime.MinValue);
            second.DateNullable.ShouldEqual(null);
            second.Entity2.ShouldEqual(null);
            second.ListOfStrings.ShouldEqual(null);
            second.ListOfEntities.ShouldEqual(null);
            second.Enum.ShouldEqual(ModelEnum.Value1);
            second.IDNameTest.ShouldEqual("100: TEST");
        }

        [Test]
        public void Map_Single_Proxy_Object_To_Existing_Object_By_Convention()
        {
            var proxy = new System.Data.Entity.DynamicProxies.ProxyEntity {
                Int = 20,
                String = "TEST PROXIED ENTITY",
                Short = 100
            };

            proxy.MapTo(_testModel);

            _testModel.Int.ShouldEqual(20);
            _testModel.IntNullable.ShouldEqual(null);
            _testModel.String.ShouldEqual("TEST PROXIED ENTITY");
            _testModel.String2.ShouldEqual(null);
            _testModel.Bool.ShouldEqual(false);
            _testModel.BoolNullable.ShouldEqual(null);
            _testModel.Decimal.ShouldEqual(0M);
            _testModel.DecimalNullable.ShouldEqual(null);
            _testModel.Double.ShouldEqual(0);
            _testModel.DoubleNullable.ShouldEqual(null);
            _testModel.Short.ShouldEqual((short)100);
            _testModel.ShortNullable.ShouldEqual(null);
            _testModel.Date.ShouldEqual(DateTime.MinValue);
            _testModel.DateNullable.ShouldEqual(null);
            _testModel.Entity2.ShouldEqual(null);
            _testModel.ListOfStrings.ShouldEqual(null);
            _testModel.ListOfEntities.ShouldEqual(null);
            _testModel.ExtraField.ShouldEqual("NOT MODIFIED");
            _testModel.Enum.ShouldEqual(ModelEnum.Value1);
        }

        [Test]
        public void Map_Single_Proxy_Object_To_Existing_Object_By_Specification()
        {
            Mapper.AddMapping<Entity, Model>((s, d) => {
                d.Int = (s.Int + 30);
                d.String = "PROXIED: " + s.String;
                d.String2 = "SHORT VALUE WAS: " + s.Short.ToString();
            });

            var proxy = new System.Data.Entity.DynamicProxies.ProxyEntity {
                Int = 20,
                String = "TEST PROXIED ENTITY",
                Short = 100
            };

            proxy.MapTo(_testModel);

            _testModel.Int.ShouldEqual(50);
            _testModel.IntNullable.ShouldEqual(null);
            _testModel.String.ShouldEqual("PROXIED: TEST PROXIED ENTITY");
            _testModel.String2.ShouldEqual("SHORT VALUE WAS: 100");
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
            _testModel.Enum.ShouldEqual(ModelEnum.Value1);
        }

        [Test]
        public void Map_Single_Object_To_Existing_Proxy_Object_By_Convention()
        {
            var proxy = new System.Data.Entity.DynamicProxies.ProxyEntity();

            _testModel.MapTo(proxy);

            proxy.Int.ShouldEqual(10);
            proxy.IntNullable.ShouldEqual(null);
            proxy.String.ShouldEqual("TEST MODEL");
            proxy.String2.ShouldEqual(null);
            proxy.Bool.ShouldEqual(false);
            proxy.BoolNullable.ShouldEqual(null);
            proxy.Decimal.ShouldEqual(0M);
            proxy.DecimalNullable.ShouldEqual(null);
            proxy.Double.ShouldEqual(0);
            proxy.DoubleNullable.ShouldEqual(null);
            proxy.Short.ShouldEqual((short)0);
            proxy.ShortNullable.ShouldEqual(null);
            proxy.Date.ShouldEqual(DateTime.MinValue);
            proxy.DateNullable.ShouldEqual(null);
            proxy.Entity2.ShouldEqual(null);
            proxy.ListOfStrings.ShouldEqual(null);
            proxy.ListOfEntities.ShouldEqual(null);
            proxy.Enum.ShouldEqual(EntityEnum.Value1);
        }

        [Test]
        public void Map_Single_Object_To_Existing_Proxy_Object_By_Specification()
        {
            var proxy = new System.Data.Entity.DynamicProxies.ProxyEntity();

            Mapper.AddMapping<Model, Entity>((s, d) => {
                d.Int = (s.Int + 30);
                d.String = "PROXIED: " + s.String;
                d.String2 = "SHORT VALUE WAS: " + s.Short.ToString();
            });

            _testModel.MapTo(proxy);

            proxy.Int.ShouldEqual(40);
            proxy.IntNullable.ShouldEqual(null);
            proxy.String.ShouldEqual("PROXIED: TEST MODEL");
            proxy.String2.ShouldEqual("SHORT VALUE WAS: 0");
            proxy.Bool.ShouldEqual(false);
            proxy.BoolNullable.ShouldEqual(null);
            proxy.Decimal.ShouldEqual(0M);
            proxy.DecimalNullable.ShouldEqual(null);
            proxy.Double.ShouldEqual(0);
            proxy.DoubleNullable.ShouldEqual(null);
            proxy.Short.ShouldEqual((short)0);
            proxy.ShortNullable.ShouldEqual(null);
            proxy.Date.ShouldEqual(DateTime.MinValue);
            proxy.DateNullable.ShouldEqual(null);
            proxy.Entity2.ShouldEqual(null);
            proxy.ListOfStrings.ShouldEqual(null);
            proxy.ListOfEntities.ShouldEqual(null);
            proxy.Enum.ShouldEqual(EntityEnum.Value1);
        }

        [Test]
        public void Map_NULL_Object_To_Existing_Object()
        {
            _testNullEntity.MapTo(_testModel);

            _testModel.Int.ShouldEqual(10);
            _testModel.String.ShouldEqual("TEST MODEL");
            _testModel.String2.ShouldEqual(null);
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
            _testModel.Enum.ShouldEqual(ModelEnum.Value1);
        }

        [Test]
        public void Map_NULL_Object_To_New_Object()
        {
            var model = _testNullEntity.MapTo<Model>();

            model.ShouldEqual(null);
        }

        [Test]
        public void Map_NULL_Object_To_New_List()
        {
            var model = _testNullEntities.MapToList<Model>();

            model.ShouldEqual(null);
        }

        [Test]
        public void Map_NULL_Object_To_New_Enumerable()
        {
            var model = _testNullEntities.MapToEnumerable<Model>();

            model.ShouldEqual(null);
        }

        [Test]
        public void Map_Single_Object_To_New_Object_With_Private_Setters_By_Convention()
        {
            var entity = new EntityPrivateProperties {
                ID = 123,
                Name = "John Test",
                Email = "john@test.com"
            };

            var model = entity.MapTo<ModelPrivateProperties>();

            model.ID.ShouldEqual(123);
            model.Name.ShouldEqual("John Test");
            model.Email.ShouldEqual("john@test.com");
        }

        [Test]
        public void Map_Single_Object_To_New_Object_With_Private_Setters_By_Specification()
        {
            Mapper.AddMapping<EntityPrivateProperties, ModelPrivateProperties>((s, d) => {
                var destProperties = d.GetType().GetProperties();
                destProperties.First(x => x.Name == "ID").SetValue(d, s.ID, null);
                destProperties.First(x => x.Name == "Name").SetValue(d, s.Name, null);
                destProperties.First(x => x.Name == "Email").SetValue(d, s.Email, null);
            });

            var entity = new EntityPrivateProperties {
                ID = 123,
                Name = "John Test",
                Email = "john@test.com"
            };

            var model = entity.MapTo<ModelPrivateProperties>();

            model.ID.ShouldEqual(123);
            model.Name.ShouldEqual("John Test");
            model.Email.ShouldEqual("john@test.com");
        }

        [Test]
        public void Map_List_Of_Object_To_New_List_Of_Object_With_Private_Setters_By_Convention()
        {
            var entities = new List<EntityPrivateProperties> {
                new EntityPrivateProperties {
                    ID = 123,
                    Name = "John Test",
                    Email = "john@test.com"
                },
                new EntityPrivateProperties {
                    ID = 456,
                    Name = "Jane Test",
                    Email = "jane@test.com"
                },
            };

            var models = entities.MapToList<ModelPrivateProperties>();

            models[0].ID.ShouldEqual(123);
            models[0].Name.ShouldEqual("John Test");
            models[0].Email.ShouldEqual("john@test.com");
            models[1].ID.ShouldEqual(456);
            models[1].Name.ShouldEqual("Jane Test");
            models[1].Email.ShouldEqual("jane@test.com");
        }

        [Test]
        public void Map_List_Of_Object_To_New_List_Of_Object_With_Private_Setters_By_Specification()
        {
            Mapper.AddMapping<EntityPrivateProperties, ModelPrivateProperties>((s, d) => {
                var destProperties = d.GetType().GetProperties();
                destProperties.First(x => x.Name == "ID").SetValue(d, s.ID, null);
                destProperties.First(x => x.Name == "Name").SetValue(d, s.Name, null);
                destProperties.First(x => x.Name == "Email").SetValue(d, s.Email, null);
            });

            var entities = new List<EntityPrivateProperties> {
                new EntityPrivateProperties {
                    ID = 123,
                    Name = "John Test",
                    Email = "john@test.com"
                },
                new EntityPrivateProperties {
                    ID = 456,
                    Name = "Jane Test",
                    Email = "jane@test.com"
                },
            };

            var models = entities.MapToList<ModelPrivateProperties>();

            models[0].ID.ShouldEqual(123);
            models[0].Name.ShouldEqual("John Test");
            models[0].Email.ShouldEqual("john@test.com");
            models[1].ID.ShouldEqual(456);
            models[1].Name.ShouldEqual("Jane Test");
            models[1].Email.ShouldEqual("jane@test.com");
        }

        [Test]
        public void Map_NULL_Object_To_New_Constructor()
        {
            ModelConstructorOnly model = null;

            var entity = model.MapToConstructor<EntityConstructorOnly>();

            model.ShouldEqual(null);
        }

        [Test]
        public void Map_Object_To_New_Constructor_Exact_Arguments()
        {
            var model = new {
                ID = 100,
                Email = "test@test.com"
            };

            var entity = model.MapToConstructor<EntityConstructorOnly>();

            entity.ID.ShouldEqual(100);
            entity.Email.ShouldEqual("test@test.com");
        }

        [Test]
        public void Map_Object_To_New_Constructor_More_Arguments()
        {
            var model = new {
                Name = "TEST NAME",
                ID = 100,
                Email = "test@test.com"
            };

            var entity = model.MapToConstructor<EntityConstructorOnly>();

            entity.ID.ShouldEqual(100);
            entity.Email.ShouldEqual("test@test.com");
        }

        [Test]
        public void Map_Object_To_New_Constructor_Less_Arguments()
        {
            var model = new {
                Email = "test@test.com"
            };

            var entity = model.MapToConstructor<EntityConstructorOnly>();

            entity.ID.ShouldEqual(0);
            entity.Email.ShouldEqual("test@test.com");
        }

        [Test]
        public void Map_Object_To_Internal_Constructor()
        {
            var model = new {
                ID = 100,
                Email = "test@test.com"
            };

            var entity = model.MapToConstructor<EntityInternalConstructor>();

            entity.ID.ShouldEqual(100);
            entity.Email.ShouldEqual("test@test.com");
        }
    }
}
