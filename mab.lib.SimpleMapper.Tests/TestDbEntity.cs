using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace mab.lib.SimpleMapper.Tests
{
    public class TestDbEntity
    {
        [Key]
        public int TestDbEntityID { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public decimal Price { get; set; }
        public DateTime Created { get; set; }
    }
}
