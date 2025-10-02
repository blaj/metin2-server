using Metin2Server.Database.Data;
using Metin2Server.Domain.Entities;
using Metin2Server.Domain.Repositories;

namespace Metin2Server.Database.Repositories;

public class CharacterSkillRepository : Repository<CharacterSkill>, ICharacterSkillRepository
{
    public CharacterSkillRepository(GameDbContext dbContext) : base(dbContext)
    {
    }
}