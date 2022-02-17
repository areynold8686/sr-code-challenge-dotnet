using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using challenge.Services;
using challenge.Models;

namespace challenge.Controllers
{
    [Route("api/reportingstructure")]
    public class ReportingStructureController : Controller
    {
        /*
         *  Questions 
         *  1 - Any authentication on the end point
         *  2 - If authentication is needed, what about authorization? What user roles or privileges? 
         *  3 - Is a requester only allowed to see a certain sub set of the employee repository?
         *  4 - If so, what do we return for someone trying to view an employee's reporting structure that isn't allowed? Return null, error, etc?
         * 
         */

        private readonly ILogger _logger;
        private readonly IReportingStructureService _reportingStructureService;

        public ReportingStructureController(ILogger<ReportingStructureController> logger, IReportingStructureService reportingStructureService)
        {
            _logger = logger;
            _reportingStructureService = reportingStructureService;
        }

        [HttpGet("{id}", Name = "getByEmployeeId")]
        public IActionResult GetByEmployeeId(String id)
        {
            try
            {
                // Should run ID through a santization method

                _logger.LogDebug($"Received reporting structure get request for '{id}'");

                var reportingStructureService = _reportingStructureService.GetByEmployeeId(id);

                if (reportingStructureService == null)
                {
                    return NotFound();
                }

                return Ok(reportingStructureService);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Some message");
            }
        }
    }
}
