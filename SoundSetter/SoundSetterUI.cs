using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Bindings.ImGui;
using SoundSetter.OptionInternals;

namespace SoundSetter
{
    public class SoundSetterUI
    {
        private static readonly Vector4 HintColor = new(0.7f, 0.7f, 0.7f, 1.0f);

        private readonly Configuration config;
        private readonly VolumeControls vc;

        public bool IsVisible { get; set; }

        public SoundSetterUI(VolumeControls vc, Configuration config)
        {
            this.vc = vc;
            this.config = config;
        }

        /**
         * isInitialized seems to be true after reloading the plugin
         * for a single frame. The first time twice is incremented is
         * the plugin load frame, and the second time twice is incremented
         * is after initialization.
         */
        private int twice;

        /**
         * Returns the appropriate window flags to allow the user to resize
         * the window, but only after the plugin has been initialized, and
         * only after giving ImGui one frame to set the default window size.
         *
         * Technically, I could just set appropriate window sizes explicitly,
         * but then those would need to be maintained, and they would need to
         * account for global font scaling.
         */
        private ImGuiWindowFlags GetWindowFlags()
        {
            if (this.twice != 2)
            {
                this.twice++;
                return ImGuiWindowFlags.AlwaysAutoResize;
            }

            return ImGuiWindowFlags.None;
        }

        public void Draw()
        {
            if (!IsVisible)
                return;

            var pVisible = IsVisible;
            ImGui.Begin("SoundSetter Configuration", ref pVisible, GetWindowFlags());
            IsVisible = pVisible;

            Settings();

            ImGui.End();
        }

