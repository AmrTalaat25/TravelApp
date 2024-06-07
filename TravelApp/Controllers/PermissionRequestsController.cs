using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using TravelApp.Dto;
using TravelApp.Models;
using TravelApp.Models.Services;
using TravelApp.Models.Services.Interfaces;

namespace TravelApp.Controllers
{
    [Route("api/permission-requests")]
    [ApiController]
    public class PermissionRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPermissionRequestService _permissionRequestService;

        public PermissionRequestsController(ApplicationDbContext context, IPermissionRequestService permissionRequestService)
        {
            _context = context;
            _permissionRequestService = permissionRequestService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ResponseModel<PermissionRequestDTO>>> CreatePermissionRequest([FromForm] CreatePermissionRequestDTO request)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")!.Value;
                var result = await _permissionRequestService.CreatePermissionRequest(request, userId);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            else { return BadRequest(); }
        }

        [HttpGet]
        public async Task<ActionResult<ResponseModel<PermissionRequestDTO>>> GetPermissionRequests()
        {
            if (ModelState.IsValid)
            {
                var result = await _permissionRequestService.GetPermissionRequests();
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            else { return BadRequest(); }
        }

        [HttpGet("byId/{requestID}")]
        public async Task<ActionResult<ResponseModel<PermissionRequestDetailedDto>>> GetPermissionRequestById(int requestID)
        {
            var result = await _permissionRequestService.GetPermissionRequestsById(requestID);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("byUserId/{userID}")]
        public async Task<ActionResult<ResponseModel<PermissionRequestDetailedDto>>> GetPermissionRequestsByUser(string userID)
        {
            var result = await _permissionRequestService.GetPermissionRequestsByUser(userID);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("download/{requestID}")]
        public async Task<IActionResult> DownloadPermissionRequestDocument(int requestID)
        {
            var response = await _permissionRequestService.GetPermissionRequestDocument(requestID);
            if (response.Success)
            {
                return File(response.Data!, "application/pdf", $"permission_request_{requestID}.pdf");
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("approve/{requestID}")]
        public async Task<ActionResult<ResponseModel<string>>> ApprovePermissionRequest(int requestID)
        {
            if (ModelState.IsValid)
            {
                var result = await _permissionRequestService.ApprovePermissionRequest(requestID);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            else { return BadRequest(); }
        }

        [HttpPost("reject/{requestID}")]
        public async Task<IActionResult> RejectPermissionRequest(int requestID)
        {
            var response = await _permissionRequestService.RejectPermissionRequest(requestID);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("update/{requestID}")]
        public async Task<IActionResult> UpdatePermissionRequest(int requestID, [FromForm] UpdatePermissionRequestDTO request)
        {
            var response = await _permissionRequestService.UpdatePermissionRequest(requestID, request);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
    }
}
