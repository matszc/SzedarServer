using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace szedarserver.Core.Domain
{
    public abstract class EntityGuid
    {
        [Key] [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; protected set; } = Guid.NewGuid();
    }
}
