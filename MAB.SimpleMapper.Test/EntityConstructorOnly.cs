using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAB.SimpleMapper.Test
{
    public class EntityConstructorOnly
    {
        public int ID { get; private set; }
        public string Email { get; private set; }

        public EntityConstructorOnly(int id, string email)
        {
            ID = id;
            Email = email;
        }
    }
}
