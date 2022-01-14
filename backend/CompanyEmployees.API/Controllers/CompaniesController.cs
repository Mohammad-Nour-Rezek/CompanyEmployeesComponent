using AutoMapper;
using CompanyEmployees.API.ModelBinders;
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
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public CompaniesController(ILoggerManager loggerManager, IRepositoryManager repositoryManager, IMapper mapper)
        {
            _logger = loggerManager;
            _repository = repositoryManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetCompanies()
        {
            var companies = _repository.Company.GetAllCompanies(trackChanges: false);

            var companyDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

            return Ok(companyDto);
        }

        [HttpGet("{id}", Name = "CompanyById")]
        public IActionResult GetCompany(Guid id)
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);

            if (company == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                
                return NotFound();
            }
            else
            {
                var companyDto = _mapper.Map<CompanyDto>(company);

                return Ok(companyDto);
            }
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public IActionResult GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");

                return BadRequest("Parameter ids is null");
            }

            var companyEntities = _repository.Company.GetByIds(ids, trackChanges: false);

            if (ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");

                return NotFound();
            }

            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

            return Ok(companiesToReturn);
        }

        [HttpPost]
        public IActionResult CreateCompany([FromBody] CompanyForCreationDto company)
        {
            if (company == null)
            {
                _logger.LogError("CompanyForCreationDto object sent from client is null.");

                return BadRequest("CompanyForCreationDto object is null");
            }

            var companyEntity = _mapper.Map<Company>(company);
            
            _repository.Company.CreateCompany(companyEntity);
            _repository.Save();

            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

            return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id }, companyToReturn);
        }

        [HttpPost("collection")]
        public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                _logger.LogError("Company collection sent from client is null.");

                return BadRequest("Company collection is null");
            }

            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);

            foreach (var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }

            _repository.Save();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

            var ids = string.Join(",", companyCollectionToReturn.Select(e => e.Id));

            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCompany(Guid Id)
        {
            var company = _repository.Company.GetCompany(Id, trackChanges: false);

            if (company == null)
            {
                _logger.LogInfo($"Company with id: {Id} doesn't exist in the database.");

                return NotFound();
            }

            _repository.Company.DeleteCompany(company);
            _repository.Save();

            return NoContent();
        }
    }
}
