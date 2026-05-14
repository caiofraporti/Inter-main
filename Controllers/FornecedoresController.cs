// ============================================================
// Controllers/FornecedoresController.cs
// CRUD completo de Fornecedores
// Mesmo padrão do ClientesController
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoveisCarrara.Data;
using MoveisCarrara.Models;

namespace MoveisCarrara.Controllers
{
    public class FornecedoresController : Controller
    {
        private readonly AppDbContext _context;

        public FornecedoresController(AppDbContext context)
        {
            _context = context;
        }

        private bool VerificarLogin() =>
            HttpContext.Session.GetString("UsuarioLogado") != null;

        // ===========================================================
        // GET /Fornecedores — lista todos os fornecedores
        // ===========================================================
        public async Task<IActionResult> Index()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var fornecedores = await _context.Fornecedores
                .Include(f => f.Pessoa)
                .ToListAsync();

            return View(fornecedores);
        }

        // ===========================================================
        // GET /Fornecedores/Create
        // ===========================================================
        public IActionResult Create()
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");
            return View();
        }

        // ===========================================================
        // POST /Fornecedores/Create
        // ===========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pessoa pessoa)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                // Fornecedores são sempre Pessoa Jurídica
                pessoa.TipoPessoa = "J";

                _context.Pessoas.Add(pessoa);
                await _context.SaveChangesAsync();

                var fornecedor = new Fornecedor { PessoaId = pessoa.Id };
                _context.Fornecedores.Add(fornecedor);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Fornecedor cadastrado com sucesso!";
                return RedirectToAction("Index");
            }

            return View(pessoa);
        }

        // ===========================================================
        // GET /Fornecedores/Edit/5
        // ===========================================================
        public async Task<IActionResult> Edit(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var fornecedor = await _context.Fornecedores
                .Include(f => f.Pessoa)
                .FirstOrDefaultAsync(f => f.PessoaId == id);

            if (fornecedor == null) return NotFound();

            return View(fornecedor.Pessoa);
        }

        // ===========================================================
        // POST /Fornecedores/Edit/5
        // ===========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pessoa pessoa)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                pessoa.Id = id;
                _context.Pessoas.Update(pessoa);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Fornecedor alterado com sucesso!";
                return RedirectToAction("Index");
            }

            return View(pessoa);
        }

        // ===========================================================
        // POST /Fornecedores/Delete/5
        // ===========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!VerificarLogin()) return RedirectToAction("Login", "Home");

            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor != null)
            {
                _context.Fornecedores.Remove(fornecedor);
                var pessoa = await _context.Pessoas.FindAsync(id);
                if (pessoa != null) _context.Pessoas.Remove(pessoa);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Fornecedor excluído com sucesso!";
            }

            return RedirectToAction("Index");
        }
    }
}
