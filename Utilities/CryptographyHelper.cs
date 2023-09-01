// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Security.Cryptography;
using System.Text;

namespace BlazorComponentLibrary.Utilities;

/// <summary>
/// Utility class for cryptographic operations.
/// Provides hashing, encryption, and token generation capabilities.
/// Uses modern, secure algorithms for all operations.
/// </summary>
public static class CryptographyHelper
{
    /// <summary>
    /// Generates SHA256 hash of input string.
    /// Used for password hashing and data integrity verification.
    /// </summary>
    public static string GenerateSHA256Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentException("Input cannot be null or empty", nameof(input));

        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(hashBytes);
        }
    }

    /// <summary>
    /// Generates HMAC-SHA256 hash using a secret key.
    /// Used for message authentication and API signatures.
    /// </summary>
    public static string GenerateHMACSHA256(string message, string secretKey)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("Message cannot be null or empty", nameof(message));

        if (string.IsNullOrEmpty(secretKey))
            throw new ArgumentException("Secret key cannot be null or empty", nameof(secretKey));

        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
        {
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return Convert.ToHexString(hashBytes);
        }
    }

    /// <summary>
    /// Generates a random token suitable for authentication and verification.
    /// Uses cryptographically secure random number generator.
    /// </summary>
    public static string GenerateSecureToken(int lengthInBytes = 32)
    {
        if (lengthInBytes <= 0)
            throw new ArgumentException("Length must be greater than 0", nameof(lengthInBytes));

        using (var rng = RandomNumberGenerator.Create())
        {
            var tokenData = new byte[lengthInBytes];
            rng.GetBytes(tokenData);
            return Convert.ToBase64String(tokenData);
        }
    }

    /// <summary>
    /// Generates a cryptographically secure random number.
    /// Used for generating secure IDs and nonces.
    /// </summary>
    public static int GenerateSecureRandomNumber(int minValue, int maxValue)
    {
        if (minValue >= maxValue)
            throw new ArgumentException("minValue must be less than maxValue");

        using (var rng = RandomNumberGenerator.Create())
        {
            var randomData = new byte[4];
            rng.GetBytes(randomData);
            var randomValue = BitConverter.ToInt32(randomData, 0) & 0x7FFFFFFF;
            return minValue + (randomValue % (maxValue - minValue));
        }
    }

    /// <summary>
    /// Generates a random alphanumeric string of specified length.
    /// Useful for temporary passwords and verification codes.
    /// </summary>
    public static string GenerateRandomString(int length, string charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
    {
        if (length <= 0)
            throw new ArgumentException("Length must be greater than 0", nameof(length));

        using (var rng = RandomNumberGenerator.Create())
        {
            var result = new StringBuilder(length);
            var data = new byte[4];

            for (int i = 0; i < length; i++)
            {
                rng.GetBytes(data);
                var randomIndex = BitConverter.ToInt32(data, 0) & 0x7FFFFFFF;
                result.Append(charset[randomIndex % charset.Length]);
            }

            return result.ToString();
        }
    }

    /// <summary>
    /// Generates a verification code (numeric) of specified length.
    /// Typically used for OTP and email verification.
    /// </summary>
    public static string GenerateVerificationCode(int length = 6)
    {
        if (length <= 0)
            throw new ArgumentException("Length must be greater than 0", nameof(length));

        return GenerateRandomString(length, "0123456789");
    }

    /// <summary>
    /// Encrypts string using AES algorithm.
    /// Requires matching key length (16, 24, or 32 bytes for 128, 192, 256-bit encryption).
    /// </summary>
    public static string EncryptAES(string plainText, string encryptionKey)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("Plain text cannot be null or empty", nameof(plainText));

        if (string.IsNullOrEmpty(encryptionKey))
            throw new ArgumentException("Encryption key cannot be null or empty", nameof(encryptionKey));

        var keyBytes = Encoding.UTF8.GetBytes(encryptionKey);

        using (var aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var encryptor = aes.CreateEncryptor())
            {
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                var result = new byte[aes.IV.Length + encryptedBytes.Length];
                Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

                return Convert.ToBase64String(result);
            }
        }
    }

    /// <summary>
    /// Decrypts AES-encrypted string.
    /// Requires the same key used for encryption.
    /// </summary>
    public static string DecryptAES(string encryptedText, string encryptionKey)
    {
        if (string.IsNullOrEmpty(encryptedText))
            throw new ArgumentException("Encrypted text cannot be null or empty", nameof(encryptedText));

        if (string.IsNullOrEmpty(encryptionKey))
            throw new ArgumentException("Encryption key cannot be null or empty", nameof(encryptionKey));

        var encryptedBytes = Convert.FromBase64String(encryptedText);
        var keyBytes = Encoding.UTF8.GetBytes(encryptionKey);

        using (var aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var iv = new byte[aes.IV.Length];
            Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor())
            {
                var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, iv.Length, encryptedBytes.Length - iv.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }

    /// <summary>
    /// Validates a hash against a plaintext value.
    /// Useful for password verification without storing plaintext.
    /// </summary>
    public static bool VerifyHash(string plainText, string hash)
    {
        if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(hash))
            return false;

        var computedHash = GenerateSHA256Hash(plainText);
        return string.Equals(computedHash, hash, StringComparison.OrdinalIgnoreCase);
    }
}
