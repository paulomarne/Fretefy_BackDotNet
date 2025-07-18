using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fretefy.Test.Infra.EntityFramework.Repositories
{
    public class RegiaoRepository : IRegiaoRepository
    {
        private readonly TestDbContext _context;

        public RegiaoRepository(TestDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Regiao>> GetAllAsync()
        {
            return await _context.Regioes
                .Include(r => r.Cidades)
                .OrderBy(r => r.Nome)
                .ToListAsync();
        }

        public async Task<Regiao> GetByIdAsync(Guid id)
        {
            return await _context.Regioes
                .Include(r => r.Cidades)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Regiao> GetByNomeAsync(string nome)
        {
            return await _context.Regioes
                .Include(r => r.Cidades)
                .FirstOrDefaultAsync(r => r.Nome.ToLower() == nome.ToLower());
        }

        public async Task<Regiao> CreateAsync(Regiao regiao)
        {
            _context.Regioes.Add(regiao);
            await _context.SaveChangesAsync();
            return regiao;
        }

        public async Task<Regiao> UpdateAsync(Regiao regiao)
        {
            var regiaoExistente = await _context.Regioes
                .Include(r => r.Cidades)
                .FirstOrDefaultAsync(r => r.Id == regiao.Id);

            if (regiaoExistente == null)
                return null;

            // Atualizar propriedades da região
            regiaoExistente.Nome = regiao.Nome;
            regiaoExistente.Ativo = regiao.Ativo;
            regiaoExistente.DataAtualizacao = regiao.DataAtualizacao;

            // Remover cidades que não estão mais na lista
            var cidadesParaRemover = regiaoExistente.Cidades
                .Where(ce => !regiao.Cidades.Any(c => c.Id == ce.Id))
                .ToList();

            foreach (var cidade in cidadesParaRemover)
            {
                _context.RegioesCidades.Remove(cidade);
            }

            // Adicionar ou atualizar cidades
            foreach (var cidade in regiao.Cidades)
            {
                var cidadeExistente = regiaoExistente.Cidades
                    .FirstOrDefault(c => c.Id == cidade.Id);

                if (cidadeExistente != null)
                {
                    // Atualizar cidade existente
                    cidadeExistente.Cidade = cidade.Cidade;
                    cidadeExistente.UF = cidade.UF;
                }
                else
                {
                    // Adicionar nova cidade
                    cidade.RegiaoId = regiao.Id;
                    _context.RegioesCidades.Add(cidade);
                }
            }

            await _context.SaveChangesAsync();
            return regiaoExistente;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var regiao = await _context.Regioes.FindAsync(id);
            if (regiao == null)
                return false;

            _context.Regioes.Remove(regiao);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Regioes.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> ExistsByNomeAsync(string nome, Guid? excludeId = null)
        {
            var query = _context.Regioes.Where(r => r.Nome.ToLower() == nome.ToLower());
            
            if (excludeId.HasValue)
                query = query.Where(r => r.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}

