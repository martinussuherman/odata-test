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
    [ODataRoutePrefix(nameof(Service))]
    [ApiVersionNeutral]
    public class ServiceController : ODataController
    {
        public ServiceController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<ServiceViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<ServiceViewDto> Get()
        {
            return _data
                .AsQueryable()
                .ProjectTo<ServiceViewDto>(_mapper.ConfigurationProvider);
        }

        [ODataRoute(ApiInfo.IdRoute)]
        [ProducesResponseType(typeof(ServiceViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<ServiceViewDto> Get(ushort id)
        {
            return SingleResult.Create(
                _data
                    .AsQueryable()
                    .Where(e => e.Id == id)
                    .ProjectTo<ServiceViewDto>(_mapper.ConfigurationProvider));
        }

        [ProducesResponseType(typeof(ServiceViewDto), Status201Created)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        public IActionResult Post([FromBody] ServiceUpdateDto create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Service item = _mapper.Map<ServiceUpdateDto, Service>(create);
            return Created(_mapper.Map<Service, ServiceViewDto>(item));
        }

        [ODataRoute(ApiInfo.IdRoute)]
        [ProducesResponseType(typeof(ServiceViewDto), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public IActionResult Patch(ushort id, [FromBody] Delta<ServiceUpdateDto> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Service item = _data.FirstOrDefault(e => e.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            ServiceUpdateDto update = _mapper.Map<Service, ServiceUpdateDto>(item);
            delta.Patch(update);
            _mapper.Map(update, item);
            return Updated(_mapper.Map<Service, ServiceViewDto>(item));
        }

        [ODataRoute(ApiInfo.IdRoute)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public IActionResult Delete(ushort id)
        {
            Service delete = _data.FirstOrDefault(e => e.Id == id);

            if (delete == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        private readonly List<Service> _data = new List<Service>
        {
            new Service
            {
                Id = 1,
                Name = "service 1"
            },
            new Service
            {
                Id = 2,
                Name = "service 2"
            },
            new Service
            {
                Id = 3,
                Name = "service 3"
            },
        };
        private readonly IMapper _mapper;
    }
}
