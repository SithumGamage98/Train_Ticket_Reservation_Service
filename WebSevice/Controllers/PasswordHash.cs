/*
 * File: PasswordHasher.cs
 * Description: This file contains the PasswordHasher class which handles password hashing and verification.
 */

using System;
using System.Security.Cryptography;
using System.Text;

public class PasswordHasher
{
    /*
     * Hashes the given password using a salt and the PBKDF2 algorithm.
     * Returns the hashed password as a base64-encoded string.
     */
    public static string HashPassword(string password)
    {
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);

        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        return Convert.ToBase64String(hashBytes);
    }

    /*
     * Verifies a password by comparing the saved password hash with a newly hashed version of the entered password.
     * Returns true if the passwords match, otherwise false.
     */
    public static bool VerifyPassword(string savedPasswordHash, string password)
    {
        byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);

        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);

        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 16] != hash[i])
            {
                return false;
            }
        }

        return true;
    }
}