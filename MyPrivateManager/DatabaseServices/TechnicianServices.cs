using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace DatabaseServices
{
    public class TechnicianServices : ITechnicianServices
    {
        private readonly DatabaseContext _dbContext;

        public TechnicianServices(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Technician>> GetTechniciansAsync()
        {
            return await _dbContext.Technicians
                            .Include(t => t.Orders)
                            .ToListAsync();
        }

        public async Task<Technician?> GetTechnicianByIdAsync(int technicianId)
        {
            return await _dbContext.Technicians
                    .Include(t => t.Orders)
                    .Where(t => t.TechnicianId == technicianId)
                    .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateTechnicianAsync(Technician technician)
        {
            _dbContext.Technicians.Add(technician);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTechnicianAsync(int technicianId, Technician technician)
        {
            var existingTechnician = await _dbContext.Technicians.FirstOrDefaultAsync(t => t.TechnicianId == technicianId);

            if (existingTechnician != null)
            {
                existingTechnician.FullName = technician.FullName;
                existingTechnician.Phone = technician.Phone;
                existingTechnician.IsActive = technician.IsActive;
                existingTechnician.AvgRating = technician.AvgRating;
                existingTechnician.CompletedJobs = technician.CompletedJobs;
                _dbContext.Technicians.Update(existingTechnician);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteTechnicianAsync(int technicianId)
        {
            var technician = await _dbContext.Technicians.FirstOrDefaultAsync(t => t.TechnicianId == technicianId);

            if (technician != null)
            {
                _dbContext.Technicians.Remove(technician);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
