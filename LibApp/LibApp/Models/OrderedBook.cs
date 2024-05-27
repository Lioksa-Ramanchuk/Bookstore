using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibApp.Models
{
    public partial class OrderedBook
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("ID_Order")]
        public long IdOrder { get; set; }
        [Column("ID_Book")]
        public long IdBook { get; set; }
        public int Count { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        [ForeignKey(nameof(IdBook))]
        [InverseProperty(nameof(Book.OrderedBooks))]
        public virtual Book IdBookNavigation { get; set; }
        [ForeignKey(nameof(IdOrder))]
        [InverseProperty(nameof(Order.OrderedBooks))]
        public virtual Order IdOrderNavigation { get; set; }
    }
}
