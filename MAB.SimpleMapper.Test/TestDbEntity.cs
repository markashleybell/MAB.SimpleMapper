using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MAB.SimpleMapper.Test
{
    public class TestDbEntity
    {
        [Key]
        public int TestDbEntityID { get; set; }
        [Required]
        public string Text1 { get; set; }
        [Required]
        public string Text2 { get; set; }
        [Required]
        public string Text3 { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public DateTime Created { get; set; }

        public int? Child_TestDbChildEntityID { get; set; }
        [ForeignKey("Child_TestDbChildEntityID")]
        public virtual TestDbChildEntity Child { get; set; }
    }
}
