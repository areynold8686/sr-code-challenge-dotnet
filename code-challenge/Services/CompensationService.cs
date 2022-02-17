using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ILogger<CompensationService> logger, IEmployeeService employeeService, ICompensationRepository compensationRepository)
        {
            _compensationRepository = compensationRepository;
            _employeeService = employeeService;
            _logger = logger;
        }

        public Compensation Create(Compensation compensation)
        {
            if(compensation != null)
            {
                _compensationRepository.Add(compensation);
                _compensationRepository.SaveAsync().Wait();
            }

            return compensation;
        }

        public Compensation GetByEmployeeId(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _compensationRepository.GetByEmployeeId(id);
            }

            return null;
        }    
        
        public bool IsDataValid(Compensation compensation)
        {
            bool isValid = true;
            
            Employee employee = _employeeService.GetById(compensation.Employee.EmployeeId);
            
            // Might be needed, depending on requirements
            //bool alreadyExists = _compensationRepository.GetByEmployeeId(compensation.Employee.EmployeeId) != null;

            if (employee == null || compensation.Salary <= 0) // || alreadyExists
            {
                isValid = false;
            }            

            return isValid;
        }
    }
}
 