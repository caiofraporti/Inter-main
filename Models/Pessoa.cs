// ============================================================
// Models/Pessoa.cs
// Representa a tabela "Pessoas" do banco de dados
// O Entity Framework usa esta classe para ler e gravar dados
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoveisCarrara.Models
{
    // [Table("Pessoas")] diz ao EF que esta classe = tabela Pessoas
    [Table("Pessoas")]
    public class Pessoa
    {
        // [Key] marca a chave primária
        [Key]
        public int Id { get; set; }

        // [Required] = campo obrigatório (valida no formulário)
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; } = "";

        [StringLength(100)]
        public string? NomeSocial { get; set; }

        // tipo_pessoa: 'F' = Física, 'J' = Jurídica
        [Column("tipo_pessoa")]
        [StringLength(1)]
        public string? TipoPessoa { get; set; }

        [StringLength(50)]
        public string? Documento { get; set; }

        [Column("tipo_endereco")]
        [StringLength(50)]
        public string? TipoEndereco { get; set; }

        [StringLength(100)]
        public string? Logradouro { get; set; }

        [StringLength(10)]
        public string? Numero { get; set; }

        [StringLength(50)]
        public string? Bairro { get; set; }

        [StringLength(50)]
        public string? Cidade { get; set; }

        [StringLength(20)]
        public string? Cep { get; set; }

        [StringLength(20)]
        public string? Telefone { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; }
    }
}
