﻿using System.Security.Cryptography;

namespace Constants;

public static class Security
{
    public const int SaltSize = 12;
    public const int OutputLength = 64;
    public const int Iterations = 500;
    public static readonly int InStringSaltSize = (int)Math.Floor(SaltSize * 8m / 6m);
    public static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;
    public const int MinPasswordLength = 1;
}