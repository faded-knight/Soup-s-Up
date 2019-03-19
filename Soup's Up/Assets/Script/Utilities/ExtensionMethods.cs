using System.Text.RegularExpressions;

public static class ExtensionMethods
{
    public static string RemoveCloneSuffix(this string value)
    {
        return Regex.Replace(value, @"\(Clone\)", "");
    }
}
