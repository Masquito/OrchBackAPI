using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orch_back_API.Entities.Configurations
{
    public class MessagesConfiguration : IEntityTypeConfiguration<Messages>
    {
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
