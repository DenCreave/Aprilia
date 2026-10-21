using System;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Aprilia.Engines.Security;
/// <summary>
/// it exists against crawlers and indexers.
/// </summary>
public static class ImageDecryptor
{
    public static Bitmap LoadEncImage(string resourcePath)
    {
        var uri = new Uri($"avares://Aprilia/{resourcePath}");
        using var encryptedStream = AssetLoader.Open(uri);

        using var memoryStream = new MemoryStream();
        encryptedStream.CopyTo(memoryStream);
        byte[] cipherText = memoryStream.ToArray();

        var key = Convert.FromHexString("417072696C696121417072696C696121417072696C696121417072696C696121");
        var iv = Convert.FromHexString("417072696C696121417072696C696121");
        var parameters = new ParametersWithIV(new KeyParameter(key), iv);

        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());
        cipher.Init(false, parameters);

        byte[] plainText = new byte[cipher.GetOutputSize(cipherText.Length)];
        int len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);
        len += cipher.DoFinal(plainText, len);

        using var decryptedStream = new MemoryStream(plainText, 0, len);
        return new Bitmap(decryptedStream);
    }
}