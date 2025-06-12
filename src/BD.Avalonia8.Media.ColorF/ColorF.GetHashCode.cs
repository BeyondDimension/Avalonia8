namespace BD.Avalonia8.Media;

partial struct ColorF // HashCode
{
    /// <inheritdoc/>
    public override int GetHashCode()
    {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return HashCode.Combine(Red, Green, Blue, Alpha);
#else
        unchecked
        {
            int hashcode = Red.GetHashCode();
            hashcode = (hashcode * 397) ^ Green.GetHashCode();
            hashcode = (hashcode * 397) ^ Blue.GetHashCode();
            hashcode = (hashcode * 397) ^ Alpha.GetHashCode();
            return hashcode;
        }
#endif
    }
}