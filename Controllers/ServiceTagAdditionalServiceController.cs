using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace ODataTest
{
    [AllowAnonymous]
    [Produces(ApiInfo.JsonOutput)]
    [ODataRoutePrefix(nameof(ServiceTagAdditionalService))]
    [ApiVersionNeutral]
    public class ServiceTagAdditionalServiceController : ODataController
    {
        public ServiceTagAdditionalServiceController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<ServiceTagAdditionalServiceDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<ServiceTagAdditionalServiceDto> Get()
        {
            return _data
                .AsQueryable()
                .ProjectTo<ServiceTagAdditionalServiceDto>(_mapper.ConfigurationProvider);
        }

        [ODataRoute("({serviceId},{additionalServiceId})")]
        [ProducesResponseType(typeof(ServiceTagAdditionalServiceDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<ServiceTagAdditionalServiceDto> Get(ushort serviceId, ushort additionalServiceId)
        {
            return SingleResult.Create(
                _data
                    .AsQueryable()
                    .Where(e =>
                        e.ServiceId == serviceId &&
                        e.AdditionalServiceId == additionalServiceId)
                    .ProjectTo<ServiceTagAdditionalServiceDto>(_mapper.ConfigurationProvider));
        }

        [ProducesResponseType(typeof(ServiceTagAdditionalServiceDto), Status201Created)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        public IActionResult Post([FromBody] ServiceTagAdditionalServiceDto create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ServiceTagAdditionalService item =
            //     _mapper.Map<ServiceTagAdditionalServiceDto, ServiceTagAdditionalService>(create);
            return Created(create);
        }

        [ODataRoute("({serviceId},{additionalServiceId})")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public IActionResult Delete(ushort serviceId, ushort additionalServiceId)
        {
            ServiceTagAdditionalService delete = _data
                .FirstOrDefault(e =>
                    e.ServiceId == serviceId &&
                    e.AdditionalServiceId == additionalServiceId);

            if (delete == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        private readonly List<ServiceTagAdditionalService> _data = new List<ServiceTagAdditionalService>
        {
            new ServiceTagAdditionalService
            {
                ServiceId = 1,
                AdditionalServiceId = 1,
            },
            new ServiceTagAdditionalService
            {
                ServiceId = 2,
                AdditionalServiceId = 2,
            },
        };
        private readonly IMapper _mapper;
    }
}
