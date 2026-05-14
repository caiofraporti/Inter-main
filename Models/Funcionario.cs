// ============================================================
// Models/Funcionario.cs
// Tabela "Funcionarios" — guarda usuário e senha para login
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoveisCarrara.Models
{
    [Table("Funcionarios")]
    public class Funcionario
    {
        [Key]
        [Column("pessoa_id")]
        public int PessoaId { get; set; }

        [Required]
        [StringLength(50)]
        public string Usuario { get; set; } = "";

        [Required]
        [StringLength(200)]
        public string Senha { get; set; } = "";

        // Navegação: acessa os dados pessoais do funcionário
        [ForeignKey("PessoaId")]
        public Pessoa? Pessoa { get; set; }
    }
}
