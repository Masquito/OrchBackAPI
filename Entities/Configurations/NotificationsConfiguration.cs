using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orch_back_API.Entities.Configurations
{
    public class NotificationsConfiguration : IEntityTypeConfiguration<Notifications>
    {
        public void Configure(EntityTypeBuilder<Notifications> eb)
        {
            eb.Property(x => x.SendDate).HasDefaultValueSql("getutcdate()");

            eb.HasOne(c => c.Author)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

        }
    }
}
