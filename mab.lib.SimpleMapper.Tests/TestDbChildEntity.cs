﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace mab.lib.SimpleMapper.Tests
{
    public class TestDbChildEntity
    {
        [Key]
        public int TestDbChildEntityID { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
