using Microsoft.AspNetCore.Mvc;
using StorageService.PL.Entities;
using StorageService.PL.Infrastructures;
using StorageService.PL.Interfaces;
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
                return NotFound($"Error! msg: {ex.Message}, code: {404}, {HttpStatusCode.NotFound}");
            }
        }

        [HttpGet("container-names", Name = "GetContainerNames")]
        public ActionResult<IEnumerable<string>> GetContainerNames()
        {
            try
            {
                return this.blobClientRepository.getContainerNames().ToArray();
            }
            catch (BlobClientException ex)
            {
                return NotFound($"Error! msg: {ex.Message} {ex.property}, source: {ex.Source}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error! msg: {ex.Message}, details: {ex.InnerException}");
            }
        }

        [HttpGet("blob-items/{containerName}", Name = "GetBlobItems")]
        public ActionResult<IEnumerable<UserBlobItem>> GetBlobItems([FromRoute] string? containerName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(containerName))
                    throw new ArgumentNullException();

                return this.blobClientRepository.getBlobItems(containerName).ToArray();
            }
            catch (BlobClientException ex)
            {
                return NotFound($"Error! msg: {ex.Message} {ex.property}, source: {ex.Source}");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest($"Error! msg: {ex.Message}, details: {ex.InnerException}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error! msg: {ex.Message}, details: {ex.InnerException}");
            }
        }
    }
}
