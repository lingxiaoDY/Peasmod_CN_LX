using System.Collections.Generic;
using System.Linq;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Managers;
using PeasAPI.Options;
using PeasAPI.Roles;
using Reactor.Extensions;
using UnityEngine;

namespace Peasmod.Roles.Crewmate
{
    [RegisterCustomRole]
    public class Foresight : BaseRole
    {
        public Foresight(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "预言家";
        public override string Description => ((CustomStringOption) AdvancedOptions["RevealVariant"]).Value == 0 ? "通告指定玩家为船员" : "揭露玩家阵营";
        public override string LongDescription => "";
        public override string TaskText => ((CustomStringOption) AdvancedOptions["RevealVariant"]).Value == 0 ? "通告指定玩家为船员" : "揭露玩家阵营";
        public override Color Color => ModdedPalette.ForesightColor;

        public override Visibility Visibility =>
            ((CustomStringOption) AdvancedOptions["RevealVariant"]).Value == 0 ? Visibility.NoOne : UsedAbility == 0 ? Visibility.NoOne : Visibility.Impostor;

        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;
        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "RevealCooldown", new CustomNumberOption("foresightcooldown", "预言冷却", 10, 120, 1, 20, NumberSuffixes.Seconds) {AdvancedRoleOption = true}
            },
            {
                "RevealCount", new CustomNumberOption("foresightreveals", "预言", 1, 15, 1, 2, NumberSuffixes.None) {AdvancedRoleOption = true}
            },
            {
                "RevealVariant", new CustomStringOption("foresightvariants", "揭露提示", "安全", "有击杀能力") {AdvancedRoleOption = true}
            },
            {
                "RevealTarget", new CustomStringOption("foresighttarget", "选择玩家 (揭露提示 B)", "随机", "指定列表", "指定范围") {AdvancedRoleOption = true}
            }
        };

        public CustomButton Button;
        public int UsedAbility;
        public List<byte> AlreadyRevealed;

        public override void OnGameStart()
        {
            UsedAbility = 0;
            AlreadyRevealed = new List<byte>();
            Button = CustomButton.AddButton(() =>
                {
                    if (((CustomStringOption) AdvancedOptions["RevealVariant"]).Value == 0)
                    {
                        var player = Utility.GetAllPlayers().Where(p =>
                            !p.Data.Role.IsImpostor &&
                            (p.Data.GetRole() == null ||
                             p.Data.GetRole() != null && p.GetRole().Team == Team.Crewmate) &&
                            !p.Data.IsDead && !p.IsLocal() && !AlreadyRevealed.Contains(p.PlayerId)).Random();
                        if (player != null)
                        {
                            TextMessageManager.ShowMessage($"你看到的 {player.Data.PlayerName} 是船员", 3f);
                            AlreadyRevealed.Add(player.PlayerId);
                        }
                        else
                            TextMessageManager.ShowMessage("你无法看到船员", 3f);

                        UsedAbility++;
                    }
                    else
                    {
                        if (((CustomStringOption) AdvancedOptions["RevealTarget"]).Value == 0)
                        {
                            var player = Utility.GetAllPlayers().Where(p =>
                                !p.Data.IsDead && !p.IsLocal() && !AlreadyRevealed.Contains(p.PlayerId)).Random();
                            if (player != null)
                            {
                                var team = player.GetRole() == null ? player.Data.Role.IsImpostor ? "坏人" : "好人" :
                                    player.GetRole().Team == Team.Crewmate ? "好人" :
                                    player.GetRole().Team == Team.Impostor ? "坏人" : "中立";
                                TextMessageManager.ShowMessage($"你看到的 {player.Data.PlayerName} 是 {team}", 3f);
                                AlreadyRevealed.Add(player.PlayerId);
                            }
                            else
                                TextMessageManager.ShowMessage("你无法再看到任何人的信息", 3f);

                            UsedAbility++;
                        }
                        else if (((CustomStringOption) AdvancedOptions["RevealTarget"]).Value == 1)
                        {
                            PlayerMenuManager.OpenPlayerMenu(Utility.GetAllPlayers()
                                .Where(p => !p.Data.IsDead && !p.IsLocal())
                                .ToList().ConvertAll(p => p.PlayerId), p =>
                            {
                                var team = p.GetRole() == null ? p.Data.Role.IsImpostor ? "坏人" : "好人" :
                                    p.GetRole().Team == Team.Crewmate ? "好人" :
                                    p.GetRole().Team == Team.Impostor ? "坏人" : "中立";
                                TextMessageManager.ShowMessage($"You see that {p.Data.PlayerName} is {team}", 3f);

                                UsedAbility++;
                            }, () => Button.SetCoolDown(0));
                        }
                        else if (((CustomStringOption) AdvancedOptions["RevealTarget"]).Value == 2)
                        {
                            var p = PlayerControl.LocalPlayer.FindClosestTarget(true);
                            var team = p.GetRole() == null ? p.Data.Role.IsImpostor ? "坏人" : "好人" :
                                p.GetRole().Team == Team.Crewmate ? "好人" :
                                p.GetRole().Team == Team.Impostor ? "坏人" : "中立";
                            TextMessageManager.ShowMessage($"You see that {p.Data.PlayerName} is {team}", 3f);

                            UsedAbility++;
                        }
                    }
                }, ((CustomNumberOption) AdvancedOptions["RevealCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"),
                p => p.IsRole(this) && !p.Data.IsDead,
                p => UsedAbility < ((CustomNumberOption) AdvancedOptions["RevealCount"]).Value && (((CustomStringOption) AdvancedOptions["RevealTarget"]).Value != 2 && ((CustomStringOption) AdvancedOptions["RevealVariant"]).Value == 1 ||
                                                                       PlayerControl.LocalPlayer
                                                                           .FindClosestTarget(true) != null),
                text: "<size=40%>预言", textOffset: new Vector2(0f, 0.5f));
        }
    }
}