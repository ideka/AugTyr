public static class GenericExtensions
{
    public static bool SetIfDiff<T>(this T to, ref T it)
    {
        if (it?.Equals(to) ?? to == null)
            return false;
        it = to;
        return true;
    }
}