using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orch_back_API.Entities
{

    public class Messages
    {
        public Guid? Id { get; set; }
        public string? Content { get; set; }
        public Guid? DeliveryId { get; set; }
        public Users? Author { get; set; }
        public Guid? AuthorId { get; set; }
        public DateTime? SendDate { get; set; }
    }
}
