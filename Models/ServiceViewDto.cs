using System;
using System.Collections.Generic;

namespace ODataTest
{
    public class ServiceViewDto
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal SingleSpacingPrice { get; set; }
        public decimal DoubleSpacingPrice { get; set; }
        public bool Inactive { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ushort MinimumOrderQuantity { get; set; }
        public List<AdditionalServiceViewDto> AdditionalServices { get; set; }
    }
}
