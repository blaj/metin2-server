using System.Buffers.Binary;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.Protocol.Codecs;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Channel.Features.Common.LoginSuccessNewslot;

public class GameClientLoginSuccessNewslotOutCodec : IPacketOutCodec<GameClientLoginSuccessNewslotPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientLoginSuccessNewslotPacket p)
    {
        int charNameLen  = Constants.CharacterNameMaxLength + 1;
        int guildNameLen = Constants.GuildNameMaxLength + 1;

        int simpleCharSize = GetSimpleCharacterSize(charNameLen); // 38 + charNameLen
        int totalSize =
            simpleCharSize * 4 +   // Characters[4]
            sizeof(uint) * 4 +     // GuildIds[4]
            guildNameLen * 4 +     // GuildNames[4] (każdy dokładnie guildNameLen bajtów)
            sizeof(uint) +         // Handle
            sizeof(uint);          // RandomKey

        var buffer = new byte[totalSize];
        var span = buffer.AsSpan();
        int offset = 0;

        // --- Characters[4] ---
        for (int i = 0; i < 4; i++)
        {
            var ch = p.Characters[i];
            offset += WriteSimpleCharacter(span.Slice(offset, simpleCharSize), ch, charNameLen);
        }

        // --- GuildIds[4] ---
        for (int i = 0; i < 4; i++)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), p.GuildIds[i]);
            offset += 4;
        }

        // --- GuildNames[4] (STAŁA długość: guildNameLen) ---
        for (int i = 0; i < 4; i++)
        {
            var dst = span.Slice(offset, guildNameLen);
            // jeżeli masz guild name jako char[] – najpierw na string:
            var gname = StringUtils.FromCBuffer(p.GuildNames[i]);
            WriteFixedCString(dst, gname);
            offset += guildNameLen;
        }

        // --- Handle ---
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), p.Handle);
        offset += 4;

        // --- RandomKey ---
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), p.RandomKey);
        offset += 4;

        return buffer;
    }

    private static int GetSimpleCharacterSize(int charNameLen)
    {
        // Id(4) + Name(N) + Job(1) + Level(1) + PlayMinutes(4) +
        // St(1) + Ht(1) + Dx(1) + Iq(1) + MainPart(2) + ChangeName(1) +
        // HairPart(2) + Dummy(4) + X(4) + Y(4) + Addr(4) + Port(2) + SkillGroup(1)
        const int fixedBytes = 38;
        return fixedBytes + charNameLen;
    }

    private static int WriteSimpleCharacter(Span<byte> dst, SimpleCharacter.Dto.SimpleCharacter c, int charNameLen)
    {
        int offset = 0;

        BinaryPrimitives.WriteUInt32LittleEndian(dst.Slice(offset, 4), c.Id);
        offset += 4;

        // Name – ZAWSZE stała długość, C-string
        var nameStr = StringUtils.FromCBuffer(c.Name); // ucinamy na pierwszym '\0'
        WriteFixedCString(dst.Slice(offset, charNameLen), nameStr);
        offset += charNameLen;

        dst[offset++] = c.Job;
        dst[offset++] = c.Level;

        BinaryPrimitives.WriteUInt32LittleEndian(dst.Slice(offset, 4), c.PlayMinutes); offset += 4;

        dst[offset++] = c.St;
        dst[offset++] = c.Ht;
        dst[offset++] = c.Dx;
        dst[offset++] = c.Iq;

        BinaryPrimitives.WriteUInt16LittleEndian(dst.Slice(offset, 2), c.MainPart); offset += 2;
        dst[offset++] = c.ChangeName;
        BinaryPrimitives.WriteUInt16LittleEndian(dst.Slice(offset, 2), c.HairPart); offset += 2;

        // Dummy – tu w tym pakiecie masz 4 bajty (sprawdź kontrakt!)
        dst[offset++] = c.Dummy[0];
        dst[offset++] = c.Dummy[1];
        dst[offset++] = c.Dummy[2];
        dst[offset++] = c.Dummy[3];

        BinaryPrimitives.WriteUInt32LittleEndian(dst.Slice(offset, 4), c.X);    offset += 4;
        BinaryPrimitives.WriteUInt32LittleEndian(dst.Slice(offset, 4), c.Y);    offset += 4;
        BinaryPrimitives.WriteUInt32LittleEndian(dst.Slice(offset, 4), c.Addr); offset += 4;

        BinaryPrimitives.WriteUInt16LittleEndian(dst.Slice(offset, 2), c.Port); offset += 2;

        dst[offset++] = c.SkillGroup;

        return offset;
    }
    
    private static void WriteFixedCString(Span<byte> target, string value)
    {
        // target.Length to stała długość pola (np. NAME_MAX+1).
        // C-string: max target.Length-1 znaków + '\0' + padding zerami.
        target.Clear();
        if (string.IsNullOrEmpty(value)) return;

        int maxPayload = target.Length - 1;
        int n = Math.Min(value.Length, maxPayload);
        for (int i = 0; i < n; i++)
        {
            // Metin2 używa 1 bajt/znak; jeśli wyjdzie > 0xFF – podstaw '?'
            char ch = value[i];
            target[i] = ch <= 0xFF ? (byte)ch : (byte)'?';
        }
        // target[n] już jest 0 przez Clear()
    }
}