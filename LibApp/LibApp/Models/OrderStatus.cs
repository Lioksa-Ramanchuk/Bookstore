using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibApp.Models
{
    public partial class OrderStatus
    {
        public OrderStatus()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        [Column("ID")]
        public short Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [InverseProperty(nameof(Order.IdStatusNavigation))]
        public virtual ICollection<Order> Orders { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is OrderStatus os &&
                Id == os.Id && Name == os.Name;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }
    }
}
