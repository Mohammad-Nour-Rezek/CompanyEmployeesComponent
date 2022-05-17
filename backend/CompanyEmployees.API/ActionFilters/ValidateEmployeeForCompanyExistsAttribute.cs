using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace CompanyEmployees.API.ActionFilters
{
    public class ValidateEmployeeForCompanyExistsAttribute : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;

        public ValidateEmployeeForCompanyExistsAttribute(IRepositoryManager repositoryManager, ILoggerManager loggerManager)
        {
          _repository = repositoryManager;
          _logger = loggerManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;

            var trackChanges = method is "PUT" or "PATCH";

            var companyId = (Guid)context.ActionArguments["companyId"];

            var company = await _repository.Company.GetCompanyAsync(companyId, false);
            
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                
                context.Result = new NotFoundResult();
                
                return;
            }

            var id = (Guid)context.ActionArguments["id"];
            
            var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            
            if (employee == null) 
            { 
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                
                context.Result = new NotFoundResult();
            
            } 
            else
            { 
                context.HttpContext.Items.Add("employee", employee);
                
                await next();
            }
        }
    }
}
