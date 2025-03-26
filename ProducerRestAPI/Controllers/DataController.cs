
using Microsoft.AspNetCore.Mvc;
using ProducerRestAPI.Services;

namespace ProducerRestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController(IDataService _dataService) : ControllerBase
    {
        [HttpPost]
        [Route("PostData")]
        public async Task <IActionResult> PostData(string requestData)
        {
            return Ok(await _dataService.SendData(requestData));
        }
    }
}
