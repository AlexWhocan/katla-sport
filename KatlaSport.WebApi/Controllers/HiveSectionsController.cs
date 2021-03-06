﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using KatlaSport.Services.HiveManagement;
using KatlaSport.WebApi.CustomFilters;
using Microsoft.Web.Http;
using Swashbuckle.Swagger.Annotations;

namespace KatlaSport.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/sections")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [CustomExceptionFilter]
    [SwaggerResponseRemoveDefaults]
    public class HiveSectionsController : ApiController
    {
        private readonly IHiveSectionService _hiveSectionService;

        public HiveSectionsController(IHiveSectionService hiveSectionService)
        {
            _hiveSectionService = hiveSectionService ?? throw new ArgumentNullException(nameof(hiveSectionService));
        }

        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a list of hive sections.", Type = typeof(HiveSectionListItem[]))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHiveSectionsAsync()
        {
            var hiveSections = await _hiveSectionService.GetHiveSectionsAsync();
            return Ok(hiveSections);
        }

        [HttpGet]
        [Route("{hiveSectionId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a hive section.", Type = typeof(HiveSection))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHiveSectionAsync(int hiveSectionId)
        {
            var hiveSection = await _hiveSectionService.GetHiveSectionAsync(hiveSectionId);
            return Ok(hiveSection);
        }

        [HttpPut]
        [Route("{hiveSectionId:int:min(1)}/status/{deletedStatus:bool}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Sets deleted status for an existed hive section.")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> SetStatusAsync([FromUri] int hiveSectionId, [FromUri] bool deletedStatus)
        {
            await _hiveSectionService.SetStatusAsync(hiveSectionId, deletedStatus);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Description = "New hive created.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Incorrect(bad) request coul not be carried out by server.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "The request couldn't be carried out because of a conflict on the server.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error occured.")]
        public async Task<IHttpActionResult> AddHiveSectionAsync([FromBody] UpdateHiveSectionRequest createRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var hiveSection = await _hiveSectionService.CreateHiveSectionAsync(createRequest);
            var location = string.Format("/api/sections/{0}", hiveSection.Id);
            return Created<HiveSection>(location, hiveSection);
        }

        [HttpPut]
        [Route("{id:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Existed hive updated.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Incorrect(bad) request coul not be carried out by server.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "The request couldn't be carried out because of a conflict on the server.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error occured.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "The requested source doesn't exist on the server.")]
        public async Task<IHttpActionResult> UpdateHiveSectionAsync([FromUri] int id, [FromBody] UpdateHiveSectionRequest updateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _hiveSectionService.UpdateHiveSectionAsync(id, updateRequest);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }

        [HttpDelete]
        [Route("{id:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Existed hive deleted.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Incorrect(bad) request coul not be carried out by server.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "The request couldn't be carried out because of a conflict on the server.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error occured.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "The requested source doesn't exist on the server.")]
        public async Task<IHttpActionResult> DeleteHiveSectionAsync([FromUri] int id)
        {
            await _hiveSectionService.DeleteHiveSectionAsync(id);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }
    }
}
