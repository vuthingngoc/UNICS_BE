﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCEC.Business.Services.MemberTakesActivitySvc;
using UniCEC.Data.RequestModels;
using UniCEC.Data.ViewModels.Common;
using UniCEC.Data.ViewModels.Entities.MemberTakesActivity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UniCEC.API.Controllers
{
    [Route("api/v1/member-takes-activity")]
    [ApiController]
    [ApiVersion("1.0")]
    public class MemberTakesActivityController : ControllerBase
    {
        private IMemberTakesActivityService _memberTakesActivityService;

        public MemberTakesActivityController(IMemberTakesActivityService memberTakesActivityService)
        {
            _memberTakesActivityService = memberTakesActivityService;
        }

        // GET: api/<MemberTakesActivityController>
        [HttpGet("tasks")]
        [SwaggerOperation(Summary = "Get tasks by conditions, 0.Doing , 1.LateTime, 2.Finished, 3.FinishedLate, 4.Approved, 5.Rejected")]
        public async Task<IActionResult> GetTaskByConditions([FromQuery] MemberTakesActivityRequestModel request)
        {
            try
            {
                PagingResult<ViewMemberTakesActivity> result = await _memberTakesActivityService.GetAllTaskesByConditions(request);
                //
                return Ok(result);
            }
            catch (NullReferenceException)
            {
                return Ok(new List<object>());
            }
            catch (SqlException)
            {
                return StatusCode(500, "Internal server exception");
            }
        }

        // GET api/<MemberTakesActivityController>/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get tasks by Id")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                ViewMemberTakesActivity result = await _memberTakesActivityService.GetByMemberTakesActivityId(id);
                return Ok(result);

            }
            catch (NullReferenceException)
            {
                return Ok(new object());
            }
            catch (SqlException)
            {
                return StatusCode(500, "Internal server exception");
            }
        }

        // POST api/<MemberTakesActivityController>
        [Authorize(Roles = "Student")]
        [HttpPost]
        [SwaggerOperation(Summary = "Insert member in task - Student ")]
        public async Task<IActionResult> InsertMemberTakesActivity([FromBody] MemberTakesActivityInsertModel model)
        {
            try
            {
                //JWT
                var header = Request.Headers;
                if (!header.ContainsKey("Authorization")) return Unauthorized();
                string token = header["Authorization"].ToString().Split(" ")[1];

                ViewMemberTakesActivity result = await _memberTakesActivityService.Insert(model, token);
                if (result != null)
                {

                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Internal server exception");
            }
            catch (SqlException)
            {
                return StatusCode(500, "Internal server exception");
            }
        }


        // PUT api/<MemberTakesActivityController>/5
        [Authorize(Roles = "Student")]
        [HttpPut("submit-activity")]
        [SwaggerOperation(Summary = "Member submit task by Id - Student")]
        public async Task<IActionResult> MemberSubmitTask([FromBody] SubmitMemberTakesActivity model)
        {
            try
            {
                //JWT
                var header = Request.Headers;
                if (!header.ContainsKey("Authorization")) return Unauthorized();
                string token = header["Authorization"].ToString().Split(" ")[1];

                Boolean check = false;
                check = await _memberTakesActivityService.Update(model, token);
                if (check)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Internal server exception");
            }
            catch (SqlException)
            {
                return StatusCode(500, "Internal server exception");
            }
        }


        // PUT api/<MemberTakesActivityController>/5
        [Authorize(Roles = "Student")]
        [HttpPut("confirm-activity")]
        [SwaggerOperation(Summary = "Club Leader is Approved task of member - Student")]
        public async Task<IActionResult> ApprovedOrRejectedTask([FromBody] ConfirmMemberTakesActivity model)
        {
            try
            {
                //JWT
                var header = Request.Headers;
                if (!header.ContainsKey("Authorization")) return Unauthorized();
                string token = header["Authorization"].ToString().Split(" ")[1];

                Boolean check = false;
                check = await _memberTakesActivityService.ApprovedOrRejectedTask(model, token);
                if (check)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Internal server exception");
            }
            catch (SqlException)
            {
                return StatusCode(500, "Internal server exception");
            }
        }

    }
}
