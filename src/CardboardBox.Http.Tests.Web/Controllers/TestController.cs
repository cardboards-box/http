using Microsoft.AspNetCore.Mvc;

namespace CardboardBox.Http.Tests.Web.Controllers;

using Shared;
using IOFile = File;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class TestController : ControllerBase
{
    [HttpGet]
    [ProducesDefaultResponseType(typeof(UserAccount[]))]
    [ProducesResponseType(typeof(FailedResult), 400)]
    public IActionResult Get([FromQuery] int count = 10)
    {
        if (count <= 0)
            return BadRequest(new FailedResult 
            { 
                Code = 400, 
                Message = "Count cannot be less than 0" 
            });

        if (count > 100)
            return BadRequest(new FailedResult
            {
                Code = 400,
                Message = "Count cannot be greater than 100"
            });

        var users = Enumerable.Range(0, count)
            .Select(i => new UserAccount
            {
                UserName = $"User{i}",
                Password = $"Password{i}"
            })
            .ToArray();
        return Ok(users);
    }

    [HttpPost]
    [ProducesDefaultResponseType(typeof(UserAccountResult))]
    public IActionResult Post([FromBody] UserAccount user)
    {
        return Ok(new UserAccountResult 
        { 
            Message = "Hello world!", 
            User = user 
        });
    }

    [HttpPut]
    [ProducesDefaultResponseType(typeof(UserAccountResult))]
    public IActionResult Put([FromBody] UserAccount user, [FromQuery] string? message = null)
    {
        return Ok(new UserAccountResult
        {
            Message = message ?? "How are you?",
            User = user
        });
    }

    [HttpPost]
    [ProducesDefaultResponseType(typeof(FileUploadResult))]
    [DisableRequestSizeLimit, RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<IActionResult> PostFile(IFormFile file)
    {
        var filePath = Path.GetTempFileName();
        using var io = IOFile.Create(filePath);
        await using var stream = file.OpenReadStream();
        await stream.CopyToAsync(io);

        return Ok(new FileUploadResult
        {
            FileName = filePath,
            Bytes = file.Length
        });
    }

    [HttpGet]
    [ProducesResponseType(typeof(FailedResult), 404)]
    [ProducesResponseType(typeof(FileStreamResult), 200, "application/octet-stream")]
    public IActionResult DownloadFile([FromQuery] string fileName)
    {
        if (!IOFile.Exists(fileName))
            return NotFound(new FailedResult { Code = 404, Message = "File not found" });

        var stream = IOFile.OpenRead(fileName);
        return File(stream, "application/octet-stream");
    }
}
