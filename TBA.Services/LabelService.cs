
using TBA.Models.Entities;
using TBA.Business;

namespace TBA.Services
{
    public class LabelService
    {
        private readonly IBusinessLabel _businessLabel;

        public LabelService(IBusinessLabel businessLabel)
        {
            _businessLabel = businessLabel;
        }

        public async Task<IEnumerable<Label>> GetAllLabelsAsync()
            => await _businessLabel.GetAllLabelsAsync();

        public async Task<Label?> GetLabelByIdAsync(int id)
            => await _businessLabel.GetLabelAsync(id);

        public async Task<bool> SaveLabelAsync(Label label)
            => await _businessLabel.SaveLabelAsync(label);

        public async Task<bool> DeleteLabelAsync(int id)
        {
            var label = await _businessLabel.GetLabelAsync(id);
            if (label == null) return false;
            return await _businessLabel.DeleteLabelAsync(label);
        }
    }
}