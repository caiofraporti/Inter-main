// ============================================================
// Controllers/VendasController.cs
// CRUD de Vendas
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoveisCarrara.Data;
using MoveisCarrara.Models;

namespace MoveisCarrara.Controllers
{
    public class VendasController : Controller
    {
        private readonly AppDbContext _context;

        public VendasController(AppDbContext context)
        {
            _context = context;
        }

        private bool VerificarLogin() =>
            HttpContext.Session.GetString("UsuarioLogado") != null;

        // GET /Vendas
        public async Task<IActionResult> Index()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var lista = await _context.Vendas
                .Include(v => v.Cliente).ThenInclude(c => c!.Pessoa)
                .Include(v => v.Funcionario).ThenInclude(f => f!.Pessoa)
                .OrderByDescending(v => v.Data)
                .ToListAsync();

            return View(lista);
        }

        // GET /Vendas/Create
        public async Task<IActionResult> Create()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            ViewBag.Clientes     = await _context.Clientes.Include(c => c.Pessoa).ToListAsync();
            ViewBag.Funcionarios = await _context.Funcionarios.Include(f => f.Pessoa).ToListAsync();
            return View();
        }

        // POST /Vendas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venda venda)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                _context.Vendas.Add(venda);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Venda cadastrada com sucesso!";
                return RedirectToAction("Index");
            }

            ViewBag.Clientes     = await _context.Clientes.Include(c => c.Pessoa).ToListAsync();
            ViewBag.Funcionarios = await _context.Funcionarios.Include(f => f.Pessoa).ToListAsync();
            return View(venda);
        }

        // GET /Vendas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var venda = await _context.Vendas
                .Include(v => v.Cliente).ThenInclude(c => c!.Pessoa)
                .Include(v => v.Funcionario).ThenInclude(f => f!.Pessoa)
                .FirstOrDefaultAsync(v => v.Codigo == id);

            if (venda == null) return NotFound();

            ViewBag.Clientes     = await _context.Clientes.Include(c => c.Pessoa).ToListAsync();
            ViewBag.Funcionarios = await _context.Funcionarios.Include(f => f.Pessoa).ToListAsync();
            return View(venda);
        }

        // POST /Vendas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venda venda)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                venda.Codigo = id;
                _context.Vendas.Update(venda);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Venda alterada com sucesso!";
                return RedirectToAction("Index");
            }

            ViewBag.Clientes     = await _context.Clientes.Include(c => c.Pessoa).ToListAsync();
            ViewBag.Funcionarios = await _context.Funcionarios.Include(f => f.Pessoa).ToListAsync();
            return View(venda);
        }

        // POST /Vendas/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var venda = await _context.Vendas.FindAsync(id);
            if (venda != null)
            {
                _context.Vendas.Remove(venda);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Venda excluída com sucesso!";
            }

            return RedirectToAction("Index");
        }
    }
}
