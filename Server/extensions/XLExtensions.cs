public static class XLExtensions
{
    public static IEnumerable<string> SplitInGroups(this string original, int size)
    {
        var p = 0;
        var l = original.Length;
        while (l - p > size)
        {
            yield return original.Substring(p, size);
            p += size;
        }
        yield return original.Substring(p);
    }
    public static string Encrypt(this string clearText)
    {
        //I DON'T LIKE HAVING ENCRYPTION KEY HERE. CHANGE LATTER
        string EncryptionKey = "MAKV2SPBNI99212";
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return clearText;
    }
}