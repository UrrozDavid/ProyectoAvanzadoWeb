using Microsoft.AspNetCore.Mvc;
using TBA.Business;
using TBA.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class LabelController(IBusinessLabel businessLabel) : ControllerBase
    {
        [HttpGet(Name = "GetLabels")]
        public async Task<IEnumerable<Label>> GetLabels()
        {
            return await businessLabel.GetAllLabelsAsync();
        }

        [HttpGet("{id}")]
        public async Task<Label> GetById(int id)
        {
            var label = await businessLabel.GetLabelAsync(id);
            return label;
        }


        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<Label> labels)
        {
            foreach (var item in labels)
            {
                await businessLabel.SaveLabelAsync(item);
            }
            return true;
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Label label)
        {
            return await businessLabel.DeleteLabelAsync(label);
        }
    }
}
