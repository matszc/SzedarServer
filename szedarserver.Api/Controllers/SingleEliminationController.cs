using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using szedarserver.Infrastructure.IServices;

namespace szedarserver.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SingleEliminationController : ControllerBase
    {
        private readonly ISingleEliminationService _singleEliminationService;

        public SingleEliminationController(ISingleEliminationService singleEliminationService)
        {
            _singleEliminationService = singleEliminationService;
        }

        [HttpGet("{id}")]
        public  IActionResult GetSingleEliminationTree(string id)
        {
            var res = _singleEliminationService.GetSingleEliminationTree(Guid.Parse(id));
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }
    }
}