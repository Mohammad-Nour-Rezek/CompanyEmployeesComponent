using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }

        // can create duplicate functions here for async CreateAsync() and inside create AddAsync()
        public void CreateCompany(Company company) =>
            Create(company);

        public void DeleteCompany(Company company) =>
            Delete(company);

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges) =>
            await FindAll(trackChanges).OrderBy(c => c.Name).ToListAsync();

        public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) =>
            await FindByCondition(c => ids.Contains(c.Id), trackChanges).ToListAsync();

        public async Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges) =>
            await FindByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefaultAsync();

        #region Test For Sync/Async
        //public Company GetCompany(Guid companyId, bool trackChanges)
        //{
        //    //await _RepositoryContext.Database.ExecuteSqlRawAsync("WAITFOR DELAY '00:00:04';"); for async method
        //    _RepositoryContext.Database.ExecuteSqlRaw("WAITFOR DELAY '00:00:04';");

        //    var comp = FindByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefault();

        //    return comp;

        //}
        #endregion
    }
}
