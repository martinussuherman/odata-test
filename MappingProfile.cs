using System.Linq;
using AutoMapper;

namespace ODataTest
{
    /// <summary>
    /// AutoMapper mapping profile.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Creates AutoMapper mapping profile.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<AdditionalService, AdditionalServiceViewDto>();
            CreateMap<AdditionalServiceUpdateDto, AdditionalService>();
            CreateMap<AdditionalService, AdditionalServiceUpdateDto>();

            CreateMap<Service, ServiceViewDto>()
                .ForMember(
                    dto => dto.AdditionalServices,
                    opt => opt.MapFrom(
                        src => src.ServiceTagAdditionalServices.Select(e => e.AdditionalService)));
            CreateMap<ServiceUpdateDto, Service>();
            CreateMap<Service, ServiceUpdateDto>();

            CreateMap<ServiceTagAdditionalService, ServiceTagAdditionalServiceDto>();
            CreateMap<ServiceTagAdditionalServiceDto, ServiceTagAdditionalService>();
        }
    }
}
