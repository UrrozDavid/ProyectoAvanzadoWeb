using TBA.Models.Entities;

internal interface IBusinessLabel
{
    Task<IEnumerable<Label>> GetAllLabelsAsync();
    Task<bool> SaveLabelAsync(Label label);
    Task<bool> DeleteLabelAsync(Label label);
    Task<Label> GetLabelAsync(int id);
}