using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Security;

namespace OLP.Controllers
{
    [ApiController]
    [Authorize(Roles = "Student")]
    [Route("api/certificates")]
    public sealed class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificates;
        private readonly ICurrentUser _current;

        public CertificateController(
            ICertificateService certificates,
            ICurrentUser current)
        {
            _certificates = certificates;
            _current = current;
        }

        [HttpPost("courses/{courseId:guid}/issue")]
        public async Task<IActionResult> Issue(Guid courseId)
            => Ok(await _certificates.IssueAsync(await _current.ProfileIdAsync(), courseId));

        [HttpGet("{certificateId:guid}")]
        public async Task<IActionResult> Get(Guid certificateId)
            => Ok(await _certificates.GetAsync(await _current.ProfileIdAsync(), certificateId));
    }
}
