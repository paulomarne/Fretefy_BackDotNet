using Fretefy.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fretefy.Test.Domain.Interfaces.Services
{
    public interface IRegiaoService
    {
        Task<IEnumerable<Regiao>> GetAllAsync();
        Task<Regiao> GetByIdAsync(Guid id);
        Task<Regiao> CreateAsync(Regiao regiao);
        Task<Regiao> UpdateAsync(Regiao regiao);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ToggleStatusAsync(Guid id);
        Task<byte[]> ExportToExcelAsync();
    }
}

