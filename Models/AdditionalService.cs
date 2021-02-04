using System;
using System.Collections.Generic;

namespace ODataTest
{
    public partial class AdditionalService
    {
        public AdditionalService()
        {
            ServiceTagAdditionalServices = new HashSet<ServiceTagAdditionalService>();
        }

        public ushort Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public bool Inactive { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<ServiceTagAdditionalService> ServiceTagAdditionalServices { get; set; }
    }
}