        private void Settings()
        {
            var buttonSize = new Vector2(23, 23) * ImGui.GetIO().FontGlobalScale;

            ImGui.Text("Plugin Settings");

            ImGui.PushItemWidth(100f);
            var kItem1 = VirtualKey.EnumToIndex(this.config.ModifierKey);
            if (ImGui.Combo("##SoundSetterKeybind1", ref kItem1, VirtualKey.Names.Take(3).ToArray(), 3))
            {
                this.config.ModifierKey = VirtualKey.IndexToEnum(kItem1);
                this.config.Save();
            }

            ImGui.SameLine();
            var kItem2 = VirtualKey.EnumToIndex(this.config.MajorKey) - 3;
            if (ImGui.Combo("Keybind##SoundSetterKeybind2", ref kItem2, VirtualKey.Names.Skip(3).ToArray(),
                    VirtualKey.Names.Length - 3))
            {
                this.config.MajorKey = VirtualKey.IndexToEnum(kItem2) + 3;
                this.config.Save();
            }

            ImGui.PopItemWidth();

            var onlyCutscenes = this.config.OnlyShowInCutscenes;
            if (ImGui.Checkbox("Only enable keybind during cutscenes.##SoundSetterCutsceneOption", ref onlyCutscenes))
            {
                this.config.OnlyShowInCutscenes = onlyCutscenes;
                this.config.Save();
            }

            ImGui.TextColored(HintColor, "Use /ssconfig to reopen this window.");

            ImGui.Spacing();
            ImGui.Text("Sound Settings");

            var vcPlaySoundsWhileWindowIsNotActive = this.vc.PlaySoundsWhileWindowIsNotActive;
            var playSoundsWhileWindowIsNotActive = vcPlaySoundsWhileWindowIsNotActive != null && vcPlaySoundsWhileWindowIsNotActive.GetValue();
            if (ImGui.Checkbox("Play sounds while window is not active.", ref playSoundsWhileWindowIsNotActive))
            {
                this.vc.PlaySoundsWhileWindowIsNotActive?.SetValue(playSoundsWhileWindowIsNotActive);
            }

            ImGui.Indent();
            ImGui.BeginDisabled(!playSoundsWhileWindowIsNotActive);
            {
                if (ImGui.BeginTable("SoundSetterWhileInactiveOptions", 2))
                {
                    ImGui.TableNextColumn();
                    var vcPlaySoundsWhileWindowIsNotActiveBgm = this.vc.PlaySoundsWhileWindowIsNotActiveBGM;
                    var playSoundsWhileWindowIsNotActiveBgm = vcPlaySoundsWhileWindowIsNotActiveBgm != null && vcPlaySoundsWhileWindowIsNotActiveBgm.GetValue();
                    if (ImGui.Checkbox("BGM", ref playSoundsWhileWindowIsNotActiveBgm))
                    {
                        this.vc.PlaySoundsWhileWindowIsNotActiveBGM?.SetValue(playSoundsWhileWindowIsNotActiveBgm);
                    }

                    ImGui.TableNextColumn();
                    var vcPlaySoundsWhileWindowIsNotActiveSoundEffects = this.vc.PlaySoundsWhileWindowIsNotActiveSoundEffects;
                    var playSoundsWhileWindowIsNotActiveSoundEffects =
                        vcPlaySoundsWhileWindowIsNotActiveSoundEffects != null && vcPlaySoundsWhileWindowIsNotActiveSoundEffects.GetValue();
                    if (ImGui.Checkbox("Sound Effects", ref playSoundsWhileWindowIsNotActiveSoundEffects))
                    {
                        this.vc.PlaySoundsWhileWindowIsNotActiveSoundEffects?.SetValue(
                            playSoundsWhileWindowIsNotActiveSoundEffects);
                    }

                    ImGui.TableNextColumn();
                    var vcPlaySoundsWhileWindowIsNotActiveVoice = this.vc.PlaySoundsWhileWindowIsNotActiveVoice;
                    var playSoundsWhileWindowIsNotActiveVoice =
                        vcPlaySoundsWhileWindowIsNotActiveVoice != null && vcPlaySoundsWhileWindowIsNotActiveVoice.GetValue();
                    if (ImGui.Checkbox("Voice", ref playSoundsWhileWindowIsNotActiveVoice))
                    {
                        this.vc.PlaySoundsWhileWindowIsNotActiveVoice?.SetValue(playSoundsWhileWindowIsNotActiveVoice);
                    }

                    ImGui.TableNextColumn();
                    var vcPlaySoundsWhileWindowIsNotActiveSystemSounds = this.vc.PlaySoundsWhileWindowIsNotActiveSystemSounds;
                    var playSoundsWhileWindowIsNotActiveSystemSounds =
                        vcPlaySoundsWhileWindowIsNotActiveSystemSounds != null && vcPlaySoundsWhileWindowIsNotActiveSystemSounds.GetValue();
                    if (ImGui.Checkbox("System Sounds", ref playSoundsWhileWindowIsNotActiveSystemSounds))
                    {
                        this.vc.PlaySoundsWhileWindowIsNotActiveSystemSounds?.SetValue(
                            playSoundsWhileWindowIsNotActiveSystemSounds);
                    }

                    ImGui.TableNextColumn();
                    var vcPlaySoundsWhileWindowIsNotActiveAmbientSounds = this.vc.PlaySoundsWhileWindowIsNotActiveAmbientSounds;
                    var playSoundsWhileWindowIsNotActiveAmbientSounds =
                        vcPlaySoundsWhileWindowIsNotActiveAmbientSounds != null && vcPlaySoundsWhileWindowIsNotActiveAmbientSounds.GetValue();
                    if (ImGui.Checkbox("Ambient Sounds", ref playSoundsWhileWindowIsNotActiveAmbientSounds))
                    {
                        this.vc.PlaySoundsWhileWindowIsNotActiveAmbientSounds?.SetValue(
                            playSoundsWhileWindowIsNotActiveAmbientSounds);
                    }

                    ImGui.TableNextColumn();
                    var vcPlaySoundsWhileWindowIsNotActivePerformance = this.vc.PlaySoundsWhileWindowIsNotActivePerformance;
                    var playSoundsWhileWindowIsNotActivePerformance =
                        vcPlaySoundsWhileWindowIsNotActivePerformance != null && vcPlaySoundsWhileWindowIsNotActivePerformance.GetValue();
                    if (ImGui.Checkbox("Performance", ref playSoundsWhileWindowIsNotActivePerformance))
                    {
                        this.vc.PlaySoundsWhileWindowIsNotActivePerformance?.SetValue(
                            playSoundsWhileWindowIsNotActivePerformance);
                    }

                    ImGui.EndTable();
                }
            }
            ImGui.EndDisabled();
            ImGui.Unindent();

            var vcPlayMusicWhenMounted = this.vc.PlayMusicWhenMounted;
            var playMusicWhenMounted = vcPlayMusicWhenMounted != null && vcPlayMusicWhenMounted.GetValue();
            if (ImGui.Checkbox("Play music when mounted.", ref playMusicWhenMounted))
            {
                this.vc.PlayMusicWhenMounted?.SetValue(playMusicWhenMounted);
            }

            var vcEnableNormalBattleMusic = this.vc.EnableNormalBattleMusic;
            var enableNormalBattleMusic = vcEnableNormalBattleMusic != null && vcEnableNormalBattleMusic.GetValue();
            if (ImGui.Checkbox("Enable normal battle music.", ref enableNormalBattleMusic))
            {
                this.vc.EnableNormalBattleMusic?.SetValue(enableNormalBattleMusic);
            }

            var vcEnableCityStateBgm = this.vc.EnableCityStateBGM;
            var enableCityStateBGM = vcEnableCityStateBgm != null && vcEnableCityStateBgm.GetValue();
            if (ImGui.Checkbox("Enable city-state BGM in residential areas.", ref enableCityStateBGM))
            {
                this.vc.EnableCityStateBGM?.SetValue(enableCityStateBGM);
            }

            var vcPlaySystemSounds = this.vc.PlaySystemSounds;
            var playSystemSounds = vcPlaySystemSounds != null && vcPlaySystemSounds.GetValue();
            if (ImGui.Checkbox("Play system sounds while waiting for Duty Finder.", ref playSystemSounds))
            {
                this.vc.PlaySystemSounds?.SetValue(playSystemSounds);
            }

            ImGui.Text("Volume Settings");

            ImGui.PushFont(UiBuilder.IconFont);
            var vcMasterVolumeMuted = this.vc.MasterVolumeMuted;
            var masterVolumeMuted = vcMasterVolumeMuted != null && vcMasterVolumeMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(masterVolumeMuted, nameof(masterVolumeMuted)), buttonSize))
            {
                this.vc.MasterVolumeMuted?.SetValue(!masterVolumeMuted);
            }

