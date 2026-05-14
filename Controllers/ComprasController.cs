// ============================================================
// Controllers/ComprasController.cs
// CRUD de Compras
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoveisCarrara.Data;
using MoveisCarrara.Models;

namespace MoveisCarrara.Controllers
{
    public class ComprasController : Controller
    {
        private readonly AppDbContext _context;

        public ComprasController(AppDbContext context)
        {
            _context = context;
        }

        private bool VerificarLogin() =>
            HttpContext.Session.GetString("UsuarioLogado") != null;

        // GET /Compras
        public async Task<IActionResult> Index()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var lista = await _context.Compras
                .Include(c => c.Fornecedor).ThenInclude(f => f!.Pessoa)
                .Include(c => c.Funcionario).ThenInclude(f => f!.Pessoa)
                .OrderByDescending(c => c.Data)
                .ToListAsync();

            return View(lista);
        }

        // GET /Compras/Create
        public async Task<IActionResult> Create()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            ViewBag.Fornecedores  = await _context.Fornecedores.Include(f => f.Pessoa).ToListAsync();
            ViewBag.Funcionarios  = await _context.Funcionarios.Include(f => f.Pessoa).ToListAsync();
            return View();
        }

        // POST /Compras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Compra compra)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Compra cadastrada com sucesso!";
                return RedirectToAction("Index");
            }

            ViewBag.Fornecedores  = await _context.Fornecedores.Include(f => f.Pessoa).ToListAsync();
            ViewBag.Funcionarios  = await _context.Funcionarios.Include(f => f.Pessoa).ToListAsync();
            return View(compra);
        }

        // GET /Compras/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var compra = await _context.Compras
                .Include(c => c.Fornecedor).ThenInclude(f => f!.Pessoa)
                .Include(c => c.Funcionario).ThenInclude(f => f!.Pessoa)
                .FirstOrDefaultAsync(c => c.Codigo == id);

            if (compra == null) return NotFound();

            ViewBag.Fornecedores  = await _context.Fornecedores.Include(f => f.Pessoa).ToListAsync();
            ViewBag.Funcionarios  = await _context.Funcionarios.Include(f => f.Pessoa).ToListAsync();
            return View(compra);
        }

        // POST /Compras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Compra compra)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                compra.Codigo = id;
                _context.Compras.Update(compra);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Compra alterada com sucesso!";
                return RedirectToAction("Index");
            }

            ViewBag.Fornecedores  = await _context.Fornecedores.Include(f => f.Pessoa).ToListAsync();
            ViewBag.Funcionarios  = await _context.Funcionarios.Include(f => f.Pessoa).ToListAsync();
            return View(compra);
        }

        // POST /Compras/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var compra = await _context.Compras.FindAsync(id);
            if (compra != null)
            {
                _context.Compras.Remove(compra);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Compra excluída com sucesso!";
            }

            return RedirectToAction("Index");
        }
    }
}
