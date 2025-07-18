using Fretefy.Test.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fretefy.Test.Infra.EntityFramework.Mappings
{
    public class RegiaoMap : IEntityTypeConfiguration<Regiao>
    {
        public void Configure(EntityTypeBuilder<Regiao> builder)
        {
            builder.ToTable("Regiao");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Ativo)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.DataCriacao)
                .IsRequired();

            builder.Property(x => x.DataAtualizacao)
                .IsRequired(false);

            // Índice único para o nome
            builder.HasIndex(x => x.Nome)
                .IsUnique()
                .HasDatabaseName("IX_Regiao_Nome");

            // Relacionamento com RegiaoCidade
            builder.HasMany(x => x.Cidades)
                .WithOne(x => x.Regiao)
                .HasForeignKey(x => x.RegiaoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

