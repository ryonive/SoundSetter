﻿namespace SoundSetter
{
    public static class EqualizerMode
    {
        public static readonly string[] Names = { "Standard", "Bass Boost", "Treble Boost", "Voice Boost", "Logitech Pro-G 50mm" };

        public enum Enum
        {
            Standard,
            BassBoost,
            TrebleBoost,
            VoiceBoost,
            LogitechProG50Mm,
        }
    }
}