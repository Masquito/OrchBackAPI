using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orch_back_API.Entities
{
    /// <summary>
    ///   <p> Klasa reprezentująca tabelę w bazie danych.</p>
    ///   <p> Działa to na EntityFramework</p>
    /// </summary>
    public class Messages
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }
        /// <summary>Gets or sets the content.</summary>
        /// <value>The content.</value>
        public string Content { get; set; }
        /// <summary>Gets or sets the devliery identifier.</summary>
        /// <value>The devliery identifier.</value>
        public Guid DevlieryId { get; set; }
        /// <summary>Gets or sets the author.</summary>
        /// <value>The author.</value>
        public Users Author { get; set; }
        /// <summary>Gets or sets the author identifier.</summary>
        /// <value>The author identifier.</value>
        public Guid AuthorId { get; set; }
        /// <summary>Gets or sets the send date.</summary>
        /// <value>The send date.</value>
        public DateTime SendDate { get; set; }
    }
}
