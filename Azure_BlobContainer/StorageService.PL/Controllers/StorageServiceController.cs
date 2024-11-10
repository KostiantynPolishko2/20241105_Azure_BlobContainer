using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace StorageService.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageServiceController : ControllerBase
    {
        private readonly ILogger<StorageServiceController> logger;
        private readonly BlobServiceClient blobServiceClient;

        public StorageServiceController(ILogger<StorageServiceController> logger, BlobServiceClient blobServiceClient)
        {
            this.logger = logger;
            this.blobServiceClient = blobServiceClient;
        }

        [HttpGet("storage-name", Name = "GetStorageName")]
        public ActionResult<string> GetStorageName()
        {
            return blobServiceClient.AccountName;
        }
    }
}
