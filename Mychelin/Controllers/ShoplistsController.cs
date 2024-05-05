using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mychelin.Data;
using Mychelin.Models;

namespace Mychelin.Controllers
{
    public class ShoplistsController : Controller
    {
        private readonly MychelinContext _context;

        public ShoplistsController(MychelinContext context)
        {
            _context = context;
        }

        // GET: Shoplists
        public async Task<IActionResult> Index()
        {
            var mychelinContext = _context.Shoplist.Include(s => s.person);
            return View(await mychelinContext.ToListAsync());
        }

        // GET: Shoplists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoplist = await _context.Shoplist
                .Include(s => s.person)
                .FirstOrDefaultAsync(m => m.ShoplistId == id);
            if (shoplist == null)
            {
                return NotFound();
            }

            return View(shoplist);
        }

        // GET: Shoplists/Create
        public IActionResult Create()
        {
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Mail");
            return View();
        }

        // POST: Shoplists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShoplistId,ShoplistName,Status,Category,Class,Star,Coment,Url,ImagePath,UpdatedDate,PersonId")] Shoplist shoplist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoplist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Mail", shoplist.PersonId);
            return View(shoplist);
        }

        // GET: Shoplists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoplist = await _context.Shoplist.FindAsync(id);
            if (shoplist == null)
            {
                return NotFound();
            }
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Mail", shoplist.PersonId);
            return View(shoplist);
        }

        // POST: Shoplists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShoplistId,ShoplistName,Status,Category,Class,Star,Coment,Url,ImagePath,UpdatedDate,PersonId")] Shoplist shoplist)
        {
            if (id != shoplist.ShoplistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoplist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoplistExists(shoplist.ShoplistId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Mail", shoplist.PersonId);
            return View(shoplist);
        }

        // GET: Shoplists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoplist = await _context.Shoplist
                .Include(s => s.person)
                .FirstOrDefaultAsync(m => m.ShoplistId == id);
            if (shoplist == null)
            {
                return NotFound();
            }

            return View(shoplist);
        }

        // POST: Shoplists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoplist = await _context.Shoplist.FindAsync(id);
            if (shoplist != null)
            {
                _context.Shoplist.Remove(shoplist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoplistExists(int id)
        {
            return _context.Shoplist.Any(e => e.ShoplistId == id);
        }
    }
}
