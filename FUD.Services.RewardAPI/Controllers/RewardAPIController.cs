using Microsoft.AspNetCore.Mvc;

namespace FUD.Services.RewardAPI.Controllers
{
    [ApiController]
    [Route("api/reward")]
    public class RewardAPIController : ControllerBase
    {


        public RewardAPIController(ILogger<RewardAPIController> logger)
        {
        }
    }
}
