using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fretefy.Test.Domain.Entities
{
    public class Regiao : IEntity
    {
        public Regiao()
        {
            Cidades = new List<RegiaoCidade>();
        }

        public Regiao(string nome)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Ativo = true;
            DataCriacao = DateTime.UtcNow;
            Cidades = new List<RegiaoCidade>();
        }

        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public virtual ICollection<RegiaoCidade> Cidades { get; set; }
    }
}

