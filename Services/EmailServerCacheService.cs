using Mail.Helpers;
using Mail.Model;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Mail.Services;

public class EmailServerCacheService
{
    private EmailServerCache[]? _emailServerCache;

    public bool TryLoad()
    {
        if (!ResourceHelper.TryGetResourceTextFile("Mail.Resources.emailServerCache.xml", out string? text))
            return false;

        try
        {
            _emailServerCache = XMLHelper.DeserializeFromString<EmailServerCache[]>(text);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public bool TryGet(string domain, [MaybeNullWhen(false)] out EmailServerCache? emailServerCache)
    {
        emailServerCache = _emailServerCache?.FirstOrDefault(cache => cache.EmailDomain.Contains(domain));
        return emailServerCache != null;
    }
}