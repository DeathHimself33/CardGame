using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    // Required for C# 9 'record' and 'init' support on Unity's older runtime.
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}
