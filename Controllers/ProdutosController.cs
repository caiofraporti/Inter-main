// ============================================================
// Controllers/ProdutosController.cs
// CRUD de Produtos
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoveisCarrara.Data;
using MoveisCarrara.Models;

namespace MoveisCarrara.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        private bool VerificarLogin() =>
            HttpContext.Session.GetString("UsuarioLogado") != null;

        // GET /Produtos
        public async Task<IActionResult> Index()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var lista = await _context.Produtos
                .OrderBy(t => t.NomeProduto)
                .ToListAsync();

            return View(lista);
        }

        // GET /Produtos/Create
        public IActionResult Create()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");
            return View();
        }

        // POST /Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto Produto)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                _context.Produtos.Add(Produto);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Produto cadastrado com sucesso!";
                return RedirectToAction("Index");
            }

            return View(Produto);
        }

        // GET /Produtos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var Produto = await _context.Produtos.FindAsync(id);
            if (Produto == null) return NotFound();

            return View(Produto);
        }

        // POST /Produtos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produto Produto)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                Produto.Codigo = id;
                _context.Produtos.Update(Produto);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Produto alterado com sucesso!";
                return RedirectToAction("Index");
            }

            return View(Produto);
        }

        // POST /Produtos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var Produto = await _context.Produtos.FindAsync(id);
            if (Produto != null)
            {
                _context.Produtos.Remove(Produto);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Produto excluído com sucesso!";
            }

            return RedirectToAction("Index");
        }
    }
}
