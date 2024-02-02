using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace Mail.Helpers;

public static class ResourceHelper
{
    public static bool TryGetResourceTextFile(string filePath, [MaybeNullWhen(false)] out string text)
    {
        text = null;

        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath);
        if (null == stream)
            return false;

        using StreamReader sr = new(stream);
        text = sr.ReadToEnd();

        return true;
    }
}
