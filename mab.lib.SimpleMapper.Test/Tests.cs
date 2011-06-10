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
        public void MapsEnumerableTypes()
        {
            var entity = new Entity { 
                ID = 1,
                Title = "Test Title",
                Body = "This is the body text",
                Publish = true,
                Tags = new List<string> {
                    "News",
                    "Update",
                    "Article"
                }
            };

            var model = Mapper.Map<Entity, Model>(entity);

            model.Tags[0].ShouldEqual("News");
            model.Tags[1].ShouldEqual("Update");
            model.Tags[2].ShouldEqual("Article");
        }

        [Test]
        public void MapsComplexTypes()
        {
            var entity = new Entity { 
                ID = 1,
                Title = "Test Title",
                Body = "This is the body text",
                Publish = true,
                Tags = new List<string> {
                    "News",
                    "Update",
                    "Article"
                },
                Children = new List<Entity> {
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
                }
            };

            var model = Mapper.Map<Entity, Model>(entity);

            model.Children[0].Title.ShouldEqual("Test Child 1");
            model.Children[1].Title.ShouldEqual("Test Child 2");
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
        public void MapsRecursively()
        {
            var entity = new Entity { 
                ID = 1,
                Title = "Test Title",
                Body = "This is the body text",
                Publish = true,
                Tags = new List<string> {
                    "News",
                    "Update",
                    "Article"
                },
                Children = new List<Entity> {
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
                }
            };

            var model = Mapper.Map<Entity, Model>(entity);

            model.Children[0].Title.ShouldEqual("Test Child 1");
            model.Children[1].Title.ShouldEqual("Test Child 2");
        }

    }

}
