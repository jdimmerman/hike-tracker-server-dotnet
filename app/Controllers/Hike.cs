using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using app.Models;
using app.Services;
using Newtonsoft.Json;

namespace app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HikeController : ControllerBase
    {
        private readonly ILogger<HikeController> _logger;

        public HikeController(ILogger<HikeController> logger)
        {
            _logger = logger;
        }

        private static List<Hike> hikes = new List<Hike>{
            new Hike
            {
                Id = 1,
                Name = "Franconia Ridge",
                DistanceFromBostonHours = 2.9,
                HikeDistanceMiles = 10.1,
            }
        };

        [HttpGet]
        public ActionResult<List<Hike>> Get()
        {
            return Ok(hikes);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<List<Hike>> Get(int id)
        {
            Hike hike = hikes.Find(item => item.Id == id);
            if (hike == null) {
                return NotFound($"No hike with id {id}");
            }
            return Ok(hike);
        }

        [HttpPost]
        public ActionResult<Hike> Post(Hike hike)
        {
            if (hike.Id != 0)
            {
                return BadRequest("Cannot post ID when creating a new Hike");
            }
            int newId = hikes.OrderByDescending(h => h.Id).First().Id + 1;
            hike.Id = newId;
            HikeQueue.Instance.EnqueueAsync(JsonConvert.SerializeObject(hike)).Wait();
            return Accepted(hike);
        }

        [HttpPut]
        public ActionResult<Hike> Put(Hike hike)
        {
            Hike matchingHike = hikes.Find(h => h.Id == hike.Id);
            if (matchingHike == null) {
                return NotFound($"No hike with id {hike.Id}");
            }
            matchingHike.Name = hike.Name;
            matchingHike.DistanceFromBostonHours = hike.DistanceFromBostonHours;
            matchingHike.HikeDistanceMiles = hike.HikeDistanceMiles;
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult<Hike> Delete(int id)
        {
            Hike matchingHike = hikes.Find(h => h.Id == id);
            if (matchingHike == null) {
                return NotFound($"No hike with id {id}");
            }
            hikes.Remove(matchingHike);
            return NoContent();
        }
    }
}
