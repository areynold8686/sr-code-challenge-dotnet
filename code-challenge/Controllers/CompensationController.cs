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
    [Route("api/compensation")]
    public class CompensationController : Controller
    {
        /*
         *  Questions 
         *  1 - Any authentication on the end points?
         *  2 - If authentication is needed, what about authorization? What user roles or privileges? 
         *  3 - Is a requester only allowed to see a certain sub set of the employee repository?
         *  4 - If so, what do we return for someone trying to view an employee's reporting structure that isn't allowed? Return null, error, etc?
         *  5 - If invalid data is submitted, such as a negative salary, how should we respond?
         *  6 - Who has access to the logs? Should we be logging sensative information like employee name and salary?
         * 
         */

        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _compensationService = compensationService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            try
            {              
                _logger.LogDebug($"Received compensation create request for '{compensation.Employee.EmployeeId}");

                // Should run compensation.Employee.EmployeeId through a santization method
                
                // Verify the incoming data is good
                bool isValid = _compensationService.IsDataValid(compensation);
                
                if (isValid)
                {
                    _compensationService.Create(compensation);

                    return CreatedAtRoute("getCompensationByEmployeeId", new { id = compensation.Employee.EmployeeId });
                }
                else
                {
                    _logger.LogDebug($"Invalid data recieved for compensation create request for '{compensation.Employee.EmployeeId}");

                    return StatusCode(400, "Some messaging indicating invalid data was provided");
                }
            }
            catch(Exception ex)
            {
                // Need to take into consideration the possibility the logger is the one who threw the exception so logging to it in the catch might not be best
                _logger.LogDebug($"Exception caught in compensation create request for '{compensation.Employee.EmployeeId}");

                return StatusCode(500, "Some message");
            }        
        }

        [HttpGet("{id}", Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(String id)
        {
            try
            {
                // Should run ID through a santization method

                _logger.LogDebug($"Received compensation get request for '{id}'");

                var compensation = _compensationService.GetByEmployeeId(id);

                if (compensation == null)
                    return NotFound();

                return Ok(compensation);
            }
            catch (Exception ex)
            {
                // Need to take into consideration the possibility the logger is the one who threw the exception so logging to it in the catch might not be best
                _logger.LogDebug($"Exception caught in compensation get request for '{id}");

                return StatusCode(500, "Some message");
            }
        }      
    }
}
