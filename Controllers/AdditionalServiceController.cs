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
    [ODataRoutePrefix(nameof(AdditionalService))]
    [ApiVersionNeutral]
    public class AdditionalServiceController : ODataController
    {
        public AdditionalServiceController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<AdditionalServiceViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<AdditionalServiceViewDto> Get()
        {
            return _data
                .AsQueryable()
                .ProjectTo<AdditionalServiceViewDto>(_mapper.ConfigurationProvider);
        }

        [ODataRoute(ApiInfo.IdRoute)]
        [ProducesResponseType(typeof(AdditionalServiceViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<AdditionalServiceViewDto> Get(ushort id)
        {
            return SingleResult.Create(
                _data
                    .AsQueryable()
                    .Where(e => e.Id == id)
                    .ProjectTo<AdditionalServiceViewDto>(_mapper.ConfigurationProvider));
        }

        [ProducesResponseType(typeof(AdditionalServiceViewDto), Status201Created)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        public IActionResult Post([FromBody] AdditionalServiceUpdateDto create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AdditionalService item = _mapper.Map<AdditionalServiceUpdateDto, AdditionalService>(create);
            return Created(_mapper.Map<AdditionalService, AdditionalServiceViewDto>(item));
        }

        [ODataRoute(ApiInfo.IdRoute)]
        [ProducesResponseType(typeof(AdditionalServiceViewDto), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public IActionResult Patch(ushort id, [FromBody] Delta<AdditionalServiceUpdateDto> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AdditionalService item = _data.FirstOrDefault(e => e.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            AdditionalServiceUpdateDto update = _mapper.Map<AdditionalService, AdditionalServiceUpdateDto>(item);
            delta.Patch(update);
            _mapper.Map(update, item);
            return Updated(_mapper.Map<AdditionalService, AdditionalServiceViewDto>(item));
        }

        [ODataRoute(ApiInfo.IdRoute)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public IActionResult Delete(ushort id)
        {
            AdditionalService delete = _data.FirstOrDefault(e => e.Id == id);

            if (delete == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        private readonly List<AdditionalService> _data = new List<AdditionalService>
        {
            new AdditionalService
            {
                Id = 1,
                Name = "additional 1",
                Description = "additional service 1"
            },
            new AdditionalService
            {
                Id = 2,
                Name = "additional 2",
                Description = "additional service 2"
            },
        };
        private readonly IMapper _mapper;
    }
}
