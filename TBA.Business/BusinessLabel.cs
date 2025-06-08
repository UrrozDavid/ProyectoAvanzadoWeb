using TBA.Models.Entities;
using TBA.Repositories;
using TBA.Core.Extensions;


namespace TBA.Business
{
    public interface IBusinessLabel
    {
        Task<IEnumerable<Label>> GetAllLabelsAsync();
        Task<bool> SaveLabelAsync(Label label);
        Task<bool> DeleteLabelAsync(Label label);
        Task<Label> GetLabelAsync(int id);
    }

    public class BusinessLabel(IRepositoryLabel repositoryLabel) : IBusinessLabel
    {
        public async Task<IEnumerable<Label>> GetAllLabelsAsync()
        {
            return await repositoryLabel.ReadAsync();
        }

        public async Task<bool> SaveLabelAsync(Label label)
        {
            var newLabel = ""; //Identity
            label.AddAudit(newLabel);
            label.AddLogging(label.LabelId <= 0 ? Models.Enums.LoggingType.Create: Models.Enums.LoggingType.Update);
            var exists = await repositoryLabel.ExistsAsync(label);
            return await repositoryLabel.UpsertAsync(label, exists);
        }

        public async Task<bool> DeleteLabelAsync(Label label)
        {
            return await repositoryLabel.DeleteAsync(label);
        }

        public async Task<Label> GetLabelAsync(int id)
        {
            return await repositoryLabel.FindAsync(id);
        }

        public Task<IEnumerable<Label>> GetAllLabels()
        {
            throw new NotImplementedException();
        }
    }
}

