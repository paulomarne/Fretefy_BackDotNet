using FluentAssertions;
using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Infra.EntityFramework.Repositories;
using Fretefy.Test.Tests.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fretefy.Test.Tests.Infra
{
    public class RegiaoRepositoryTests : IDisposable
    {
        private readonly RegiaoRepository _repository;
        private readonly Fretefy.Test.Infra.EntityFramework.TestDbContext _context;

        public RegiaoRepositoryTests()
        {
            _context = TestDbContextHelper.CreateInMemoryContext();
            _repository = new RegiaoRepository(_context);
        }

        [Fact]
        public async Task CreateAsync_DeveAdicionarRegiaoNoBanco()
        {
            // Arrange
            var regiao = new Regiao("Região Teste");
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "São Paulo", "SP"));
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "Rio de Janeiro", "RJ"));

            // Act
            var resultado = await _repository.CreateAsync(regiao);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().NotBeEmpty();

            var regiaoNoBanco = await _context.Regioes.FindAsync(resultado.Id);
            regiaoNoBanco.Should().NotBeNull();
            regiaoNoBanco.Nome.Should().Be("Região Teste");
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarTodasAsRegioes()
        {
            // Arrange
            var regiao1 = new Regiao("Região 1");
            regiao1.Cidades.Add(new RegiaoCidade(regiao1.Id, "São Paulo", "SP"));

            var regiao2 = new Regiao("Região 2");
            regiao2.Cidades.Add(new RegiaoCidade(regiao2.Id, "Belo Horizonte", "MG"));

            await _repository.CreateAsync(regiao1);
            await _repository.CreateAsync(regiao2);

            // Act
            var resultado = await _repository.GetAllAsync();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.All(r => r.Cidades.Any()).Should().BeTrue();
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarRegiao()
        {
            // Arrange
            var regiao = new Regiao("Região Teste");
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "São Paulo", "SP"));

            await _repository.CreateAsync(regiao);

            // Act
            var resultado = await _repository.GetByIdAsync(regiao.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(regiao.Id);
            resultado.Nome.Should().Be("Região Teste");
            resultado.Cidades.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetByNomeAsync_ComNomeValido_DeveRetornarRegiao()
        {
            // Arrange
            var regiao = new Regiao("Região Única");
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "São Paulo", "SP"));

            await _repository.CreateAsync(regiao);

            // Act
            var resultado = await _repository.GetByNomeAsync("Região Única");

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be("Região Única");
        }

        [Fact]
        public async Task UpdateAsync_DeveAtualizarRegiaoExistente()
        {
            // Arrange
            var regiao = new Regiao("Região Original");
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "São Paulo", "SP"));

            await _repository.CreateAsync(regiao);

            // Modificar a região
            regiao.Nome = "Região Atualizada";
            regiao.Cidades.Clear();
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "Rio de Janeiro", "RJ"));
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "Belo Horizonte", "MG"));

            // Act
            var resultado = await _repository.UpdateAsync(regiao);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be("Região Atualizada");
            resultado.Cidades.Should().HaveCount(2);
        }

        [Fact]
        public async Task DeleteAsync_ComIdValido_DeveRemoverRegiao()
        {
            // Arrange
            var regiao = new Regiao("Região para Deletar");
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "São Paulo", "SP"));

            await _repository.CreateAsync(regiao);

            // Act
            var resultado = await _repository.DeleteAsync(regiao.Id);

            // Assert
            resultado.Should().BeTrue();

            var regiaoNoBanco = await _context.Regioes.FindAsync(regiao.Id);
            regiaoNoBanco.Should().BeNull();
        }

        [Fact]
        public async Task ExistsByNomeAsync_ComNomeExistente_DeveRetornarTrue()
        {
            // Arrange
            var regiao = new Regiao("Região Existente");
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "São Paulo", "SP"));

            await _repository.CreateAsync(regiao);

            // Act
            var resultado = await _repository.ExistsByNomeAsync("Região Existente");

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByNomeAsync_ComNomeInexistente_DeveRetornarFalse()
        {
            // Act
            var resultado = await _repository.ExistsByNomeAsync("Região Inexistente");

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsByNomeAsync_ComExclusaoDeId_DeveIgnorarIdEspecificado()
        {
            // Arrange
            var regiao = new Regiao("Região Teste");
            regiao.Cidades.Add(new RegiaoCidade(regiao.Id, "São Paulo", "SP"));

            await _repository.CreateAsync(regiao);

            // Act
            var resultado = await _repository.ExistsByNomeAsync("Região Teste", regiao.Id);

            // Assert
            resultado.Should().BeFalse(); // Deve ignorar a própria região
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}

