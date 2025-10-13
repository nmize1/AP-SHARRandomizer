namespace SHARMemory.SHAR;

/// <summary>
/// A collection of SHAR helper functions.
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Takes a string and return a key value corresponding to it. It is a running hash, i.e. finding the hash of the part of the string, then passing that value in and hashing the rest will yield the same result as hashing the whole string at once.
    /// </summary>
    /// <param name="pToken">The string to hash.</param>
    /// <param name="keyValue">The initial key value.</param>
    /// <returns>A key corresponding to the given string.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Radical's naming")]
    public static ulong radMakeKey(string pToken, ulong keyValue = 0)
    {
        if (pToken == null || pToken.Length == 0)
            return keyValue;

        int firstNull = pToken.IndexOf('\0');
        if (firstNull == -1)
            firstNull = pToken.Length;

        for (int i = 0; i < firstNull; i++)
        {
            char c = pToken[i];
            keyValue *= 65599;
            keyValue ^= c;
        }

        return keyValue;
    }

    /// <summary>
    /// Takes a string and return a key value corresponding to it.
    /// </summary>
    /// <param name="x">The string to hash.</param>
    /// <returns>A key corresponding to the given string.</returns>
    public static long MakeUID(string x) => (long)radMakeKey(x);
}
