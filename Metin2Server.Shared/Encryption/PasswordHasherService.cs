using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;

namespace Metin2Server.Shared.Encryption;

public class PasswordHasherService
{
    private const int SaltLen = 16;
    private const int HashLen = 32;

    private static readonly Argon2Config DefaultCfg = new()
    {
        Type = Argon2Type.DataIndependentAddressing,
        Version = Argon2Version.Nineteen,
        TimeCost = 3,
        MemoryCost = 64 * 1024,
        Lanes = 1,
        Threads = 1,
        HashLength = HashLen
    };
    
    private readonly byte[] _pepper;
    
    public PasswordHasherService(string pepperBase64)
    {
        if (string.IsNullOrWhiteSpace(pepperBase64))
        {
            throw new ArgumentException("Pepper must be configured");
        }

        _pepper = Convert.FromBase64String(pepperBase64);
    }

    public string Hash(string password)
    {
        Span<byte> salt = stackalloc byte[SaltLen];
        RandomNumberGenerator.Fill(salt);

        var passBytes = Combine(_pepper, Encoding.UTF8.GetBytes(password));

        var cfg = Clone(DefaultCfg);
        cfg.Salt = salt.ToArray();
        cfg.Password = passBytes;

        return Argon2.Hash(cfg);
    }

    public bool Verify(string password, string phc)
    {
        var passBytes = Combine(_pepper, Encoding.UTF8.GetBytes(password));
        return Argon2.Verify(phc, passBytes);
    }

    public bool NeedsRehash(string phc)
    {
        return !phc.Contains($"$argon2id$")
               || !phc.Contains($"v={(int)DefaultCfg.Version}")
               || !phc.Contains($"m={DefaultCfg.MemoryCost}")
               || !phc.Contains($"t={DefaultCfg.TimeCost}")
               || !phc.Contains($"p={DefaultCfg.Lanes}");
    }

    private static byte[] Combine(ReadOnlySpan<byte> a, byte[] b)
    {
        var res = new byte[a.Length + b.Length];
        a.CopyTo(res);
        b.AsSpan().CopyTo(res.AsSpan(a.Length));
        return res;
    }

    private static Argon2Config Clone(Argon2Config c) => new()
    {
        Type       = c.Type,
        Version    = c.Version,
        TimeCost   = c.TimeCost,
        MemoryCost = c.MemoryCost,
        Lanes      = c.Lanes,
        Threads    = c.Threads,
        HashLength = c.HashLength
    };
}