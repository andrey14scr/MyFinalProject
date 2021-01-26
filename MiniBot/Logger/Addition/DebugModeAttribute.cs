using System;

namespace LogInfo
{
    public class DebugModeAttribute : Attribute
    {
        public bool IsDebugMode { get; private set; }

        public DebugModeAttribute()
        {
            IsDebugMode = true;
        }

        public DebugModeAttribute(bool debugMode)
        {
            IsDebugMode = debugMode;
        }
    }
}
