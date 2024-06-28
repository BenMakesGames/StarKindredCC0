using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace StarKindred.Common.Services;

public interface IPassphraseHasher
{
    string Hash(string plainText);
    bool Verify(string plainText, string hash);
}

// mostly copy-pasted out of MS's code, but with a less-inconvenient interface :P
public class PassphraseHasher: IPassphraseHasher
{
    private const int IterationCount = 100000;

    private RandomNumberGenerator RNG { get; }

    public PassphraseHasher()
    {
        RNG = RandomNumberGenerator.Create();
    }

    public string Hash(string plainText) =>
        Convert.ToHexString(HashPasswordV3(plainText));

    public bool Verify(string plainText, string hash) =>
        VerifyHashedPasswordV3(Convert.FromHexString(hash), plainText);

    private byte[] HashPasswordV3(string password) =>
        HashPasswordV3(
            password,
            KeyDerivationPrf.HMACSHA512,
            IterationCount,
            128 / 8,
            256 / 8);

    private byte[] HashPasswordV3(string password, KeyDerivationPrf prf, int iterCount, int saltSize, int numBytesRequested)
    {
        // Produce a version 3 (see comment above) text hash.
        byte[] salt = new byte[saltSize];
        RNG.GetBytes(salt);
        byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

        var outputBytes = new byte[13 + salt.Length + subkey.Length];
        outputBytes[0] = 0x01; // format marker
        WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
        WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
        WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
        Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
        Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
        return outputBytes;
    }
    
    private static bool VerifyHashedPasswordV3(byte[] hashedPassword, string password)
    {
        try
        {
            // Read header information
            var prf = (KeyDerivationPrf) ReadNetworkByteOrder(hashedPassword, 1);
            var iterCount = (int) ReadNetworkByteOrder(hashedPassword, 5);
            int saltLength = (int) ReadNetworkByteOrder(hashedPassword, 9);

            // Read the salt: must be >= 128 bits
            if (saltLength < 128 / 8)
            {
                return false;
            }

            byte[] salt = new byte[saltLength];
            Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

            // Read the subkey (the rest of the payload): must be >= 128 bits
            int subkeyLength = hashedPassword.Length - 13 - salt.Length;
            if (subkeyLength < 128 / 8)
            {
                return false;
            }

            byte[] expectedSubkey = new byte[subkeyLength];
            Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            // Hash the incoming password and verify it
            byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);

            return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
        }
        catch
        {
            // This should never occur except in the case of a malformed payload, where
            // we might go off the end of the array. Regardless, a malformed payload
            // implies verification failed.
            return false;
        }
    }

    private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
    {
        buffer[offset + 0] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)(value >> 0);
    }

    private static uint ReadNetworkByteOrder(byte[] buffer, int offset) =>
        ((uint)(buffer[offset + 0]) << 24) |
        ((uint)(buffer[offset + 1]) << 16) |
        ((uint)(buffer[offset + 2]) << 8) |
        // ReSharper disable once RedundantCast
        ((uint)(buffer[offset + 3]));
}