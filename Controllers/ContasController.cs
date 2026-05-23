// ============================================================
// Controllers/ContasController.cs
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoveisCarrara.Data;
using MoveisCarrara.Models;

namespace MoveisCarrara.Controllers
{
    public class ContasController : Controller
    {
        private readonly AppDbContext _context;

        public ContasController(AppDbContext context)
        {
            _context = context;
        }

        private bool VerificarLogin() =>
            HttpContext.Session.GetString("UsuarioLogado") != null;

        // GET /Contas
        public async Task<IActionResult> Index(string? filtro)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            // Carrega lancamentos com venda+cliente e compra+fornecedor
            var query = _context.Lancamentos
                .Include(l => l.Situacao)
                .Include(l => l.Venda)
                    .ThenInclude(v => v!.Cliente)
                        .ThenInclude(c => c!.Pessoa)
                .Include(l => l.Compra)
                    .ThenInclude(c => c!.Fornecedor)
                        .ThenInclude(f => f!.Pessoa)
                .AsQueryable();

            if (filtro == "pagar")
                query = query.Where(l => l.CompraCodigo != null);
            else if (filtro == "receber")
                query = query.Where(l => l.VendaCodigo != null);
            else if (filtro == "pendentes")
                query = query.Where(l => l.DataPagamento == null);
            else if (filtro == "pagas")
                query = query.Where(l => l.DataPagamento != null);

            ViewBag.FiltroAtivo = filtro ?? "todas";

            var lancamentos = await query
                .OrderByDescending(l => l.DataVencimento)
                .ToListAsync();

            return View(lancamentos);
        }

        // GET /Contas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var lancamento = await _context.Lancamentos
                .Include(l => l.Situacao)
                .FirstOrDefaultAsync(l => l.Codigo == id);

            if (lancamento == null) return NotFound();

            ViewBag.Situacoes = await _context.Situacoes.ToListAsync();
            return View(lancamento);
        }

        // POST /Contas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Lancamento lancamento)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                lancamento.Codigo = id;
                _context.Lancamentos.Update(lancamento);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Conta alterada com sucesso!";
                return RedirectToAction("Index");
            }

            ViewBag.Situacoes = await _context.Situacoes.ToListAsync();
            return View(lancamento);
        }

        // POST /Contas/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var lancamento = await _context.Lancamentos.FindAsync(id);
            if (lancamento != null)
            {
                _context.Lancamentos.Remove(lancamento);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Conta excluída com sucesso!";
            }

            return RedirectToAction("Index");
        }
    }
}
