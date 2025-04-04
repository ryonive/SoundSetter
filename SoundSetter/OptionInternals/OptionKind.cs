﻿namespace SoundSetter.OptionInternals;

/// <summary>
/// Config option IDs as understood by the game's UI.
/// </summary>
public enum OptionKind : ulong
{
    PlaySoundsWhileWindowIsNotActive = 17,

    PlayMusicWhenMounted,
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

    PlaySoundsWhileWindowIsNotActiveBGM = 43,
    PlaySoundsWhileWindowIsNotActiveSoundEffects,
    PlaySoundsWhileWindowIsNotActiveVoice,
    PlaySoundsWhileWindowIsNotActiveSystemSounds,
    PlaySoundsWhileWindowIsNotActiveAmbientSounds,
    PlaySoundsWhileWindowIsNotActivePerformance,

    EqualizerMode = 49,
}