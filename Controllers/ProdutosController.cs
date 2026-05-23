// ============================================================
// Controllers/TipoProdutosController.cs
// CRUD de Tipos de Produtos
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

        // GET /TipoProdutos
        public async Task<IActionResult> Index()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var lista = await _context.Produtos
                .OrderBy(t => t.NomeProduto)
                .ToListAsync();

            return View(lista);
        }

        // GET /TipoProdutos/Create
        public IActionResult Create()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");
            return View();
        }

        // POST /TipoProdutos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto tipoProduto)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                _context.Produtos.Add(tipoProduto);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Tipo de produto cadastrado com sucesso!";
                return RedirectToAction("Index");
            }

            return View(tipoProduto);
        }

        // GET /TipoProdutos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var tipoProduto = await _context.Produtos.FindAsync(id);
            if (tipoProduto == null) return NotFound();

            return View(tipoProduto);
        }

        // POST /TipoProdutos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produto tipoProduto)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                tipoProduto.Codigo = id;
                _context.Produtos.Update(tipoProduto);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Tipo de produto alterado com sucesso!";
                return RedirectToAction("Index");
            }

            return View(tipoProduto);
        }

        // POST /TipoProdutos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var tipoProduto = await _context.Produtos.FindAsync(id);
            if (tipoProduto != null)
            {
                _context.Produtos.Remove(tipoProduto);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Tipo de produto excluído com sucesso!";
            }

            return RedirectToAction("Index");
        }
    }
}
