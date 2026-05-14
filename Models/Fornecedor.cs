// ============================================================
// Models/Fornecedor.cs
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoveisCarrara.Models
{
    [Table("Fornecedores")]
    public class Fornecedor
    {
        [Key]
        [Column("pessoa_id")]
        public int PessoaId { get; set; }

        [ForeignKey("PessoaId")]
        public Pessoa? Pessoa { get; set; }
    }
}
