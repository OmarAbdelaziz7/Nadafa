using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Factory 
    {
        public int Id { get; set; }

        public required string FactoryName { get; set; }
        public required string Email { get; set; }
        public required string HashedPassword { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }
    }
}
