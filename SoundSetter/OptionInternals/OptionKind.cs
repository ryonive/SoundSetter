﻿namespace SoundSetter.OptionInternals
{
    public enum OptionKind : ulong
    {
        PlayMusicWhenMounted = 18,
        EnableNormalBattleMusic,
        EnableCityStateBGM,
        PlaySystemSounds,

        Master = 23,
        Bgm,
        SoundEffects,
        Voice,
        SystemSounds,
        AmbientSounds,
        Performance,

        Self,
        Party,
        OtherPCs,

        MasterMuted,
        BgmMuted,
        SoundEffectsMuted,
        VoiceMuted,
        SystemSoundsMuted,
        AmbientSoundsMuted,
        PerformanceMuted,

        EqualizerMode = 49,
    }
}