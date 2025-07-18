using Fretefy.Test.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fretefy.Test.Infra.EntityFramework.Mappings
{
    public class RegiaoCidadeMap : IEntityTypeConfiguration<RegiaoCidade>
    {
        public void Configure(EntityTypeBuilder<RegiaoCidade> builder)
        {
            builder.ToTable("RegiaoCidade");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(x => x.RegiaoId)
                .IsRequired();

            builder.Property(x => x.Cidade)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.UF)
                .IsRequired()
                .HasMaxLength(2)
                .IsFixedLength();

            // Índice composto para evitar duplicação de cidade/UF na mesma região
            builder.HasIndex(x => new { x.RegiaoId, x.Cidade, x.UF })
                .IsUnique()
                .HasDatabaseName("IX_RegiaoCidade_RegiaoId_Cidade_UF");

            // Relacionamento com Regiao
            builder.HasOne(x => x.Regiao)
                .WithMany(x => x.Cidades)
                .HasForeignKey(x => x.RegiaoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

