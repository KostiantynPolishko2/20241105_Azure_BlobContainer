using Microsoft.AspNetCore.Mvc;
using StorageService.PL.Interfaces;
using StorageService.PL.Repositories;
using System.Net;

namespace StorageService.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageServiceController : ControllerBase
    {
        private readonly ILogger<StorageServiceController> logger;
        private IBlobClientRepository blobClientRepository { get; }

        public StorageServiceController(ILogger<StorageServiceController> logger, IBlobClientRepository blobClientRepository)
        {
            this.logger = logger;
            this.blobClientRepository = blobClientRepository;
        }

        [HttpGet("storage-name", Name = "GetStorageName")]
        public ActionResult<string> GetStorageName()
        {
            try
            {
                return blobClientRepository.getStorageName();
            }
            catch (Exception ex)
            {
                return NotFound($"msg: {ex.Message}, code: {404}, {HttpStatusCode.NotFound}");
            }
        }
    }
}
