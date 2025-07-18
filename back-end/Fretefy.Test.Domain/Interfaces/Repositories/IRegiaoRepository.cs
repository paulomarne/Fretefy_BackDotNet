using Fretefy.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fretefy.Test.Domain.Interfaces.Repositories
{
    public interface IRegiaoRepository
    {
        Task<IEnumerable<Regiao>> GetAllAsync();
        Task<Regiao> GetByIdAsync(Guid id);
        Task<Regiao> GetByNomeAsync(string nome);
        Task<Regiao> CreateAsync(Regiao regiao);
        Task<Regiao> UpdateAsync(Regiao regiao);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByNomeAsync(string nome, Guid? excludeId = null);
    }
}

