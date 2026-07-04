using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Micron.Identity.Domain.Common;

namespace Micron.Identity.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }
}