            ImGui.PopFont();
            ImGui.SameLine();
            var vcMasterVolume = this.vc.MasterVolume;
            if (vcMasterVolume != null)
            {
                var masterVolume = (int)vcMasterVolume.GetValue();
                if (ImGui.SliderInt("Master Volume", ref masterVolume, 0, 100))
                {
                    vcMasterVolume.SetValue((byte)masterVolume);
                }
            }

            ImGui.PushFont(UiBuilder.IconFont);
            var vcBgmMuted = this.vc.BgmMuted;
            var bgmMuted = vcBgmMuted != null && vcBgmMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(bgmMuted, nameof(bgmMuted)), buttonSize))
            {
                this.vc.BgmMuted?.SetValue(!bgmMuted);
            }

            ImGui.PopFont();
            ImGui.SameLine();
            var byteOption = this.vc.Bgm;
            if (byteOption != null)
            {
                var bgm = (int)byteOption.GetValue();
                if (ImGui.SliderInt("BGM", ref bgm, 0, 100))
                {
                    byteOption.SetValue((byte)bgm);
                }
            }

            ImGui.PushFont(UiBuilder.IconFont);
            var vcSoundEffectsMuted = this.vc.SoundEffectsMuted;
            var soundEffectsMuted = vcSoundEffectsMuted != null && vcSoundEffectsMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(soundEffectsMuted, nameof(soundEffectsMuted)), buttonSize))
            {
                this.vc.SoundEffectsMuted?.SetValue(!soundEffectsMuted);
            }

            ImGui.PopFont();
            ImGui.SameLine();
            var vcSoundEffects = this.vc.SoundEffects;
            if (vcSoundEffects != null)
            {
                var soundEffects = (int)vcSoundEffects.GetValue();
                if (ImGui.SliderInt("Sound Effects", ref soundEffects, 0, 100))
                {
                    vcSoundEffects.SetValue((byte)soundEffects);
                }
            }

            ImGui.PushFont(UiBuilder.IconFont);
            var vcVoiceMuted = this.vc.VoiceMuted;
            var voiceMuted = vcVoiceMuted != null && vcVoiceMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(voiceMuted, nameof(voiceMuted)), buttonSize))
            {
                this.vc.VoiceMuted?.SetValue(!voiceMuted);
            }

            ImGui.PopFont();
            ImGui.SameLine();
            var vcVoice = this.vc.Voice;
            if (vcVoice != null)
            {
                var voice = (int)vcVoice.GetValue();
                if (ImGui.SliderInt("Voice", ref voice, 0, 100))
                {
                    vcVoice.SetValue((byte)voice);
                }
            }

            ImGui.PushFont(UiBuilder.IconFont);
            var vcSystemSoundsMuted = this.vc.SystemSoundsMuted;
            var systemSoundsMuted = vcSystemSoundsMuted != null && vcSystemSoundsMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(systemSoundsMuted, nameof(systemSoundsMuted)), buttonSize))
            {
                this.vc.SystemSoundsMuted?.SetValue(!systemSoundsMuted);
            }

            ImGui.PopFont();
            ImGui.SameLine();
            var vcSystemSounds = this.vc.SystemSounds;
            if (vcSystemSounds != null)
            {
                var systemSounds = (int)vcSystemSounds.GetValue();
                if (ImGui.SliderInt("System Sounds", ref systemSounds, 0, 100))
                {
                    vcSystemSounds.SetValue((byte)systemSounds);
                }
            }

            ImGui.PushFont(UiBuilder.IconFont);
            var vcAmbientSoundsMuted = this.vc.AmbientSoundsMuted;
            var ambientSoundsMuted = vcAmbientSoundsMuted != null && vcAmbientSoundsMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(ambientSoundsMuted, nameof(ambientSoundsMuted)), buttonSize))
            {
                this.vc.AmbientSoundsMuted?.SetValue(!ambientSoundsMuted);
            }

            ImGui.PopFont();
            ImGui.SameLine();
            var vcAmbientSounds = this.vc.AmbientSounds;
            if (vcAmbientSounds != null)
            {
                var ambientSounds = (int)vcAmbientSounds.GetValue();
                if (ImGui.SliderInt("Ambient Sounds", ref ambientSounds, 0, 100))
                {
                    vcAmbientSounds.SetValue((byte)ambientSounds);
                }
            }

            ImGui.PushFont(UiBuilder.IconFont);
            var vcPerformanceMuted = this.vc.PerformanceMuted;
            var performanceMuted = vcPerformanceMuted != null && vcPerformanceMuted.GetValue();
            if (ImGui.Button(VolumeButtonName(performanceMuted, nameof(performanceMuted)), buttonSize))
            {
                this.vc.PerformanceMuted?.SetValue(!performanceMuted);
            }

            ImGui.PopFont();
            ImGui.SameLine();
            var vcPerformance = this.vc.Performance;
            if (vcPerformance != null)
            {
                var performance = (int)vcPerformance.GetValue();
                if (ImGui.SliderInt("Performance", ref performance, 0, 100))
                {
                    vcPerformance.SetValue((byte)performance);
                }
            }

            ImGui.Text("Player Effects Volume");

            var vcSelf = this.vc.Self;
            if (vcSelf != null)
            {
                var self = (int)vcSelf.GetValue();
                if (ImGui.SliderInt("Self", ref self, 0, 100))
                {
                    vcSelf.SetValue((byte)self);
                }
            }

            var vcParty = this.vc.Party;
            if (vcParty != null)
            {
                var party = (int)vcParty.GetValue();
                if (ImGui.SliderInt("Party", ref party, 0, 100))
                {
                    vcParty.SetValue((byte)party);
                }
            }

            var vcOtherPCs = this.vc.OtherPCs;
            if (vcOtherPCs != null)
            {
                var others = (int)vcOtherPCs.GetValue();
                if (ImGui.SliderInt("Other PCs", ref others, 0, 100))
                {
                    vcOtherPCs.SetValue((byte)others);
                }
            }

            ImGui.Text("Equalizer");

            var equalizerModeOption = this.vc.EqualizerMode;
            if (equalizerModeOption != null)
            {
                var eqMode = (int)equalizerModeOption.GetValue();
                if (ImGui.Combo("Mode", ref eqMode, EqualizerMode.Names, EqualizerMode.Names.Length))
                {
                    equalizerModeOption.SetValue((EqualizerMode.Enum)eqMode);
                }
            }
        }

        private static string VolumeButtonName(bool state, string internalName)
        {
            return
                $"{(state ? FontAwesomeIcon.VolumeOff.ToIconString() : FontAwesomeIcon.VolumeUp.ToIconString())}##SoundSetter{internalName}";
        }
    }
}