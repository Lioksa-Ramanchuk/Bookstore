using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibApp.Models
{
    public partial class Favourite
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("ID_Client")]
        public long IdClient { get; set; }
        [Column("ID_Book")]
        public long IdBook { get; set; }

        [ForeignKey(nameof(IdBook))]
        [InverseProperty(nameof(Book.Favourites))]
        public virtual Book IdBookNavigation { get; set; }
        [ForeignKey(nameof(IdClient))]
        [InverseProperty(nameof(Login.Favourites))]
        public virtual Login IdClientNavigation { get; set; }
    }
}
