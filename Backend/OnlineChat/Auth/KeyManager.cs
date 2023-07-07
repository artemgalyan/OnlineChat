using System.Security.Cryptography;

namespace OnlineChat;

public class KeyManager
{
    private const string KeyFile = "key"; 
    
    public KeyManager()
    {
        RsaKey = RSA.Create();
        if (File.Exists(KeyFile))
        {
            RsaKey.ImportRSAPrivateKey(File.ReadAllBytes(KeyFile), out _);
        }
        else
        {
            var key = RsaKey.ExportRSAPrivateKey();
            File.WriteAllBytes(KeyFile, key);
        }
    }
    
    public RSA RsaKey { get; }
}