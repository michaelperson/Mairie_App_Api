namespace Mairie.API.Helpers
{
    using System.Text;
    using System.Security.Cryptography;
    public static class SecretManager
    {
        /// <summary>
        /// Permet d'encrypté une chaine de caractere en utilisant DPAPI
        /// </summary>
        /// <param name="plainText">Le texte qui devra être sécurisé</param>
        /// <returns>Une string de la valeur cryptée (base64)</returns>
        public static string Encrypt(string plainText, DataProtectionScope scope)
        {
            //1 - tranformer en tableau de byte le texte
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            //2 - Appeler la méthode Protect de DPAPI
#pragma warning disable CA1416 // Validate platform compatibility
            byte[] encryptedBytes = ProtectedData.Protect(
                bytes,null, scope);
#pragma warning restore CA1416 // Validate platform compatibility
            //3 - Transformer le tableau de byte chiffré en chaine de caractere
            return Convert.ToBase64String(encryptedBytes);
        }
        /// <summary>
        /// Permet de décrypter une chaine de caractère en utilisant DPAPI
        /// </summary>
        /// <param name="encryptedText">LE texte a décrypter</param>
        /// <returns>Le texte en clair</returns>
        public static string Decrypt(string encryptedText, DataProtectionScope scope)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
#pragma warning disable CA1416 // Validate platform compatibility
            byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, scope);
#pragma warning restore CA1416 // Validate platform compatibility
            return Encoding.UTF8.GetString(decryptedBytes);

        }
    }
}
