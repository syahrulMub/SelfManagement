using MyPrivateManager.Models;
namespace MyPrivateManager.IDatabaseServices;

public interface ITechnicianServices
{
    Task<IEnumerable<Technician>> GetTechniciansAsync();
    Task<Technician?> GetTechnicianByIdAsync(int technicianId);
    Task<bool> CreateTechnicianAsync(Technician technician);
    Task<bool> UpdateTechnicianAsync(int technicianId, Technician technician);
    Task<bool> DeleteTechnicianAsync(int technicianId);
}
