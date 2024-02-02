namespace Mail.Model;

public class EmailServerCache
{
    public string[] EmailDomain { get; set; } = [];
    public string SmtpServer { get; set; } = string.Empty;
    public uint Port { get; set; } = 0;

    public EmailServerCache() { }

    public EmailServerCache(string[] emailDomain, string smtpServer, uint port)
    {
        EmailDomain = emailDomain;
        SmtpServer = smtpServer;
        Port = port;
    }
}