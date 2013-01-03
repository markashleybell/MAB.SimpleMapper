using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mab.lib.SimpleMapper.Tests
{
    public class Model
    {
        public int _int;
        public string _string;

        public Model() 
        { 
        }

        public Model(int intParam, string stringParam)
        {
            _int = intParam;
            _string = stringParam;
        }

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

        public Model2 Detail { get; set; }

        public List<string> Tags { get; set; }

        public List<Entity> Children { get; set; }

        // Properties of another entity which need to be flattened out
        public int Entity2_ID { get; set; }
        public string Entity2_Title { get; set; }
        public string Entity2_Body { get; set; }
    }
}
