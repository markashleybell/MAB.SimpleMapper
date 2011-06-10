using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  mab.lib.SimpleMapper.Test {
    public class Entity {

        public int ID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Publish { get; set; }

        public decimal Total { get; set; }

        public DateTime Date { get; set; }

        public DateTime? DateNullable { get; set; }
        public int? IntNullable { get; set; }
        public decimal? DecimalNullable { get; set; }

        public Entity2 Detail { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<Entity> Children { get; set; }
    }
}
