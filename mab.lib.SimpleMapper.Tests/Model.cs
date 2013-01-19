using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mab.lib.SimpleMapper.Tests
{
    public class Model
    {
        public int Int { get; set; }
        public int? IntNullable { get; set; }
        public string String { get; set; }
        public string String2 { get; set; }
        public bool Bool { get; set; }
        public bool? BoolNullable { get; set; }
        public decimal Decimal { get; set; }
        public decimal? DecimalNullable { get; set; }
        public double Double { get; set; }
        public double? DoubleNullable { get; set; }
        public short Short { get; set; }
        public short? ShortNullable { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DateNullable { get; set; }
        public Entity2 Entity2 { get; set; }
        public IEnumerable<string> ListOfStrings { get; set; }
        public IEnumerable<Entity> ListOfEntities { get; set; }

        public string ExtraField { get; set; }
    }
}
