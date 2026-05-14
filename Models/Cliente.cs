// ============================================================
// Models/Cliente.cs
// Tabela "Clientes" — tem apenas a chave estrangeira para Pessoas
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoveisCarrara.Models
{
    [Table("Clientes")]
    public class Cliente
    {
        // pessoa_id é chave primária E chave estrangeira ao mesmo tempo
        [Key]
        [Column("pessoa_id")]
        public int PessoaId { get; set; }

        // Propriedade de navegação: permite acessar os dados da Pessoa
        // Ex: cliente.Pessoa.Nome
        [ForeignKey("PessoaId")]
        public Pessoa? Pessoa { get; set; }
    }
}
