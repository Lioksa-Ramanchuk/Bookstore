using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibApp.Models
{
    public partial class Book
    {
        public Book()
        {
            Carts = new HashSet<Cart>();
            Favourites = new HashSet<Favourite>();
            OrderedBooks = new HashSet<OrderedBook>();
        }

        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        [Required]
        [StringLength(255)]
        public string Author { get; set; }
        public short Year { get; set; }
        public short Pages { get; set; }
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        [Required]
        [StringLength(511)]
        public string Tags { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        public int Count { get; set; }
        [Required]
        [StringLength(255)]
        public string Publisher { get; set; }
        [Required]
        [StringLength(255)]
        public string Image { get; set; }
        [Required]
        [Column("ISBN")]
        [StringLength(20)]
        public string Isbn { get; set; }

        [InverseProperty(nameof(Cart.IdBookNavigation))]
        public virtual ICollection<Cart> Carts { get; set; }
        [InverseProperty(nameof(Favourite.IdBookNavigation))]
        public virtual ICollection<Favourite> Favourites { get; set; }
        [InverseProperty(nameof(OrderedBook.IdBookNavigation))]
        public virtual ICollection<OrderedBook> OrderedBooks { get; set; }
    }
}
