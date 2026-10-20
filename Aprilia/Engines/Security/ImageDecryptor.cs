using System;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PgpCore;

namespace Aprilia.Engines.Security;
/// <summary>
/// it exists against crawlers and indexers.
/// </summary>
public static class ImageDecryptor
{
    public static Bitmap LoadGpgImage(string resourcePath, string spell)
    {
        var uri = new Uri($"avares://Aprilia/{resourcePath}");
        using var encryptedStream = AssetLoader.Open(uri);
        using var decryptedStream = new MemoryStream();

        var keys = new EncryptionKeys(spell);
        
        using (var pgp = new PGP(keys))
        {
            pgp.DecryptStream(encryptedStream, decryptedStream);
        }

        decryptedStream.Position = 0;
        return new Bitmap(decryptedStream);
    }
}