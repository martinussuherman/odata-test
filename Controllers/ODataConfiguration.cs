using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace ODataTest
{
    /// <summary>
    /// Represents the model configuration.
    /// </summary>
    public class ODataConfiguration : IModelConfiguration
    {
        /// <inheritdoc />
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix)
        {
            builder.ComplexType<AdditionalServiceUpdateDto>();
            EntityTypeConfiguration<AdditionalServiceViewDto> additionalService = builder
                .EntitySet<AdditionalServiceViewDto>(nameof(AdditionalService))
                .EntityType;

            additionalService.HasKey(p => p.Id);
            additionalService
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            builder.ComplexType<ServiceUpdateDto>();
            EntityTypeConfiguration<ServiceViewDto> service = builder
                .EntitySet<ServiceViewDto>(nameof(Service))
                .EntityType;

            service.HasKey(p => p.Id);
            service
                .Expand()
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();

            EntityTypeConfiguration<ServiceTagAdditionalServiceDto> serviceTag = builder
                .EntitySet<ServiceTagAdditionalServiceDto>(nameof(ServiceTagAdditionalService))
                .EntityType;

            serviceTag.HasKey(p => new { p.ServiceId, p.AdditionalServiceId });
            serviceTag
                .Expand()
                .Filter()
                .OrderBy()
                .Page(50, 50)
                .Select();
        }
    }
}
