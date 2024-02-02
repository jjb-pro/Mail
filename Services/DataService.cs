using Mail.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Mail.Services;

public abstract class DataService<T> where T : class
{
    public T[] Values { get; private set; } = [];

    protected abstract string? FileName { get; }

    private static readonly string applicationDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"jjb\Mail");

    public bool TryLoad()
    {
        try
        {
            CreateApplicationDataPath();

            var data = XMLHelper.Deserialize<T[]>(Path.Combine(applicationDataPath, FileName!));
            if (null != data)
                Values = data;
            else
                return false;
        }
        catch
        {
            return false;
        }

        return true;
    }

    public async Task OverwriteAndSaveAsync(T[] values)
    {
        Values = values;

        CreateApplicationDataPath();
        await Task.Run(() => XMLHelper.Serialize(values, Path.Combine(applicationDataPath, FileName!)));
    }

    private static void CreateApplicationDataPath() => Directory.CreateDirectory(applicationDataPath);
}