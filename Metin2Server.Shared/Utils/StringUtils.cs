namespace Metin2Server.Shared.Utils;

public static class StringUtils
{
    public static string NormalizeChar(char[] chars)
    {
        return new string(chars).Trim().ToLower().Replace("\0", "");
    }

    public static char[] NormalizeString(string str, int length)
    {
        var strArray = new char[length];
        var strChars = str.ToCharArray();
        var lengthToCopy = Math.Min(strChars.Length, length);

        for (var i = 0; i < lengthToCopy; i++)
        {
            strArray[i] = strChars[i];
        }

        return strArray;
    }

    public static char[] EmptyChars(int length)
    {
        var chars = new char[length];
        chars[0] = '\0';
        return chars;
    }
    
    public static string FromCBuffer(char[] buf)
    {
        var n = Array.IndexOf(buf, '\0');
        
        if (n < 0)
        {
            n = buf.Length;
        }
        
        return new string(buf, 0, n);
    }
    
    public static char[] ToCBuffer(string str, int length)
    {
        var buffer = new char[length];

        if (!string.IsNullOrEmpty(str))
        {
            var maxPayload = length - 1;
            var count = Math.Min(str.Length, maxPayload);
            
            for (var i = 0; i < count; i++)
            {
                buffer[i] = str[i];
            }
            
            buffer[count] = '\0';
        }
        else
        {
            buffer[0] = '\0';
        }

        return buffer;
    }
}