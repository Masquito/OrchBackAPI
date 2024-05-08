using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orch_back_API.Entities.Configurations
{
    /// <summary>Klasa konfiguracyjna dla Encji Messages dziedzicząca po interfejsie IEntityTypeConfiguration</summary>
    public class MessagesConfiguration : IEntityTypeConfiguration<Messages>
    {
        /// <summary>
        ///   <p>Konfiguracja Encji Messages:</p>
        ///   <p>1. Ustawienie wartości domyślnej na kolumnie SendDate</p>
        ///   <p>2. Ustawienie relacji jeden do wielu na zasadzie, że jeden Autor może mieć wiele wiadomości</p>
        /// </summary>
        /// <param name="eb">Obiekt eb</param>
        public void Configure(EntityTypeBuilder<Messages> eb)
        {
            eb.Property(x => x.SendDate).HasDefaultValueSql("getutcdate()");

            eb.HasOne(c => c.Author)
                .WithMany(x => x.Messes)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

        }
    }
}
