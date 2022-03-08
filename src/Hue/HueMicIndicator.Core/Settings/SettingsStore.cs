using System;
using System.IO;
using System.Text.Json;

namespace HueMicIndicator.Core.Settings;

public static class SettingsStore
{
    private static readonly object ioLock = new();
    private static readonly Lazy<DirectoryInfo> settingsDirectory = new(DirectoryFactory);

    private static DirectoryInfo DirectoryFactory()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var path = Path.Combine(localAppData, "HueMicIndicator", "settings");

        return new DirectoryInfo(path);
    }

    private static FileInfo GetSubFile(string sub)
    {
        var directory = settingsDirectory.Value;
        directory.Create();

        var path = Path.Combine(directory.FullName, sub);

        return new FileInfo(path);
    }

    public static T Get<T>(string sub)
        where T : new()
    {
        lock (ioLock)
        {
            return GetSync<T>(sub, out _);
        }
    }

    private static T GetSync<T>(string sub, out FileInfo file)
        where T : new()
    {
        file = GetSubFile(sub);

        if (!file.Exists)
            return new T();

        using Stream stream = file.OpenRead();
        return JsonSerializer.Deserialize<T>(stream) ?? new T();
    }

    public static T Set<T>(string sub, Func<T, T> update)
        where T : new()
    {
        lock (ioLock)
        {
            var initial = GetSync<T>(sub, out var file);
            var updated = update(initial);

            using Stream stream = file.Create();
            JsonSerializer.Serialize(stream, updated);

            return updated;
        }
    }

    public static void Reset(string sub)
    {
        lock (ioLock)
        {
            var file = GetSubFile(sub);

            if (file.Exists)
                file.Delete();
        }
    }
}