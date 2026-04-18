using FitnessApi.DataLayer;
using FitnessApi.DataLayer.Entities;
using FitnessApi.Interfaces;
using FitnessApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessApi.DataAccess
{
    public class SugarLogsDao : ISugarLogsDao
    {
        private readonly ParienseDbContext _parienseDbContext;

        public SugarLogsDao(ParienseDbContext parienseDbContext)
        {
            _parienseDbContext = parienseDbContext;
        }
        public async Task AddSugarLog(string userId, SugarLogDto logDto)
        {
            if (logDto == null) return;

            var date = new DateTime(logDto.Date.Year, logDto.Date.Month, logDto.Date.Day);

            // Find existing row for same user and calendar date
            var existing = await _parienseDbContext.SugarLog
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Date == date)
                .ConfigureAwait(false);

            if (existing == null)
            {
                var entity = new SugarLog
                {
                    UserId = userId,
                    Date = date,
                    Fasting = MapSlot(logDto.Fasting),
                    AfterBreakfast = MapSlot(logDto.AfterBreakfast),
                    BeforeLunch = MapSlot(logDto.BeforeLunch),
                    AfterLunch2hrs = MapSlot(logDto.AfterLunch2hrs),
                    BeforeDinner = MapSlot(logDto.BeforeDinner),
                    AfterDinner2hrs = MapSlot(logDto.AfterDinner2hrs),
                    Between2am3am = MapSlot(logDto.Between2am3am)
                };

                await _parienseDbContext.SugarLog.AddAsync(entity).ConfigureAwait(false);
            }
            else
            {
                // Update existing row — overwrite slots only when provided in DTO
                existing.Fasting = MapSlot(logDto.Fasting) ?? existing.Fasting;
                existing.AfterBreakfast = MapSlot(logDto.AfterBreakfast) ?? existing.AfterBreakfast;
                existing.BeforeLunch = MapSlot(logDto.BeforeLunch) ?? existing.BeforeLunch;
                existing.AfterLunch2hrs = MapSlot(logDto.AfterLunch2hrs) ?? existing.AfterLunch2hrs;
                existing.BeforeDinner = MapSlot(logDto.BeforeDinner) ?? existing.BeforeDinner;
                existing.AfterDinner2hrs = MapSlot(logDto.AfterDinner2hrs) ?? existing.AfterDinner2hrs;
                existing.Between2am3am = MapSlot(logDto.Between2am3am) ?? existing.Between2am3am;

                _parienseDbContext.SugarLog.Update(existing);
            }

            await _parienseDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IList<SugarLogDto>> GetAllSugarLogs(string userId)
        {
            var items = await _parienseDbContext.SugarLog
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync().ConfigureAwait(false);

            return items.Select(e => new SugarLogDto
            {
                Id = e.Id.ToString(),
                Date = DateOnly.FromDateTime(e.Date),
                Fasting = MapSlotDto(e.Fasting),
                AfterBreakfast = MapSlotDto(e.AfterBreakfast),
                BeforeLunch = MapSlotDto(e.BeforeLunch),
                AfterLunch2hrs = MapSlotDto(e.AfterLunch2hrs),
                BeforeDinner = MapSlotDto(e.BeforeDinner),
                AfterDinner2hrs = MapSlotDto(e.AfterDinner2hrs),
                Between2am3am = MapSlotDto(e.Between2am3am)
            }).ToList();
        }

        private static SugarLogSlot? MapSlot(SugarLogslotDto? dto)
        {
            if (dto == null) return null;
            return new SugarLogSlot { SugarLevel = dto.SugarLevel, Timestamp = dto.Timestamp };
        }

        private static SugarLogslotDto? MapSlotDto(SugarLogSlot? slot)
        {
            if (slot == null) return null;
            return new SugarLogslotDto { SugarLevel = slot.SugarLevel, Timestamp = slot.Timestamp };
        }
    }
}
