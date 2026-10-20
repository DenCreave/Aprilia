using System.Collections.Generic;
using Avalonia.Media.Imaging;

namespace Aprilia.Engines.Security;

public class EncVault
{
    private readonly Dictionary<string, Bitmap> _cache = new();

    public EncVault()
    {
        string q = "Aprilia<3";
        
        _cache["evolution"] = ImageDecryptor.LoadGpgImage("Assets/evolution.gpg", q);
        _cache["flipflap"] = ImageDecryptor.LoadGpgImage("Assets/flipflap.gpg", q);
        _cache["outside"] = ImageDecryptor.LoadGpgImage("Assets/outside.gpg", q);
        _cache["easter_egg"] = ImageDecryptor.LoadGpgImage("Assets/easter_egg.gpg", q);
    }

    public Bitmap GetImage(string key)
    {
        return _cache[key];
    }

}