using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mab.lib.SimpleMapper.Tests
{
    public class Entity
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Other { get; set; }
        public bool Publish { get; set; }
        public bool? BoolNullable { get; set; }

        public decimal Total { get; set; }

        public double Distance { get; set; }
        public double? Distance2 { get; set; }

        public short ShortDistance { get; set; }
        public short? ShortDistance2 { get; set; }

        public DateTime Date { get; set; }

        public DateTime? DateNullable { get; set; }
        public int? IntNullable { get; set; }
        public decimal? DecimalNullable { get; set; }

        public Entity2 Detail { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<Entity> Children { get; set; }
    }
}
