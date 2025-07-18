using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fretefy.Test.WebApi.DTOs
{
    public class RegiaoDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome da região é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        [Required(ErrorMessage = "É obrigatório informar ao menos uma cidade")]
        [MinLength(1, ErrorMessage = "É obrigatório informar ao menos uma cidade")]
        public List<RegiaoCidadeDto> Cidades { get; set; }
    }

    public class RegiaoCidadeDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome da cidade é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome da cidade deve ter no máximo 100 caracteres")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "A UF é obrigatória")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve ter exatamente 2 caracteres")]
        public string UF { get; set; }
    }

    public class CreateRegiaoDto
    {
        [Required(ErrorMessage = "O nome da região é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "É obrigatório informar ao menos uma cidade")]
        [MinLength(1, ErrorMessage = "É obrigatório informar ao menos uma cidade")]
        public List<CreateRegiaoCidadeDto> Cidades { get; set; }
    }

    public class CreateRegiaoCidadeDto
    {
        [Required(ErrorMessage = "O nome da cidade é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome da cidade deve ter no máximo 100 caracteres")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "A UF é obrigatória")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve ter exatamente 2 caracteres")]
        public string UF { get; set; }
    }

    public class UpdateRegiaoDto
    {
        [Required(ErrorMessage = "O nome da região é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "É obrigatório informar ao menos uma cidade")]
        [MinLength(1, ErrorMessage = "É obrigatório informar ao menos uma cidade")]
        public List<UpdateRegiaoCidadeDto> Cidades { get; set; }
    }

    public class UpdateRegiaoCidadeDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "O nome da cidade é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome da cidade deve ter no máximo 100 caracteres")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "A UF é obrigatória")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve ter exatamente 2 caracteres")]
        public string UF { get; set; }
    }
}

