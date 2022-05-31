using System.Collections.Generic;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Options;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Impostor;

[RegisterCustomRole]
public class EvilForensic : BaseRole
{
    public EvilForensic(BasePlugin plugin) : base(plugin)
    {
    }

    public override string Name => "邪恶法医";
    public override string Description => "给船员下毒";
    public override string LongDescription => "在船员的血液中注入毒药，在短时间内就能杀死他们";
    public override string TaskText => "给船员下毒";
    public override Color Color => Palette.ImpostorRed;
    public override Visibility Visibility => Visibility.Impostor;
    public override Team Team => Team.Impostor;
    public override bool HasToDoTasks => false;
    public override int MaxCount => 3;
    public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
    {
        {
            "PoisonCooldown", new CustomNumberOption("poisoncooldown", "下毒冷却", 30, 180, 1, 30, NumberSuffixes.Seconds)
        },
        {
            "PoisonDuration", new CustomNumberOption("poisonduration", "下毒持续时间", 20, 60, 1, 20, NumberSuffixes.Seconds)
        }
    };
    public override bool CanVent => true;
    public override bool CanKill(PlayerControl victim = null) => !victim || !victim.Data.Role.IsImpostor;
    public override bool CanSabotage(SystemTypes? sabotage) => true;

    public CustomButton Button;
    public byte PoisonVictim;
    
    public override void OnGameStart()
    {
        PoisonVictim = byte.MaxValue;
        Button = CustomButton.AddButton(() =>
            {
                PoisonVictim = PlayerControl.LocalPlayer.FindClosestTarget(true).PlayerId;
            }, ((CustomNumberOption) AdvancedOptions["PoisonCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"),
            p => p.IsRole(this) && !p.Data.IsDead, p => PlayerControl.LocalPlayer.FindClosestTarget(true) != null, text: "<size=40%>下毒", textOffset: new Vector2(0f, 0.5f),
            onEffectEnd: () =>
            {
                var target = PoisonVictim.GetPlayer();
                PoisonVictim = byte.MaxValue;
                target.RpcMurderPlayer(target);
            }, effectDuration: ((CustomNumberOption) AdvancedOptions["PoisonDuration"]).Value, target: CustomButton.TargetType.Player, targetColor: Color);
    }
}