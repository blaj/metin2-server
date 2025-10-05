using System.Collections.Concurrent;
using Metin2Server.Shared.Dto;

namespace Metin2Server.Channel.Services;

public class GameCharacterStore
{
    private readonly ConcurrentDictionary<uint, GameCharacter> _gameCharactersByVid = new();

    public bool TryAdd(GameCharacter gameCharacter) => _gameCharactersByVid.TryAdd(gameCharacter.Vid, gameCharacter);

    public bool TryGet(uint vid, out GameCharacter? gameCharacter) =>
        _gameCharactersByVid.TryGetValue(vid, out gameCharacter);

    public bool TryRemove(uint vid, out GameCharacter? gameCharacter) =>
        _gameCharactersByVid.TryRemove(vid, out gameCharacter);

    public IEnumerable<GameCharacter> All() => _gameCharactersByVid.Values;
}