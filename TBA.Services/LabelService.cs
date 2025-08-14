using TBA.Models.Entities;
using TBA.Business;

namespace TBA.Services
{

    public interface ILabelService
    {
        Task<IEnumerable<Label>> GetAllLabelsAsync();
        Task<Label?> GetLabelByIdAsync(int id);
        Task<bool> SaveLabelAsync(Label label);
        Task<bool> DeleteLabelAsync(int id);
        

    }

    public class LabelService : ILabelService
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

        
        public Task<bool> DeleteLabelAsync(int id) => _businessLabel.DeleteByIdAsync(id);
    }
}
