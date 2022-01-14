using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.API.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetEmployees(Guid companyId)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);

            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");

                return NotFound();
            }
            else
            {
                var employees = _repository.Employee.GetAllEmployees(companyId, trackChanges: false);

                var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);

                return Ok(employeesDto);
            }
        }

        [HttpGet("{id}", Name = "GetEmployeeForCompany")]
        public IActionResult GetEmployee(Guid companyId, Guid id)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);

            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");

                return NotFound();
            }
            
            var employee = _repository.Employee.GetEmployee(companyId, id, trackChanges: false);

            if (employee == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");

                return NotFound();
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employee);

            return Ok(employeeDto);
        }

        [HttpPost]
        public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            if (employee == null)
            {
                _logger.LogError("EmployeeForCreationDto object sent from client is null.");

                return BadRequest("EmployeeForCreationDto object is null");
            }

            var company = _repository.Company.GetCompany(companyId, trackChanges: false);

            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");

                return NotFound();
            }

            var employeeEntity = _mapper.Map<Employee>(employee);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            _repository.Save();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployeeForCompany(Guid CompanyId, Guid Id)
        {
            var company = _repository.Company.GetCompany(CompanyId, trackChanges: false);

            if (company == null)
            {
                _logger.LogInfo($"Company with id: {CompanyId} doesn't exist in the database.");

                return NotFound();
            }

            var employeeForCompany = _repository.Employee.GetEmployee(CompanyId, Id, trackChanges: false);

            if (employeeForCompany == null)
            {
                _logger.LogInfo($"Employee with id: {Id} doesn't exist in the database.");

                return NotFound();
            }

            _repository.Employee.DeleteEmployee(employeeForCompany);
            _repository.Save();

            return NoContent();
        }

    }
}
