using FluentAssertions;
using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces.Services;
using Fretefy.Test.WebApi.Controllers;
using Fretefy.Test.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fretefy.Test.Tests.WebApi
{
    public class RegiaoControllerTests
    {
        private readonly Mock<IRegiaoService> _mockService;
        private readonly RegiaoController _controller;

        public RegiaoControllerTests()
        {
            _mockService = new Mock<IRegiaoService>();
            _controller = new RegiaoController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_DeveRetornarOkComListaDeRegioes()
        {
            // Arrange
            var regioes = new List<Regiao>
            {
                new Regiao("Região 1"),
                new Regiao("Região 2")
            };

            foreach (var regiao in regioes)
            {
                regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "São Paulo", "SP"));
            }

            _mockService.Setup(s => s.GetAllAsync())
                       .ReturnsAsync(regioes);

            // Act
            var resultado = await _controller.GetAll();

            // Assert
            var okResult = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
            var regioesDto = okResult.Value.Should().BeAssignableTo<IEnumerable<RegiaoDto>>().Subject;
            regioesDto.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetById_ComIdValido_DeveRetornarOkComRegiao()
        {
            // Arrange
            var id = Guid.NewGuid();
            var regiao = new Regiao("Região Teste") { Id = id };
            regiao.Cidades.Add(new RegiaoCidade(id, "São Paulo", "SP"));

            _mockService.Setup(s => s.GetByIdAsync(id))
                       .ReturnsAsync(regiao);

            // Act
            var resultado = await _controller.GetById(id);

            // Assert
            var okResult = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
            var regiaoDto = okResult.Value.Should().BeOfType<RegiaoDto>().Subject;
            regiaoDto.Id.Should().Be(id);
            regiaoDto.Nome.Should().Be("Região Teste");
        }

        [Fact]
        public async Task GetById_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockService.Setup(s => s.GetByIdAsync(id))
                       .ReturnsAsync((Regiao)null);

            // Act
            var resultado = await _controller.GetById(id);

            // Assert
            resultado.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Create_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var createDto = new CreateRegiaoDto
            {
                Nome = "Nova Região",
                Cidades = new List<CreateRegiaoCidadeDto>
                {
                    new CreateRegiaoCidadeDto { Cidade = "São Paulo", UF = "SP" }
                }
            };

            var regiaoCriada = new Regiao("Nova Região");
            regiaoCriada.Cidades.Add(new RegiaoCidade(regiaoCriada.Id, "São Paulo", "SP"));

            _mockService.Setup(s => s.CreateAsync(It.IsAny<Regiao>()))
                       .ReturnsAsync(regiaoCriada);

            // Act
            var resultado = await _controller.Create(createDto);

            // Assert
            var createdResult = resultado.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var regiaoDto = createdResult.Value.Should().BeOfType<RegiaoDto>().Subject;
            regiaoDto.Nome.Should().Be("Nova Região");
        }

        [Fact]
        public async Task Create_ComDadosInvalidos_DeveRetornarBadRequest()
        {
            // Arrange
            var createDto = new CreateRegiaoDto
            {
                Nome = "", // Nome inválido
                Cidades = new List<CreateRegiaoCidadeDto>()
            };

            _controller.ModelState.AddModelError("Nome", "O nome da região é obrigatório");

            // Act
            var resultado = await _controller.Create(createDto);

            // Assert
            resultado.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Create_ComExcecaoDeNegocio_DeveRetornarBadRequest()
        {
            // Arrange
            var createDto = new CreateRegiaoDto
            {
                Nome = "Região Existente",
                Cidades = new List<CreateRegiaoCidadeDto>
                {
                    new CreateRegiaoCidadeDto { Cidade = "São Paulo", UF = "SP" }
                }
            };

            _mockService.Setup(s => s.CreateAsync(It.IsAny<Regiao>()))
                       .ThrowsAsync(new ArgumentException("Já existe uma região com este nome"));

            // Act
            var resultado = await _controller.Create(createDto);

            // Assert
            resultado.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var updateDto = new UpdateRegiaoDto
            {
                Nome = "Região Atualizada",
                Cidades = new List<UpdateRegiaoCidadeDto>
                {
                    new UpdateRegiaoCidadeDto { Cidade = "Rio de Janeiro", UF = "RJ" }
                }
            };

            var regiaoAtualizada = new Regiao("Região Atualizada") { Id = id };
            regiaoAtualizada.Cidades.Add(new RegiaoCidade(id, "Rio de Janeiro", "RJ"));

            _mockService.Setup(s => s.UpdateAsync(It.IsAny<Regiao>()))
                       .ReturnsAsync(regiaoAtualizada);

            // Act
            var resultado = await _controller.Update(id, updateDto);

            // Assert
            var okResult = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
            var regiaoDto = okResult.Value.Should().BeOfType<RegiaoDto>().Subject;
            regiaoDto.Nome.Should().Be("Região Atualizada");
        }

        [Fact]
        public async Task Delete_ComIdValido_DeveRetornarNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockService.Setup(s => s.DeleteAsync(id))
                       .ReturnsAsync(true);

            // Act
            var resultado = await _controller.Delete(id);

            // Assert
            resultado.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockService.Setup(s => s.DeleteAsync(id))
                       .ReturnsAsync(false);

            // Act
            var resultado = await _controller.Delete(id);

            // Assert
            resultado.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ToggleStatus_ComIdValido_DeveRetornarOk()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockService.Setup(s => s.ToggleStatusAsync(id))
                       .ReturnsAsync(true);

            // Act
            var resultado = await _controller.ToggleStatus(id);

            // Assert
            resultado.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ExportToExcel_DeveRetornarArquivoExcel()
        {
            // Arrange
            var excelData = new byte[] { 1, 2, 3, 4, 5 }; // Dados simulados

            _mockService.Setup(s => s.ExportToExcelAsync())
                       .ReturnsAsync(excelData);

            // Act
            var resultado = await _controller.ExportToExcel();

            // Assert
            var fileResult = resultado.Should().BeOfType<FileContentResult>().Subject;
            fileResult.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fileResult.FileContents.Should().BeEquivalentTo(excelData);
        }
    }
}

