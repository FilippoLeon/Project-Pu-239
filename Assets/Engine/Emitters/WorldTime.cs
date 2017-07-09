using MoonSharp.Interpreter;
using System;

[MoonSharpUserData]
public class WorldTime
{
    private int elapsed = 0;

    public override string ToString()
    {
        return Convert.ToString(elapsed);
    }

    public void Tic()
    {
        elapsed++;
    }
}