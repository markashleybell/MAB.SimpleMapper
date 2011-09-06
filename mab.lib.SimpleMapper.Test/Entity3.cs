using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mab.lib.SimpleMapper.Test
{
    public class Entity3
    {
        private decimal _rate;

        public Entity3(decimal rate)
        {
            _rate = rate;
        }

        public int ID { get; set; }
        public string Description { get; set; }
        public decimal Rate
        {
            get { return _rate; }
        }
    }
}