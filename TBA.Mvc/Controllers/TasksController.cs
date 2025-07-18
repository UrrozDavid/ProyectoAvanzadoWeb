using Microsoft.AspNetCore.Mvc;
using TBA.Services;

namespace TBA.Mvc.Controllers
{
    public class TasksController (ICardService _cardService) : Controller
    {

        public async Task<IActionResult> Index()
        {
            var tasks = await _cardService.GetTasksAsync();
            return View(tasks);
        }
    }

}
