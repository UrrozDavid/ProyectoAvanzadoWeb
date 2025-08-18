using Microsoft.AspNetCore.Mvc;
using TBA.Models.Entities;
using TBA.Services;
using TBA.Mvc.Models; // Si tienes ViewModel para Label, usa aquí

namespace TBA.Mvc.Controllers
{
    public class LabelsController : Controller
    {
        private readonly LabelService _labelService;

        public LabelsController(LabelService labelService)
        {
            _labelService = labelService;
        }

        // GET: Labels
        public async Task<IActionResult> Index()
        {
            var labels = await _labelService.GetAllLabelsAsync();
            return View(labels);
        }

        // GET: Labels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Labels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Label model)
        {
            if (ModelState.IsValid)
            {
                var success = await _labelService.SaveLabelAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Labels/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var label = await _labelService.GetLabelByIdAsync(id);
            if (label == null) return NotFound();

            return View(label);
        }

        // POST: Labels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Label model)
        {
            if (ModelState.IsValid)
            {
                var success = await _labelService.SaveLabelAsync(model);
                if (success)
                    return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Labels/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var label = await _labelService.GetLabelByIdAsync(id);
            if (label == null) return NotFound();

            return View(label);
        }

        // POST: Labels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _labelService.DeleteLabelAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: Labels/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var label = await _labelService.GetLabelByIdAsync(id);
            if (label == null)
                return NotFound();

            // Si Label tiene alguna propiedad relacionada que debas cargar, hazlo aquí.
            // Por ejemplo, si Label tuviera una referencia a otra entidad:
            // label.SomeRelatedEntity = await repositorySomeEntity.FindAsync(label.SomeRelatedEntityId);

            return View(label);
        }
    }
}
