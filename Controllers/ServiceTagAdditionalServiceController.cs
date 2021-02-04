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

        // // http://localhost:60096/odata/Stadium(Name='Baz', Country='Germany')
        // [ODataRoute("(Name={name}, Country={country})")]
        // [EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All)]
        // [HttpGet]
        // public IHttpActionResult Get([FromODataUri] string name, [FromODataUri] string country)
        // {
        //   return Ok(new Stadium { Capacity = 2300, Country = country, Name = name, Owner = "FC Zug" });
        // }

        [ODataRoute("(ServiceId={serviceId}, AdditionalServiceId={additionalServiceId})")]
        [ProducesResponseType(typeof(ServiceTagAdditionalServiceDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<ServiceTagAdditionalServiceDto> Get(
            [FromODataUri] ushort serviceId,
            [FromODataUri] ushort additionalServiceId)
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

        // [ODataRoute("serviceId={serviceId}, additionalServiceId={additionalServiceId}")]
        [ODataRoute("(ServiceId={keyServiceId}, AdditionalServiceId={keyAdditionalServiceId})")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public IActionResult Delete(
            [FromODataUri] ushort keyserviceId,
            [FromODataUri] ushort keyadditionalServiceId)
        {
            ServiceTagAdditionalService delete = _data
                .FirstOrDefault(e =>
                    e.ServiceId == keyserviceId &&
                    e.AdditionalServiceId == keyadditionalServiceId);

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
