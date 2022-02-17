using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{
    public class ReportingStructureService : IReportingStructureService
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<ReportingStructureService> _logger;        

        public ReportingStructureService(ILogger<ReportingStructureService> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        public ReportingStructure GetByEmployeeId(string employeeID)
        {
            ReportingStructure reportingStructure = null;

            if (!String.IsNullOrEmpty(employeeID))
            {
                Employee employee = _employeeService.GetById(employeeID);

                if (employee != null)
                {
                    int numberOfReports = GetNumberOfReportsByEmployee(employee);

                    reportingStructure = new ReportingStructure() { Employee = employee, NumberOfReports = numberOfReports };
                }
            }

            return reportingStructure;
        }

        private int GetNumberOfReportsByEmployee(Employee employee)
        {          
            int numberOfReports = 0;

            if (employee.DirectReports != null && employee.DirectReports.Count > 0)
            {
                foreach (Employee directReportEmployee in employee.DirectReports)
                {
                    // Increment for this direct
                    numberOfReports++;

                    // Recursively check the next level
                    numberOfReports += GetNumberOfReportsByEmployee(directReportEmployee);
                }
            }

            return numberOfReports;
        }
    }
}
