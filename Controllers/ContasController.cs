// ============================================================
// Controllers/ContasController.cs
// Gerencia os Lançamentos (contas a pagar e a receber)
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

        // ===========================================================
        // GET /Contas — lista todos os lançamentos
        // Aceita filtro por tipo via parâmetro na URL: /Contas?filtro=pagar
        // ===========================================================
        public async Task<IActionResult> Index(string? filtro)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            // Começa a query — IQueryable permite adicionar filtros antes de ir ao banco
            var query = _context.Lancamentos
                .Include(l => l.Situacao)
                .AsQueryable();

            // Aplica o filtro conforme o parâmetro recebido na URL
            if (filtro == "pagar")
                query = query.Where(l => l.VendaCodigo == null);   // sem venda = é uma conta a pagar
            else if (filtro == "receber")
                query = query.Where(l => l.VendaCodigo != null);   // tem venda = conta a receber
            else if (filtro == "pendentes")
                query = query.Where(l => l.DataPagamento == null);
            else if (filtro == "pagas")
                query = query.Where(l => l.DataPagamento != null);

            // Passa o filtro ativo para a View destacar o botão correto
            ViewBag.FiltroAtivo = filtro ?? "todas";

            var lancamentos = await query
                .OrderByDescending(l => l.DataVencimento)
                .ToListAsync();

            return View(lancamentos);
        }

        // ===========================================================
        // GET /Contas/Create — formulário para cadastrar conta
        // ===========================================================
        public async Task<IActionResult> Create()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            // Carrega as situações para o dropdown
            ViewBag.Situacoes = await _context.Situacoes.ToListAsync();
            return View();
        }

        // ===========================================================
        // POST /Contas/Create
        // ===========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lancamento lancamento)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                _context.Lancamentos.Add(lancamento);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Conta cadastrada com sucesso!";
                return RedirectToAction("Index");
            }

            ViewBag.Situacoes = await _context.Situacoes.ToListAsync();
            return View(lancamento);
        }

        // ===========================================================
        // GET /Contas/Edit/5
        // ===========================================================
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

        // ===========================================================
        // POST /Contas/Edit/5
        // ===========================================================
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

        // ===========================================================
        // POST /Contas/Delete/5
        // ===========================================================
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
