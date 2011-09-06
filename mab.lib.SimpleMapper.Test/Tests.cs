using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace  mab.lib.SimpleMapper.Test {

    [TestFixture]
    public class Tests {

        [Test]
        public void MapsBasicTypes()
        {
            var entity = new Entity { 
                ID = 1,
                Title = "Test Title",
                Body = "This is the body text",
                Publish = true
            };

            var model = Mapper.Map<Entity, Model>(entity);

            model.ID.ShouldEqual(1);
            model.Title.ShouldEqual("Test Title");
            model.Body.ShouldEqual("This is the body text");
            model.Publish.ShouldEqual(true);
        }

        [Test]
        public void MapListWorksCorrectly()
        {
            var entities = new List<Entity> {
                new Entity { 
                    ID = 2,
                    Title = "Test Child 1",
                    Body = "This is the body text",
                    Publish = false,
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
                    Tags = new List<string> {
                        "News",
                        "Errata"
                    }
                }
            };

            var models = Mapper.MapList<Entity, Model>(entities);

            models[0].Title.ShouldEqual("Test Child 1");
            models[1].Title.ShouldEqual("Test Child 2");
        }

        [Test]
        public void MapsTypesWithConstructorParameters()
        {
            var entity = new Entity3(20.50M) {
                ID = 1,
                Description = "Test Entity"
            };

            var model = Mapper.Map<Entity3, Model3>(entity, new object[] { 20.50M });

            model.ID.ShouldEqual(1);
            model.Description.ShouldEqual("Test Entity");
            model.Rate.ShouldEqual(20.50M);
        }

    }

}
