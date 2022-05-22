using Contracts;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateEmployeeForCompany(Guid compnyId, Employee employee)
        {
            employee.CompanyId = compnyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee) =>
            Delete(employee);

        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            var employees = await FindByCondition(c => c.CompanyId.Equals(companyId), trackChanges)
                                  .OrderBy(e => e.Name)
                                  .Skip(count: (employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
                                  .Take(employeeParameters.PageSize)
                                  .ToListAsync();

            var count = await FindByCondition(c => c.CompanyId.Equals(companyId), trackChanges).CountAsync();

            return PagedList<Employee>.ToPagedList(employees, employeeParameters.PageNumber, employeeParameters.PageSize, count);

        } 

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) => 
            await FindByCondition(c => c.CompanyId.Equals(companyId) && c.Id.Equals(id),
                            trackChanges).SingleOrDefaultAsync();
    }
}
