using FluentAssertions;
using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces.Repositories;
using Fretefy.Test.Domain.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fretefy.Test.Tests.Domain
{
    public class RegiaoServiceTests
    {
        private readonly Mock<IRegiaoRepository> _mockRepository;
        private readonly RegiaoService _service;

        public RegiaoServiceTests()
        {
            _mockRepository = new Mock<IRegiaoRepository>();
            _service = new RegiaoService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarTodasAsRegioes()
        {
            // Arrange
            var regioes = new List<Regiao>
            {
                new Regiao("Região 1"),
                new Regiao("Região 2")
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(regioes);

            // Act
            var resultado = await _service.GetAllAsync();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(regioes);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarRegiao()
        {
            // Arrange
            var id = Guid.NewGuid();
            var regiao = new Regiao("Região Teste") { Id = id };

            _mockRepository.Setup(r => r.GetByIdAsync(id))
                          .ReturnsAsync(regiao);

            // Act
            var resultado = await _service.GetByIdAsync(id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(id);
            resultado.Nome.Should().Be("Região Teste");
        }

        [Fact]
        public async Task CreateAsync_ComDadosValidos_DeveCriarRegiao()
        {
            // Arrange
            var regiao = new Regiao("Nova Região");
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "São Paulo", "SP"));

            _mockRepository.Setup(r => r.ExistsByNomeAsync(regiao.Nome, null))
                          .ReturnsAsync(false);

            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Regiao>()))
                          .ReturnsAsync(regiao);

            // Act
            var resultado = await _service.CreateAsync(regiao);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be("Nova Região");
            resultado.Ativo.Should().BeTrue();
            resultado.Cidades.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateAsync_ComNomeVazio_DeveLancarExcecao()
        {
            // Arrange
            var regiao = new Regiao("");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(regiao));
        }

        [Fact]
        public async Task CreateAsync_ComNomeExistente_DeveLancarExcecao()
        {
            // Arrange
            var regiao = new Regiao("Região Existente");
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "São Paulo", "SP"));

            _mockRepository.Setup(r => r.ExistsByNomeAsync(regiao.Nome, null))
                          .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(regiao));
        }

        [Fact]
        public async Task CreateAsync_SemCidades_DeveLancarExcecao()
        {
            // Arrange
            var regiao = new Regiao("Região Sem Cidades");

            _mockRepository.Setup(r => r.ExistsByNomeAsync(regiao.Nome, null))
                          .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(regiao));
        }

        [Fact]
        public async Task CreateAsync_ComCidadesDuplicadas_DeveLancarExcecao()
        {
            // Arrange
            var regiao = new Regiao("Região com Duplicatas");
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "São Paulo", "SP"));
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "São Paulo", "SP")); // Duplicata

            _mockRepository.Setup(r => r.ExistsByNomeAsync(regiao.Nome, null))
                          .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(regiao));
        }

        [Fact]
        public async Task UpdateAsync_ComDadosValidos_DeveAtualizarRegiao()
        {
            // Arrange
            var id = Guid.NewGuid();
            var regiaoExistente = new Regiao("Região Original") { Id = id };
            var regiaoAtualizada = new Regiao("Região Atualizada") { Id = id };
            regiaoAtualizada.Cidades.Add(new RegiaoCidade(id, "Rio de Janeiro", "RJ"));

            _mockRepository.Setup(r => r.GetByIdAsync(id))
                          .ReturnsAsync(regiaoExistente);

            _mockRepository.Setup(r => r.ExistsByNomeAsync(regiaoAtualizada.Nome, id))
                          .ReturnsAsync(false);

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Regiao>()))
                          .ReturnsAsync(regiaoAtualizada);

            // Act
            var resultado = await _service.UpdateAsync(regiaoAtualizada);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be("Região Atualizada");
            resultado.DataAtualizacao.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteAsync_ComIdValido_DeveRemoverRegiao()
        {
            // Arrange
            var id = Guid.NewGuid();
            var regiao = new Regiao("Região para Deletar") { Id = id };

            _mockRepository.Setup(r => r.GetByIdAsync(id))
                          .ReturnsAsync(regiao);

            _mockRepository.Setup(r => r.DeleteAsync(id))
                          .ReturnsAsync(true);

            // Act
            var resultado = await _service.DeleteAsync(id);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task ToggleStatusAsync_ComIdValido_DeveAlternarStatus()
        {
            // Arrange
            var id = Guid.NewGuid();
            var regiao = new Regiao("Região Teste") { Id = id, Ativo = true };

            _mockRepository.Setup(r => r.GetByIdAsync(id))
                          .ReturnsAsync(regiao);

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Regiao>()))
                          .ReturnsAsync(regiao);

            // Act
            var resultado = await _service.ToggleStatusAsync(id);

            // Assert
            resultado.Should().BeTrue();
            regiao.Ativo.Should().BeFalse();
            regiao.DataAtualizacao.Should().NotBeNull();
        }
    }
}

