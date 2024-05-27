using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibApp.Models
{
    public partial class Login
    {
        public Login()
        {
            Carts = new HashSet<Cart>();
            Favourites = new HashSet<Favourite>();
            Orders = new HashSet<Order>();
        }

        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [Required]
        public byte[] Password { get; set; }
        [Column("ID_Role")]
        public short IdRole { get; set; }

        [ForeignKey(nameof(IdRole))]
        [InverseProperty(nameof(Role.Logins))]
        public virtual Role IdRoleNavigation { get; set; }
        [InverseProperty(nameof(Cart.IdClientNavigation))]
        public virtual ICollection<Cart> Carts { get; set; }
        [InverseProperty(nameof(Favourite.IdClientNavigation))]
        public virtual ICollection<Favourite> Favourites { get; set; }
        [InverseProperty(nameof(Order.IdClientNavigation))]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
