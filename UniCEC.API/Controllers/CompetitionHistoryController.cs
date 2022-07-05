﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCEC.Business.Services.CompetitionHistorySvc;
using UniCEC.Data.ViewModels.Entities.CompetitionHistory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UniCEC.API.Controllers
{
    [Route("api/v1/competition-histories")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CompetitionHistoryController : ControllerBase
    {
        private ICompetitionHistoryService _competitionHistoryService;

        public CompetitionHistoryController(ICompetitionHistoryService competitionHistoryService)
        {
            _competitionHistoryService = competitionHistoryService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all history status of competition")]
        public async Task<IActionResult> GetAllHistoryOfCompetition ([FromQuery(Name = "competitionId")] int CompetitionId, [FromQuery(Name = "clubId")] int ClubId)
        {
            try
            {
                var header = Request.Headers;
                if (!header.ContainsKey("Authorization")) return Unauthorized();
                string token = header["Authorization"].ToString().Split(" ")[1];

                List<ViewCompetitionHistory> result = await _competitionHistoryService.GetAllHistoryOfCompetition(CompetitionId, ClubId, token);
                return Ok(result);
            }
            catch (NullReferenceException)
            {
                return Ok(new List<object>());
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (SqlException)
            {
                return StatusCode(500, "Internal server exception");
            }
        }

    }
}