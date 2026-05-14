// ============================================================
// Models/Situacao.cs
// Tabela "Situacao" — Pendente, Pago, Atrasado
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoveisCarrara.Models
{
    [Table("Situacao")]
    public class Situacao
    {
        [Key]
        public int Codigo { get; set; }

        [StringLength(30)]
        public string? Descricao { get; set; }
    }
}
