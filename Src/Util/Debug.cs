namespace GGJ2024.Util;

using System;
using Godot;

internal static class Debug
{
    internal static void Assert(bool cond, string msg)
    {
#if DEBUG
        if (cond) return;

        GD.PrintErr(msg);
        throw new ApplicationException($"Assert Failed: {msg}");
#endif
    }
}