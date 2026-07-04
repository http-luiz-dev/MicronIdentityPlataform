using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Micron.Identity.Domain.Common;

namespace Micron.Identity.Domain.Entities
{
    public class Application : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}