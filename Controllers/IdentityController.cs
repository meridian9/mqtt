using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using webapp.interfaces;
using webapp.models;
using webapp.repos;

namespace webapp.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IPubSub _pubsub;
        private readonly IIdentityRepo _identity;

        public IdentityController(ILogger<IdentityController> logger, IPubSub pubsub, IIdentityRepo identity)
        {
            _logger = logger;
            _pubsub = pubsub;
            _identity = identity;
        }

        [HttpGet]
        public async Task<JsonResult> Get()
        {            
            var r = new { Identity = _identity.All(), DateTime = DateTime.Now };
            await _pubsub.PublishAsync("log/identity", r);

            return new JsonResult(r);
        }
    }
}
