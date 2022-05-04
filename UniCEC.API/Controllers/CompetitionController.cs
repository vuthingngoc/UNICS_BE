﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCEC.Business.Services.CompetitionSvc;
using UniCEC.Data.Enum;
using UniCEC.Data.RequestModels;
using UniCEC.Data.ViewModels.Common;
using UniCEC.Data.ViewModels.Entities.Competition;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UniCEC.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CompetitionController : ControllerBase
    {
        private ICompetitionService _competitionService;

        public CompetitionController(ICompetitionService competitionService)
        {
            _competitionService = competitionService;   
        }

        // GET: api/<MemberTakesActivityController>
        [HttpGet]
        [SwaggerOperation(Summary = "Get EVENT or COMPETITION by conditions")]
        public async Task<IActionResult> GetCompOrEve([FromQuery] CompetitionRequestModel request)
        {
            try
            {
                PagingResult<ViewCompetition> result = await _competitionService.GetCompOrEve(request);

                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    //
                    return NotFound();
                }
            }
            catch (NullReferenceException e)
            {
                return NotFound(e.Message);
            }
            catch (SqlException)
            {
                return StatusCode(500, "Internal server exception");
            }
        }

        // GET: api/<MemberTakesActivityController>
        [HttpGet("top3")]
        [SwaggerOperation(Summary = "Get top 3 EVENT or COMPETITION")]
        public async Task<IActionResult> GetTop3CompOrEve([FromQuery] bool? Event, [FromQuery] CompetitionStatus? Status, [FromQuery] bool? Public)
        {
            try
            {
                List<ViewCompetition> result = await _competitionService.GetTop3CompOrEve_Status(Event, Status, Public);

                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    //
                    return NotFound();
                }
            }
            catch (NullReferenceException e)
            {
                return NotFound(e.Message);
            }
            catch (SqlException)
            {
                return StatusCode(500, "Internal server exception");
            }
        }

        // GET api/<CompetitionController>/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get detail of EVENT or COMPETITON by id")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                ViewCompetition result = await _competitionService.GetById(id);
                if (result == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (NullReferenceException e)
            {
                return NotFound(e.Message);
            }
            catch (SqlException)
            {
                return StatusCode(500, "Internal server exception");
            }
        }

        // POST api/<CompetitionController>
        [HttpPost]
        [SwaggerOperation(Summary = "Insert EVENT or COMPETITON, if Event please put value at number-of-group = 0 ")]
        //phải có author student
        public async Task<IActionResult> Insert([FromBody] CompetitionInsertModel model)
        {
            try
            {
                ViewCompetition viewCompetition = await _competitionService.Insert(model);
                if (viewCompetition != null)
                {

                    return Ok(viewCompetition);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Internal Server Exception");
            }
            catch (SqlException)
            {
                return StatusCode(500, "Internal Server Exception");
            }
        }

        // PUT api/<CompetitionController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CompetitionController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
