using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using SoundSetter.OptionInternals;

namespace SoundSetter
{
    public class SoundSetterUI(VolumeControls vc, Configuration config)
    {
        private static readonly Vector4 HintColor = new(0.7f, 0.7f, 0.7f, 1.0f);

        public bool IsVisible { get; set; }

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
            ImGui.Text("Plugin Settings");

            ImGui.PushItemWidth(100f);
            var kItem1 = VirtualKey.EnumToIndex(config.ModifierKey);
            if (ImGui.Combo("##SoundSetterKeybind1", ref kItem1, VirtualKey.Names.Take(3).ToArray(), 3))
            {
                config.ModifierKey = VirtualKey.IndexToEnum(kItem1);
                config.Save();
            }

            ImGui.SameLine();
            var kItem2 = VirtualKey.EnumToIndex(config.MajorKey) - 3;
            if (ImGui.Combo("Keybind##SoundSetterKeybind2", ref kItem2, VirtualKey.Names.Skip(3).ToArray(),
                    VirtualKey.Names.Length - 3))
            {
                config.MajorKey = VirtualKey.IndexToEnum(kItem2) + 3;
                config.Save();
            }

            ImGui.PopItemWidth();

            var onlyCutscenes = config.OnlyShowInCutscenes;
            if (ImGui.Checkbox("Only enable keybind during cutscenes.##SoundSetterCutsceneOption", ref onlyCutscenes))
            {
                config.OnlyShowInCutscenes = onlyCutscenes;
                config.Save();
            }

            ImGui.TextColored(HintColor, "Use /ssconfig to reopen this window.");

            ImGui.Spacing();
            ImGui.Text("Sound Settings");

            var vcPlaySoundsWhileWindowIsNotActive = vc.PlaySoundsWhileWindowIsNotActive;
            var playSoundsWhileWindowIsNotActive = vcPlaySoundsWhileWindowIsNotActive != null && vcPlaySoundsWhileWindowIsNotActive.GetValue();
            if (ImGui.Checkbox("Play sounds while window is not active.", ref playSoundsWhileWindowIsNotActive))
            {
                vc.PlaySoundsWhileWindowIsNotActive?.SetValue(playSoundsWhileWindowIsNotActive);
            }

            ImGui.Indent();
            ImGui.BeginDisabled(!playSoundsWhileWindowIsNotActive);
            {
                if (ImGui.BeginTable("SoundSetterWhileInactiveOptions", 2))
                {
                    ImGui.TableNextColumn();
                    ToggleCheckboxControl("BGM", vc.PlaySoundsWhileWindowIsNotActiveBGM);

                    ImGui.TableNextColumn();
                    ToggleCheckboxControl("Sound Effects", vc.PlaySoundsWhileWindowIsNotActiveSoundEffects);

                    ImGui.TableNextColumn();
                    ToggleCheckboxControl("Voice", vc.PlaySoundsWhileWindowIsNotActiveVoice);

                    ImGui.TableNextColumn();
                    ToggleCheckboxControl("System Sounds", vc.PlaySoundsWhileWindowIsNotActiveSystemSounds);

                    ImGui.TableNextColumn();
                    ToggleCheckboxControl("Ambient Sounds", vc.PlaySoundsWhileWindowIsNotActiveAmbientSounds);

                    ImGui.TableNextColumn();
                    ToggleCheckboxControl("Performance", vc.PlaySoundsWhileWindowIsNotActivePerformance);

                    ImGui.EndTable();
                }
            }
            ImGui.EndDisabled();
            ImGui.Unindent();

            ToggleCheckboxControl("Play music when mounted.", vc.PlayMusicWhenMounted);
            ToggleCheckboxControl("Enable normal battle music.", vc.EnableNormalBattleMusic);
            ToggleCheckboxControl("Enable city-state BGM in residential areas.", vc.EnableCityStateBGM);
            ToggleCheckboxControl("Play system sounds while waiting for Duty Finder.", vc.PlaySystemSounds);

            ImGui.Text("Volume Settings");

            MainVolumeControl("Master Volume", vc.MasterVolumeMuted, vc.MasterVolume);
            MainVolumeControl("BGM", vc.BgmMuted, vc.Bgm);
            MainVolumeControl("Sound Effects", vc.SoundEffectsMuted, vc.SoundEffects);
            MainVolumeControl("Voice", vc.VoiceMuted, vc.Voice);
            MainVolumeControl("System Sounds", vc.SystemSoundsMuted, vc.SystemSounds);
            MainVolumeControl("Ambient Sounds", vc.AmbientSoundsMuted, vc.AmbientSounds);
            MainVolumeControl("Performance", vc.PerformanceMuted, vc.Performance);

            ImGui.Text("Player Effects Volume");

            PlayerEffectVolumeControl("Self", vc.Self);
            PlayerEffectVolumeControl("Party", vc.Party);
            PlayerEffectVolumeControl("Other PCs", vc.OtherPCs);

            ImGui.Text("Equalizer");

            EQModeControl("Mode", vc.EqualizerMode);
        }

        private static void ToggleCheckboxControl(string label, BooleanOption? toggleOption)
        {
            var value = toggleOption != null && toggleOption.GetValue();
            if (ImGui.Checkbox(label, ref value))
            {
                toggleOption?.SetValue(value);
            }
        }

        private static void MainVolumeControl(string label, BooleanOption? muteOption, ByteOption? volumeOption)
        {
            var buttonSize = new Vector2(23, 23) * ImGui.GetIO().FontGlobalScale;

            using (ImRaii.PushFont(UiBuilder.IconFont))
            {
                var volumeMuted = muteOption != null && muteOption.GetValue();
                if (ImGui.Button(VolumeButtonName(volumeMuted, label), buttonSize))
                {
                    muteOption?.SetValue(!volumeMuted);
                }
            }

            ImGui.SameLine();
            if (volumeOption != null)
            {
                var volume = (int)volumeOption.GetValue();
                if (ImGui.SliderInt(label, ref volume, 0, 100))
                {
                    volumeOption.SetValue((byte)volume);
                }
            }
        }

        private static void PlayerEffectVolumeControl(string label, ByteOption? volumeOption)
        {
            if (volumeOption != null)
            {
                var value = (int)volumeOption.GetValue();
                if (ImGui.SliderInt(label, ref value, 0, 100))
                {
                    volumeOption.SetValue((byte)value);
                }
            }
        }

        private static void EQModeControl(string label, EqualizerModeOption? modeOption)
        {
            if (modeOption != null)
            {
                var eqMode = (int)modeOption.GetValue();
                if (ImGui.Combo(label, ref eqMode, EqualizerMode.Names, EqualizerMode.Names.Length))
                {
                    modeOption.SetValue((EqualizerMode.Enum)eqMode);
                }
            }
        }

        private static string VolumeButtonName(bool state, string internalName)
        {
            var icon = state ? FontAwesomeIcon.VolumeOff.ToIconString() : FontAwesomeIcon.VolumeUp.ToIconString();
            var idSuffix = internalName.Replace(" ", "");
            return $"{icon}##SoundSetterVolumeButton_{idSuffix}";
        }
    }
}