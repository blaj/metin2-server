using Metin2Server.Database.Data;
using Metin2Server.Database.Domain.Entities;
using Metin2Server.Database.Domain.Repositories;

namespace Metin2Server.Database.Repositories;

public class CharacterQuickSlotRepository : Repository<CharacterQuickSlot>, ICharacterQuickSlotRepository
{
    public CharacterQuickSlotRepository(GameDbContext dbContext) : base(dbContext)
    {
    }
}