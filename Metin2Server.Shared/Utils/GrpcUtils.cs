namespace Metin2Server.Shared.Utils;

public static class GrpcUtils
{
    public static byte ToByteChecked(uint value)
    {
        if (value > byte.MaxValue)
        {
            throw new OverflowException($"Value {value} doesn't fit into byte.");
        }
        
        return (byte)value;
    }

    public static byte ToByteChecked(int value)
    {
        if (value is < byte.MinValue or > byte.MaxValue)
        {
            throw new OverflowException($"Value {value} doesn't fit into byte.");
        }
        
        return (byte)value;
    }

    public static ushort ToUShortChecked(uint value)
    {
        if (value > ushort.MaxValue)
        {
            throw new OverflowException($"Value {value} doesn't fit into ushort.");
        }
        
        return (ushort)value;
    }

    public static ushort ToUShortChecked(int value)
    {
        if (value is < ushort.MinValue or > ushort.MaxValue)
        {
            throw new OverflowException($"Value {value} doesn't fit into ushort.");
        }
        
        return (ushort)value;
    }

    public static short ToShortChecked(int value)
    {
        if (value is < short.MinValue or > short.MaxValue)
        {
            throw new OverflowException($"Value {value} doesn't fit into short.");
        }
        
        return (short)value;
    }
    
    public static uint ToUIntChecked(long value)
    {
        if (value is < uint.MinValue or > uint.MaxValue)
        {
            throw new OverflowException($"Value {value} doesn't fit into uint.");
        }

        return (uint)value;
    }
}