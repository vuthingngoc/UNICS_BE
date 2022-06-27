﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using UniCEC.Business.Services.ActivitiesEntitySvc;
using UniCEC.Data.ViewModels.Entities.ActivitiesEntity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UniCEC.API.Controllers
{
    [Route("api/activities-entity")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ActivitiesEntityController : ControllerBase
    {
        private IActivitiesEntityService _activitiesEntityService;
        public ActivitiesEntityController(IActivitiesEntityService activitiesEntityService)
        {
            _activitiesEntityService = activitiesEntityService;
        }

        //---------------------------------------------------------------------------Competition Entity
        //POST api/<CompetitionEntityController>
        //[Authorize(Roles = "Student")]
        //[HttpPost("image")]
        //[SwaggerOperation(Summary = "Add image for Competition Activity")]
        //public async Task<IActionResult> AddEntityForCompetitionActivities([FromBody] ActivitiesEntityInsertModel model)
        //{
        //    try
        //    {
        //        var header = Request.Headers;
        //        if (!header.ContainsKey("Authorization")) return Unauthorized();
        //        string token = header["Authorization"].ToString().Split(" ")[1];


        //        ViewActivitiesEntity result = await _activitiesEntityService.AddActivitiesEntity(model, token);

        //        if (result != null)
        //        {

        //            return Ok(result);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    catch (ArgumentNullException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        return Unauthorized(ex.Message);
        //    }
        //    catch (DbUpdateException)
        //    {
        //        return StatusCode(500, "Internal server exception");
        //    }
        //    catch (SqlException)
        //    {
        //        return StatusCode(500, "Internal server exception");
        //    }
        //}

    }
}
