namespace Blaj.ReMetin2Server.Common.Application.Utils;

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
}