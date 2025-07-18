using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces.Services;
using Fretefy.Test.WebApi.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fretefy.Test.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RegiaoController : ControllerBase
    {
        private readonly IRegiaoService _regiaoService;

        public RegiaoController(IRegiaoService regiaoService)
        {
            _regiaoService = regiaoService;
        }

        /// <summary>
        /// Obtém todas as regiões cadastradas
        /// </summary>
        /// <returns>Lista de regiões</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RegiaoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RegiaoDto>>> GetAll()
        {
            try
            {
                var regioes = await _regiaoService.GetAllAsync();
                var regioesDto = regioes.Select(MapToDto).ToList();
                return Ok(regioesDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma região específica pelo ID
        /// </summary>
        /// <param name="id">ID da região</param>
        /// <returns>Dados da região</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RegiaoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RegiaoDto>> GetById(Guid id)
        {
            try
            {
                var regiao = await _regiaoService.GetByIdAsync(id);
                if (regiao == null)
                    return NotFound(new { message = "Região não encontrada" });

                return Ok(MapToDto(regiao));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Cria uma nova região
        /// </summary>
        /// <param name="createRegiaoDto">Dados da região a ser criada</param>
        /// <returns>Região criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RegiaoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegiaoDto>> Create([FromBody] CreateRegiaoDto createRegiaoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var regiao = MapFromCreateDto(createRegiaoDto);
                var regiaoCriada = await _regiaoService.CreateAsync(regiao);

                return CreatedAtAction(nameof(GetById), new { id = regiaoCriada.Id }, MapToDto(regiaoCriada));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma região existente
        /// </summary>
        /// <param name="id">ID da região</param>
        /// <param name="updateRegiaoDto">Dados atualizados da região</param>
        /// <returns>Região atualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RegiaoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RegiaoDto>> Update(Guid id, [FromBody] UpdateRegiaoDto updateRegiaoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var regiao = MapFromUpdateDto(id, updateRegiaoDto);
                var regiaoAtualizada = await _regiaoService.UpdateAsync(regiao);

                return Ok(MapToDto(regiaoAtualizada));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Remove uma região
        /// </summary>
        /// <param name="id">ID da região</param>
        /// <returns>Confirmação da remoção</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var sucesso = await _regiaoService.DeleteAsync(id);
                if (!sucesso)
                    return NotFound(new { message = "Região não encontrada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Alterna o status (ativo/inativo) de uma região
        /// </summary>
        /// <param name="id">ID da região</param>
        /// <returns>Confirmação da alteração</returns>
        [HttpPatch("{id}/toggle-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var sucesso = await _regiaoService.ToggleStatusAsync(id);
                if (!sucesso)
                    return NotFound(new { message = "Região não encontrada" });

                return Ok(new { message = "Status da região alterado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Exporta as regiões para Excel
        /// </summary>
        /// <returns>Arquivo Excel com as regiões</returns>
        [HttpGet("export")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ExportToExcel()
        {
            try
            {
                var excelData = await _regiaoService.ExportToExcelAsync();
                var fileName = $"regioes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
            }
        }

        #region Métodos de Mapeamento

        private RegiaoDto MapToDto(Regiao regiao)
        {
            return new RegiaoDto
            {
                Id = regiao.Id,
                Nome = regiao.Nome,
                Ativo = regiao.Ativo,
                DataCriacao = regiao.DataCriacao,
                DataAtualizacao = regiao.DataAtualizacao,
                Cidades = regiao.Cidades?.Select(c => new RegiaoCidadeDto
                {
                    Id = c.Id,
                    Cidade = c.Cidade,
                    UF = c.UF
                }).ToList() ?? new List<RegiaoCidadeDto>()
            };
        }

        private Regiao MapFromCreateDto(CreateRegiaoDto dto)
        {
            var regiao = new Regiao(dto.Nome);
            
            foreach (var cidadeDto in dto.Cidades)
            {
                regiao.Cidades.Add(new RegiaoCidade(regiao.Id, cidadeDto.Cidade, cidadeDto.UF));
            }

            return regiao;
        }

        private Regiao MapFromUpdateDto(Guid id, UpdateRegiaoDto dto)
        {
            var regiao = new Regiao
            {
                Id = id,
                Nome = dto.Nome,
                DataAtualizacao = DateTime.UtcNow,
                Cidades = dto.Cidades?.Select(c => new RegiaoCidade
                {
                    Id = c.Id ?? Guid.NewGuid(),
                    RegiaoId = id,
                    Cidade = c.Cidade,
                    UF = c.UF
                }).ToList() ?? new List<RegiaoCidade>()
            };

            return regiao;
        }

        #endregion
    }
}

