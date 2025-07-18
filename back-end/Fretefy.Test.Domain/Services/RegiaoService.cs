using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces.Repositories;
using Fretefy.Test.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;

namespace Fretefy.Test.Domain.Services
{
    public class RegiaoService : IRegiaoService
    {
        private readonly IRegiaoRepository _regiaoRepository;

        public RegiaoService(IRegiaoRepository regiaoRepository)
        {
            _regiaoRepository = regiaoRepository;
        }

        public async Task<IEnumerable<Regiao>> GetAllAsync()
        {
            return await _regiaoRepository.GetAllAsync();
        }

        public async Task<Regiao> GetByIdAsync(Guid id)
        {
            return await _regiaoRepository.GetByIdAsync(id);
        }

        public async Task<Regiao> CreateAsync(Regiao regiao)
        {
            // Validações de negócio
            if (string.IsNullOrWhiteSpace(regiao.Nome))
                throw new ArgumentException("O nome da região é obrigatório");

            if (await _regiaoRepository.ExistsByNomeAsync(regiao.Nome))
                throw new ArgumentException("Já existe uma região com este nome");

            if (regiao.Cidades == null || !regiao.Cidades.Any())
                throw new ArgumentException("É obrigatório informar ao menos uma cidade na região");

            // Validar se não há cidades duplicadas
            var cidadesDuplicadas = regiao.Cidades
                .GroupBy(c => new { c.Cidade, c.UF })
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            if (cidadesDuplicadas.Any())
                throw new ArgumentException("Não é possível informar a mesma cidade mais de uma vez");

            regiao.Id = Guid.NewGuid();
            regiao.Ativo = true;
            regiao.DataCriacao = DateTime.UtcNow;

            // Definir IDs para as cidades
            foreach (var cidade in regiao.Cidades)
            {
                cidade.Id = Guid.NewGuid();
                cidade.RegiaoId = regiao.Id;
            }

            return await _regiaoRepository.CreateAsync(regiao);
        }

        public async Task<Regiao> UpdateAsync(Regiao regiao)
        {
            var regiaoExistente = await _regiaoRepository.GetByIdAsync(regiao.Id);
            if (regiaoExistente == null)
                throw new ArgumentException("Região não encontrada");

            // Validações de negócio
            if (string.IsNullOrWhiteSpace(regiao.Nome))
                throw new ArgumentException("O nome da região é obrigatório");

            if (await _regiaoRepository.ExistsByNomeAsync(regiao.Nome, regiao.Id))
                throw new ArgumentException("Já existe uma região com este nome");

            if (regiao.Cidades == null || !regiao.Cidades.Any())
                throw new ArgumentException("É obrigatório informar ao menos uma cidade na região");

            // Validar se não há cidades duplicadas
            var cidadesDuplicadas = regiao.Cidades
                .GroupBy(c => new { c.Cidade, c.UF })
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            if (cidadesDuplicadas.Any())
                throw new ArgumentException("Não é possível informar a mesma cidade mais de uma vez");

            regiao.DataAtualizacao = DateTime.UtcNow;

            // Definir IDs para as cidades que não possuem
            foreach (var cidade in regiao.Cidades)
            {
                if (cidade.Id == Guid.Empty)
                    cidade.Id = Guid.NewGuid();
                cidade.RegiaoId = regiao.Id;
            }

            return await _regiaoRepository.UpdateAsync(regiao);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var regiao = await _regiaoRepository.GetByIdAsync(id);
            if (regiao == null)
                return false;

            return await _regiaoRepository.DeleteAsync(id);
        }

        public async Task<bool> ToggleStatusAsync(Guid id)
        {
            var regiao = await _regiaoRepository.GetByIdAsync(id);
            if (regiao == null)
                return false;

            regiao.Ativo = !regiao.Ativo;
            regiao.DataAtualizacao = DateTime.UtcNow;

            await _regiaoRepository.UpdateAsync(regiao);
            return true;
        }

        public async Task<byte[]> ExportToExcelAsync()
        {
            var regioes = await _regiaoRepository.GetAllAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Regiões");

                // Cabeçalhos
                worksheet.Cells[1, 1].Value = "Nome da Região";
                worksheet.Cells[1, 2].Value = "Status";
                worksheet.Cells[1, 3].Value = "Data de Criação";
                worksheet.Cells[1, 4].Value = "Cidades";

                // Estilo do cabeçalho
                using (var range = worksheet.Cells[1, 1, 1, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                int row = 2;
                foreach (var regiao in regioes)
                {
                    worksheet.Cells[row, 1].Value = regiao.Nome;
                    worksheet.Cells[row, 2].Value = regiao.Ativo ? "Ativo" : "Inativo";
                    worksheet.Cells[row, 3].Value = regiao.DataCriacao.ToString("dd/MM/yyyy");
                    
                    var cidades = string.Join("; ", regiao.Cidades.Select(c => $"{c.Cidade}/{c.UF}"));
                    worksheet.Cells[row, 4].Value = cidades;

                    row++;
                }

                // Auto-ajustar colunas
                worksheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}

