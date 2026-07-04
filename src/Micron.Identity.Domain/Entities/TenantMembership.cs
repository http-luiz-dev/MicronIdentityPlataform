using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Micron.Identity.Domain.Common;

namespace Micron.Identity.Domain.Entities
{
    public class TenantMembership : BaseEntity
    {
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public bool IsOwner { get; set; }
    }
}