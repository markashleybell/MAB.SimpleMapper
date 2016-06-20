using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAB.SimpleMapper.Test
{
    public class TestDbEntityDTO
    {
        public int TestDbEntityID { get; set; }
        public string Text1 { get; set; }
        public decimal Price { get; set; }

        public int? Child_TestDbChildEntityID { get; set; }
        public TestDbChildEntityDTO Child { get; set; }
    }
}
