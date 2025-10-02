using System.Text;

namespace Blaj.ReMetin2Server.Common.Domain.Models;

public class Packet
{
    private List<byte> _buffer = new();
    private byte[] _readableBuffer = [];
    private int _readPos = 0;

    public Packet()
    {
    }

    public Packet(byte[] bytes)
    {
        SetBytes(bytes);
    }

    #region Functions

    public void SetBytes(byte[] bytes)
    {
        _buffer.AddRange(bytes);
        _readableBuffer = _buffer.ToArray();
    }

    public byte[] ToArray()
    {
        _readableBuffer = _buffer.ToArray();
        return _readableBuffer;
    }

    public int Length()
    {
        return _buffer.Count;
    }

    #endregion

    #region Write data

    public void Write(byte value)
    {
        _buffer.Add(value);
    }

    public void Write(byte[] bytes)
    {
        _buffer.AddRange(bytes);
    }

    public void Write(char value)
    {
        _buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(char[] values)
    {
        _buffer.AddRange(Encoding.ASCII.GetBytes(values));
    }

    public void Write(short value)
    {
        _buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(short[] values)
    {
        foreach (var value in values)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }
    }

    public void Write(ushort value)
    {
        _buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(ushort[] values)
    {
        foreach (var value in values)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }
    }

    public void Write(int value)
    {
        _buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(int[] values)
    {
        foreach (var value in values)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }
    }

    public void Write(uint value)
    {
        _buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(uint[] values)
    {
        foreach (var value in values)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }
    }

    public void Write(float value)
    {
        _buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(float[] values)
    {
        foreach (var value in values)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }
    }

    public void Write(bool value)
    {
        _buffer.AddRange(BitConverter.GetBytes(value));
    }

    #endregion

    #region Read data

    public byte ReadByte(bool moveReadPos = true)
    {
        if (_buffer.Count <= _readPos)
        {
            throw new Exception("Could not read value of type = byte");
        }

        var value = _readableBuffer[_readPos];

        if (moveReadPos)
        {
            _readPos += 1;
        }

        return value;
    }

    public byte[] ReadBytes(int length, bool moveReadPos = true)
    {
        if (_buffer.Count <= _readPos)
        {
            throw new Exception("Could not read value of type 'byte[]'!");
        }

        var value = _buffer.GetRange(_readPos, length).ToArray();

        if (moveReadPos)
        {
            _readPos += length;
        }

        return value;
    }

    public char ReadChar(bool moveReadPos = true)
    {
        if (_buffer.Count <= _readPos)
        {
            throw new Exception("Could not read value of type 'char'!");
        }

        var value = BitConverter.ToChar(_readableBuffer, _readPos);

        if (moveReadPos)
        {
            _readPos += 1;
        }

        return value;
    }

    public char[] ReadChars(int length, bool moveReadPos = true)
    {
        if (_buffer.Count <= _readPos)
        {
            throw new Exception("Could not read value of type 'char[]'!");
        }

        var value = _buffer
            .GetRange(_readPos, length)
            .Select(b => (char)b)
            .ToArray();

        if (moveReadPos)
        {
            _readPos += length;
        }

        return value;
    }

    public short ReadShort(bool moveReadPos = true)
    {
        if (_buffer.Count <= _readPos)
        {
            throw new Exception("Could not read value of type 'short'!");
        }

        var value = BitConverter.ToInt16(_readableBuffer, _readPos);

        if (moveReadPos)
        {
            _readPos += 2;
        }

        return value;
    }

    public short[] ReadShorts(int length, bool moveReadPos = true)
    {
        if (_buffer.Count <= _readPos + length * 2)
        {
            throw new Exception("Could not read value of type 'short[]'!");
        }

        var values = new short[length];

        for (var i = 0; i < length; i++)
        {
            values[i] = BitConverter.ToInt16(_readableBuffer, _readPos + i * 2);
        }

        if (moveReadPos)
        {
            _readPos += 2 * length;
        }

        return values;
    }

    public ushort ReadUshort(bool moveReadPos = true)
    {
        if (_buffer.Count <= _readPos)
        {
            throw new Exception("Could not read value of type 'ushort'!");
        }

        var value = BitConverter.ToUInt16(_readableBuffer, _readPos);

        if (moveReadPos)
        {
            _readPos += 2;
        }

        return value;
    }

    public ushort[] ReadUshorts(int length, bool moveReadPos = true)
    {
        if (_buffer.Count <= _readPos + length * 2)
        {
            throw new Exception("Could not read value of type 'ushort[]'!");
        }

        var values = new ushort[length];

        for (var i = 0; i < length; i++)
        {
            values[i] = BitConverter.ToUInt16(_readableBuffer, _readPos + i * 2);
        }

        if (moveReadPos)
        {
            _readPos += 2 * length;
        }

        return values;
    }

    public int ReadInt(bool moveReadPos = true)
    {
        if (_buffer.Count <= _readPos)
        {
            throw new Exception("Could not read value of type 'int'!");
        }

        var value = BitConverter.ToInt32(_readableBuffer, _readPos);

        if (moveReadPos)
        {
            _readPos += 4;
        }

        return value;
    }

    public int[] ReadInts(int length, bool moveReadPos = true)
    {
        if (_buffer.Count <= _readPos + length * 4)
        {
            throw new Exception("Could not read value of type 'int[]'!");
        }

        var values = new int[length];

        for (var i = 0; i < length; i++)
        {
            values[i] = BitConverter.ToInt32(_readableBuffer, _readPos + i * 4);
        }

        if (moveReadPos)
        {
            _readPos += 4 * length;
        }

        return values;
    }

    public uint ReadUint(bool moveReadPos = true)
    {
        if (_buffer.Count <= _readPos)
        {
            throw new Exception("Could not read value of type 'uint'!");
        }

        var value = BitConverter.ToUInt32(_readableBuffer, _readPos);

        if (moveReadPos)
        {
            _readPos += 4;
        }

        return value;
    }

    public uint[] ReadUints(int length, bool moveReadPos = true)
    {
        if (_buffer.Count <= _readPos + length * 4)
        {
            throw new Exception("Could not read value of type 'uint[]'!");
        }

        var values = new uint[length];

        for (var i = 0; i < length; i++)
        {
            values[i] = BitConverter.ToUInt32(_readableBuffer, _readPos + i * 4);
        }

        if (moveReadPos)
        {
            _readPos += 4 * length;
        }

        return values;
    }

    #endregion

    // public override string ToString()
    // {
    //     return ((Header)ReadByte(false)).ToString();
    // }

    public enum Header : byte
    {
        GcTimeSync = 0xfc,
        GcPhase = 0xfd,
        GcHandshake = 0xff,
        GcCharacterAdd = 1,
        GcMove = 3,
        GcChat = 4,
        GcLoginFailure = 7,
        GcCharacterCreateSuccess = 8,
        GcCharacterCreateFailure = 9,
        GcCharacterDeleteSuccess = 10,
        GcCharacterDeleteWrongPrivateCode = 11,
        GcCharacterPoints = 16,
        GcItemDel = 20,
        GcItemSet = 21,
        GcLoginSuccessNewslot = 32,
        GcSkillLevel = 76,
        GcEmpire = 90,
        GcTime = 106,
        GcMainCharacter = 113,
        GcChannel = 121,
        GcCharAdditionalInfo = 136,
        GcAuthSuccess = 150,
        GcRespondChannelStatus = 210,

        CgClientVersion2 = 0xf1,
        CgHandshake = 0xff,
        CgPong = 0xfe,
        CgChat = 3,
        CgCharacterCreate = 4,
        CgCharacterDelete = 5,
        CgCharacterSelect = 6,
        CgMove = 7,
        CgEntergame = 10,
        CgItemUse = 11,
        CgItemDrop = 12,
        CgItemMove = 13,
        CgItemPickup = 15,
        CgEmpire = 90,
        CgLogin2 = 109,
        CgLogin3 = 111,
        CgNewcibnPasspodAnswer = 202,
        CgStateChecker = 206,
    }
}