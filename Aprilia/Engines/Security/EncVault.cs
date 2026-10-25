using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace Aprilia.Engines.Security;

public class EncVault
{
    private readonly Dictionary<string, Bitmap> _cache = new();

    public EncVault()
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();
        _cache["evolution"] = ImageDecryptor.LoadEncImage("Assets/Metia/evolution");
        _cache["flipflap"] = ImageDecryptor.LoadEncImage("Assets/Metia/flipflap");
        _cache["outside"] = ImageDecryptor.LoadEncImage("Assets/Metia/outside");
        _cache["easter_egg"] = ImageDecryptor.LoadEncImage("Assets/Metia/easter_egg");
        _cache["sky"] = ImageDecryptor.LoadEncImage("Assets/Metia/sky");
        timer.Stop();
        Console.WriteLine($"total ellapsed timer for decryption: {timer.ElapsedMilliseconds} ms");
    }

    public Bitmap GetImage(string key)
    {
        return _cache[key];
    }

}