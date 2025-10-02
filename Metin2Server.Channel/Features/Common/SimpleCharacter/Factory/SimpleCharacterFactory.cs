using System.Net;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Channel.Features.Common.SimpleCharacter.Factory;

public class SimpleCharacterFactory
{
    public static Dto.SimpleCharacter FromProto(Shared.DbContracts.Common.Character character) => new(
        GrpcUtils.ToUIntChecked(character.Id),
        StringUtils.NormalizeString(character.Name, Constants.CharacterNameMaxLength + 1),
        (byte)character.Race,
        GrpcUtils.ToByteChecked(character.Statistics.Level),
        character.Statistics.PlayTime,
        GrpcUtils.ToByteChecked(character.Statistics.St),
        GrpcUtils.ToByteChecked(character.Statistics.Ht),
        GrpcUtils.ToByteChecked(character.Statistics.Dx),
        GrpcUtils.ToByteChecked(character.Statistics.Iq),
        GrpcUtils.ToUShortChecked(character.PartMain),
        0,
        GrpcUtils.ToUShortChecked(character.PartHair),
        [0, 0, 0, 0],
        GrpcUtils.ToUIntChecked(character.Coordinates.X),
        GrpcUtils.ToUIntChecked(character.Coordinates.Y),
        BitConverter.ToUInt32(IPAddress.Parse("127.0.0.1").GetAddressBytes(), 0),
        13000,
        GrpcUtils.ToByteChecked(character.SkillGroup));

    public static Dto.SimpleCharacter Empty() => new(
        0,
        StringUtils.EmptyChars(Constants.CharacterNameMaxLength + 1),
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        1,
        [0, 0, 0, 0],
        0,
        0,
        BitConverter.ToUInt32(IPAddress.Parse("127.0.0.1").GetAddressBytes(), 0),
        13000,
        0);
}