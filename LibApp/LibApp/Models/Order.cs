using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibApp.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderedBooks = new HashSet<OrderedBook>();
        }

        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("ID_Client")]
        public long IdClient { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        [Column("ID_Status")]
        public short IdStatus { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [Required]
        [StringLength(255)]
        public string Email { get; set; }
        [Required]
        [StringLength(255)]
        public string Address { get; set; }
        [Required]
        [StringLength(511)]
        public string Comment { get; set; }

        [ForeignKey(nameof(IdClient))]
        [InverseProperty(nameof(Login.Orders))]
        public virtual Login IdClientNavigation { get; set; }
        [ForeignKey(nameof(IdStatus))]
        [InverseProperty(nameof(OrderStatus.Orders))]
        public virtual OrderStatus IdStatusNavigation { get; set; }
        [InverseProperty(nameof(OrderedBook.IdOrderNavigation))]
        public virtual ICollection<OrderedBook> OrderedBooks { get; set; }
    }
}
