using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mab.lib.SimpleMapper;
using mab.lib.SimpleMapper.Tests;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var entity = new Entity {
                ID = 1,
                Title = "Test Title",
                Body = "This is the body text",
                Publish = true,
                Date = DateTime.Now,
                DateNullable = DateTime.Now,
                IntNullable = 3,
                DecimalNullable = 22.11M,
                Tags = new List<string> {
                    "News",
                    "Update",
                    "Article"
                },
                Detail = new Entity2 {
                    ID = 2,
                    Title = "Test Detail",
                    Body = "This is the body text"
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

            var entity2 = new Entity2 {
                ID = 1,
                Title = "ENTITY 2",
                Body = "DESCRIPTION OF ENTITY 2"
            };

            //var c = entity.Children;

            //var model = Mapper.Map<Entity,Model>(entity);

            //var child = entity.Children.Where(x => x.ID == 2).FirstOrDefault();

            //var nullModel = Mapper.Map<Entity2,Model2>(child.Detail);

            //var model2 = entity.MapTo<Model>();

            //var childModels = entity.Children.MapToList<Model>();
            ////var childModels = entity.Children.MapToList<Model>(new object[] { 50, "TEST2" });


            //var childModels2 = Mapper.MapList<Entity, Model>(c.ToList());

            //var model3 = new Model {
            //    ID = 7,
            //    Title = "Test Model",
            //    Body = "This should be the new Body"
            //};

            //entity.MapTo(model3);

            //var model4 = new Model();

            //entity.MapTo(model4);

            //Mapper.MapToPrefixed<Entity2, Model>(entity2, model4, null, null, null);

            //var entity3 = new Entity2();

            //Mapper.MapFromPrefixed<Model, Entity2>(model4, entity3, null, null, null);

            Console.WriteLine();
        }
    }
}
