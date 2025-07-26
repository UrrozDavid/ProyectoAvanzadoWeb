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
            try
            {
                bool isUpdate = label.LabelId > 0;
                var currentUser = "system";

                label.AddAudit(currentUser);
                label.AddLogging(isUpdate ? Models.Enums.LoggingType.Update : Models.Enums.LoggingType.Create);

                if (isUpdate)
                {
                    var existing = await repositoryLabel.FindAsync(label.LabelId);
                    if (existing == null) return false;

                    existing.Name = label.Name;
                    existing.Color = label.Color;

                    return await repositoryLabel.UpdateAsync(existing);
                }
                else
                {
                    return await repositoryLabel.CreateAsync(label);
                }
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error si quieres, por ejemplo: logger.LogError(ex, "Error saving label");
                return false;
            }
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

