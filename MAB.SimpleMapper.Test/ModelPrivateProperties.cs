using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAB.SimpleMapper.Test
{
    public class ModelPrivateProperties
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
    }
}
