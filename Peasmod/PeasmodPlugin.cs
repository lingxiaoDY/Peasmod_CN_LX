using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using BepInEx.Logging;
using PeasAPI;
using PeasAPI.Managers;
using UnityEngine;

namespace Peasmod
{
    [BepInPlugin(Id, "Peasmod", PluginVersion)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(PeasAPI.PeasAPI.Id)]
    public class PeasmodPlugin : BasePlugin
    {
        public const string Id = "tk.peasplayer.peasmod";

        public const string PluginName = "Peasmod";
        public const string PluginAuthor = "Peasplayer#2541";
        public const string PluginVersion = "3.0.0-pre2.1";

        public Harmony Harmony { get; } = new Harmony(Id);
        
        public static ManualLogSource Logger { get; private set; }
        
        public static ConfigFile ConfigFile { get; private set; }

        public override void Load()
        {
            Logger = Log;
            ConfigFile = Config;

            WatermarkManager.AddWatermark($" | {PluginName} v{PluginVersion} {PeasAPI.Utility.StringColor.Green} by {PluginAuthor}", $" | {PluginName} v{PluginVersion}\n{PeasAPI.Utility.StringColor.Green} by {PluginAuthor}", 
                new Vector3(0f, -0.3f),  new Vector3(-0.9f, 0f));
            
            CustomServerManager.RegisterServer("Peaspowered", "au.peasplayer.tk", 22023);
            CustomServerManager.RegisterServer("matux.fr", "152.228.160.91", 22023);
            CustomServerManager.RegisterServer("Miniduikboot's Server", "impostor.duikbo.at", 22023);
            
            UpdateManager.RegisterGitHubUpdateListener("Peasplayer", "Peasmod");
            
            CustomHatManager.RegisterNewVisor("DreamMask", "Peasmod.Resources.Hats.DreamMask.png", new Vector2(0f, 0.2f));
            CustomHatManager.RegisterNewVisor("PeasMask", "Peasmod.Resources.Hats.PeasMask.png", new Vector2(0f, 0.2f));
            CustomHatManager.RegisterNewHat("Sitting Tux", "Peasmod.Resources.Hats.Tux.png", new Vector2(0f, 0.2f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.Tuxb.png"));
            CustomHatManager.RegisterNewHat("Laying Tux", "Peasmod.Resources.Hats.Tux2.png", new Vector2(0f, 0.2f), true, true, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.Tux2b.png"));
            CustomHatManager.RegisterNewHat("KristalCrown", "Peasmod.Resources.Hats.KristalCrown.png");
            CustomHatManager.RegisterNewHat("Elf Hat", "Peasmod.Resources.Hats.Elf.png", new Vector2(0f, 0.2f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.Elfb.png"));
            CustomHatManager.RegisterNewHat("Santa", "Peasmod.Resources.Hats.Santa.png", new Vector2(0f, 0.3f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.Santab.png"));
            CustomHatManager.RegisterNewHat("Christmas Tree", "Peasmod.Resources.Hats.XmasTree.png", new Vector2(0f, 0.2f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.XmasTreeb.png"));
            CustomHatManager.RegisterNewHat("Christmas Sock", "Peasmod.Resources.Hats.Sock.png", new Vector2(0f, 0.2f), true, false, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Hats.Sockb.png"));
            
            CustomColorManager.RegisterCustomColor(new Color(59 / 255f, 47 / 255f, 47 / 255f), "深咖啡");
            CustomColorManager.RegisterCustomColor(new Color(102/ 255f, 93 / 255f, 30 / 255f), "古铜");
            CustomColorManager.RegisterCustomColor(new Color(139 / 255f, 128 / 255f, 0 / 255f), "深黄");
            CustomColorManager.RegisterCustomColor(new Color(232 / 255f, 163 / 255f, 23 / 255f), "校车黄");
            CustomColorManager.RegisterCustomColor(new Color(195 / 255f, 142 / 255f, 199/ 255f), "紫龙");
            CustomColorManager.RegisterCustomColor(new Color(97 / 255f, 64 / 255f, 81 / 255f), "茄紫");
            CustomColorManager.RegisterCustomColor(new Color(106 / 255f, 251 / 255f, 146 / 255f), "青龙");
            CustomColorManager.RegisterCustomColor(new Color(137 / 255f, 195 / 255f, 92 / 255f), "豌豆绿");
            CustomColorManager.RegisterCustomColor(new Color(78 / 255f, 226 / 255f, 236 / 255f), "宝石蓝");
            CustomColorManager.RegisterCustomColor(new Color(53 / 255f, 126 / 255f, 199 / 255f), "天蓝");
            CustomColorManager.RegisterCustomColor(new Color(82/ 255f, 208/ 255f, 23 / 255f), "青豆");
            
            CustomColorManager.RegisterCustomColor(new Color(176 / 255f, 196 / 255f, 222 / 255f), "浅钢蓝");
            CustomColorManager.RegisterCustomColor(new Color(102 / 255f, 205 / 255f, 170 / 255f), "宝石浅蓝");
            CustomColorManager.RegisterCustomColor(new Color(139 / 255f, 0 / 255f, 139 / 255f), "深品");
            CustomColorManager.RegisterCustomColor(new Color(107 / 225f, 142 / 255f, 35 / 255f), "橄榄绿");
            CustomColorManager.RegisterCustomColor(new Color(220 / 255f, 20 / 255f, 60 / 255f), "绯红");
            CustomColorManager.RegisterCustomColor(new Color(218 / 255f, 165 / 255f, 32 / 255f), "黄花");
            CustomColorManager.RegisterCustomColor(new Color(255 / 255f, 250 / 255f, 179 / 250f), "雪白");
            CustomColorManager.RegisterCustomColor(new CustomColorManager.AUColor(new Color(65 / 255f, 32 / 255f, 43 / 255f), new Color(50 / 255f, 39 / 255f, 49 / 255f), "深橄榄"));
            CustomColorManager.RegisterCustomColor(new CustomColorManager.AUColor(new Color(45 / 255f, 37 / 255f, 69 / 255f), new Color(135 / 255f, 179 / 255f, 155 / 255f), "腐烂"));
            CustomColorManager.RegisterCustomColor(new Color(245 / 255f, 222 / 255f, 179 / 255f), "麦黄");
            CustomColorManager.RegisterCustomColor(new CustomColorManager.AUColor(new Color(65 / 255f, 32 / 255f, 43 / 255f), new Color(50 / 255f, 39 / 255f, 49 / 255f), "深橄榄"));
            
            Settings.Load();
            
            Harmony.PatchAll();
        }
    }
}