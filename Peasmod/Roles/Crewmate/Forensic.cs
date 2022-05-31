using System;
using System.Collections.Generic;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Managers;
using PeasAPI.Options;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Crewmate;

[RegisterCustomRole]
public class Forensic : BaseRole
{
    public Forensic(BasePlugin plugin) : base(plugin) { }

    public override string Name => "法医";
    public override string Description => "分析船员的血液";
    public override string LongDescription => "提取船员的血样进行分析，找出他们的阵营";
    public override string TaskText => "分析船员的血液";
    public override Color Color => ModdedPalette.ForensicColor;
    public override Visibility Visibility => Visibility.NoOne;
    public override Team Team => Team.Crewmate;
    public override bool HasToDoTasks => true;
    public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
    {
        {
            "AnalyseCooldown", new CustomNumberOption("analysecooldown", "分析冷却时间", 30, 180, 1, 30, NumberSuffixes.Seconds) {AdvancedRoleOption = true}
        },
        {
            "AnalyseDuration", new CustomNumberOption("analyseduration", "分析持续时间", 30, 180, 1, 30, NumberSuffixes.Seconds) {AdvancedRoleOption = true}
        }/*,
        {
            "AnalyseCount", new CustomNumberOption("analysecount", "Analyses", 1, 15, 1, 2, NumberSuffixes.None) {AdvancedRoleOption = true}
        }*/
    };

    public CustomButton Button;
    public byte BloodSample;

    public override void OnGameStart()
    {
        BloodSample = Byte.MaxValue;
        Button = CustomButton.AddButton(() =>
            {
                BloodSample = PlayerControl.LocalPlayer.FindClosestTarget(true).PlayerId;
            }, ((CustomNumberOption) AdvancedOptions["AnalyseCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"),
            p => p.IsRole(this) && !p.Data.IsDead, p => PlayerControl.LocalPlayer.FindClosestTarget(true) != null, text: "<size=40%>血液\n分析", textOffset: new Vector2(0f, 0.5f),
            onEffectEnd: () =>
            {
                var target = BloodSample.GetPlayer();
                BloodSample = Byte.MaxValue;
                var team = target.GetRole() == null ? target.Data.Role.IsImpostor ? "坏人" : "好人" :
                    target.GetRole().Team == Team.Crewmate ? "好人" :
                    target.GetRole().Team == Team.Impostor ? "坏人" : "中立";
                TextMessageManager.ShowMessage($"分析结果显示 {target.Data.PlayerName} 是 {team}", 3f);
            }, effectDuration: ((CustomNumberOption) AdvancedOptions["AnalyseDuration"]).Value, target: CustomButton.TargetType.Player, targetColor: Color);
    }
}