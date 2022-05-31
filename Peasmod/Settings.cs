using HarmonyLib;
using PeasAPI;
using PeasAPI.Options;
using UnhollowerBaseLib;
using UnityEngine;

namespace Peasmod
{
    public static class Settings
    {
        /*
         * This are the unicode symboles I used to have. I keep them here in case I need them again.
         * •; └; └──
         */

        public static CustomOptionHeader Header =
            new CustomOptionHeader(Utility.StringColor.Green + "\nPeasmod" + Utility.StringColor.Reset);

        public static readonly CustomOptionButton
            SectionGeneral = new CustomOptionButton("general", "˅ 常规设置", false);

        public static CustomOptionHeader GeneralHeader = new CustomOptionHeader("常规");

        public static readonly CustomToggleOption Venting = new CustomToggleOption("venting",
            $"• {Palette.CrewmateBlue.GetTextColor()}通风管道{Utility.StringColor.Reset}", true);

        public static readonly CustomToggleOption ReportBodys =
            new CustomToggleOption("reporting",
                $"• {Palette.CrewmateBlue.GetTextColor()}尸体报告{Utility.StringColor.Reset}", true);

        public static readonly CustomToggleOption Sabotaging =
            new CustomToggleOption("sabotaging",
                $"• {Palette.CrewmateBlue.GetTextColor()}破坏{Utility.StringColor.Reset}", true);

        public static readonly CustomToggleOption CrewVenting =
            new CustomToggleOption("crewventing",
                $"• {Palette.CrewmateBlue.GetTextColor()}船员通风管道{Utility.StringColor.Reset}", false);

        public static readonly CustomOptionButton SectionModes =
            new CustomOptionButton("ModeSettings", "˅ 小游戏模式", false);

        public static CustomOptionHeader ModesHeader =
            new CustomOptionHeader($"游戏模式");

        public static CustomOptionHeader HideAndSeek =
            new CustomOptionHeader($"捉迷藏");

        public static readonly CustomNumberOption HideAndSeekSeekerCooldown =
            new CustomNumberOption("hideandseekseekercooldown", "• 寻觅者击杀冷却", 20, 60, 1, 20, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption HideAndSeekSeekerDuration =
            new CustomNumberOption("hideandseekseekerduration", "• 游戏时间", 30, 300, 10, 120, NumberSuffixes.Seconds);

        public static readonly CustomToggleOption HideAndSeekSeekerVenting =
            new CustomToggleOption("hideandseekseekerventing", "• 可使用通风管道", false);
        
        public static CustomOptionHeader PropHunt =
            new CustomOptionHeader($"猎杀者");

        public static readonly CustomNumberOption PropHuntSeekerCooldown =
            new CustomNumberOption("prophuntseekercooldown", "• 猎手冷却", 20, 60, 1, 20, NumberSuffixes.Seconds);

        public static readonly CustomNumberOption PropHuntSeekerDuration =
            new CustomNumberOption("prophuntseekerduration", "• 游戏时间", 30, 300, 10, 120, NumberSuffixes.Seconds);
        
        public static readonly CustomNumberOption PropHuntSeekerClickCooldown =
            new CustomNumberOption("prophuntseekerclickcooldown", "• 猎手击杀冷却", 1, 60, 1, 5, NumberSuffixes.Seconds);

        public static CustomOptionHeader GodImpostor =
            new CustomOptionHeader($"伪装者之神");

        public static readonly CustomToggleOption VentBuilding =
            new CustomToggleOption("ventbuilding", $"• 设置通风管道", false);

        public static readonly CustomToggleOption BodyDragging =
            new CustomToggleOption("bodydragging", $"• 拖动尸体", false);

        public static readonly CustomToggleOption Invisibility =
            new CustomToggleOption("invisibility", $"• 隐身", false);

        public static readonly CustomToggleOption Freeze =
            new CustomToggleOption("freeze", $"• 冻结", false);

        public static readonly CustomToggleOption Morphing =
            new CustomToggleOption("morphing", $"• 化形", false);

        public static readonly CustomNumberOption MorphingCooldown =
            new CustomNumberOption("morphingcooldown", $"└ 化形冷却", 20, 60, 1, 20, NumberSuffixes.Seconds);

        public static void Load()
        {
            SectionGeneralListener(false);
            SectionModesListener(false);

            GeneralHeader.MenuVisible = false;
            ModesHeader.MenuVisible = false;
            SectionGeneral.HudVisible = false;
            SectionModes.HudVisible = false;
            HideAndSeek.HudVisible = false;
            GodImpostor.HudVisible = false;
        }

        public static void SectionGeneralListener(bool value)
        {
            Venting.MenuVisible = value;
            ReportBodys.MenuVisible = value;
            Sabotaging.MenuVisible = value;
            CrewVenting.MenuVisible = value;
        }

        public static void SectionModesListener(bool value)
        {
            HideAndSeek.MenuVisible = value;
            HideAndSeekSeekerCooldown.MenuVisible = value;
            HideAndSeekSeekerDuration.MenuVisible = value;
            HideAndSeekSeekerVenting.MenuVisible = value;
            PropHunt.MenuVisible = value;
            PropHuntSeekerCooldown.MenuVisible = value;
            PropHuntSeekerDuration.MenuVisible = value;
            PropHuntSeekerClickCooldown.MenuVisible = value;
            GodImpostor.MenuVisible = value;
            VentBuilding.MenuVisible = value;
            BodyDragging.MenuVisible = value;
            Invisibility.MenuVisible = value;
            Freeze.MenuVisible = value;
            Morphing.MenuVisible = value;
            MorphingCooldown.MenuVisible = value;
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
        public static class GameSettingMenuPatch
        {
            static void Prefix(GameSettingMenu __instance)
            {
                __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);

                SectionGeneral.OnValueChanged += args => { SectionGeneralListener(args.NewValue); };

                SectionModes.OnValueChanged += args => { SectionModesListener(args.NewValue); };
            }
        }
    }
}