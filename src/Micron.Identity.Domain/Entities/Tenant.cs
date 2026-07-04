using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Micron.Identity.Domain.Common;

namespace Micron.Identity.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int MaxUsers { get; set; } = 0;
        public List<Application> Products { get; set; } = [];
    }
}