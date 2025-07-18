using System;
using System.ComponentModel.DataAnnotations;

namespace Fretefy.Test.Domain.Entities
{
    public class RegiaoCidade : IEntity
    {
        public RegiaoCidade()
        {
        }

        public RegiaoCidade(Guid regiaoId, string cidade, string uf)
        {
            Id = Guid.NewGuid();
            RegiaoId = regiaoId;
            Cidade = cidade;
            UF = uf;
        }

        public Guid Id { get; set; }

        public Guid RegiaoId { get; set; }

        [Required(ErrorMessage = "O nome da cidade é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome da cidade deve ter no máximo 100 caracteres")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "A UF é obrigatória")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve ter exatamente 2 caracteres")]
        public string UF { get; set; }

        public virtual Regiao Regiao { get; set; }
    }
}

