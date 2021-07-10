using System;

namespace FPG
{
    [Flags]
    public enum Logger
    {
        Console = 0,
        Firebase = 1,
        Facebook = 2,
        RDS = 4,
        Tenjin = 8,
        Adjust = 16
    }

    public static class EventLogger
    {

    }
}