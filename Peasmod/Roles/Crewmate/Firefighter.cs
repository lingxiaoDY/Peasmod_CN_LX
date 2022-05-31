using System.Collections.Generic;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Options;
using PeasAPI.Roles;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles.Crewmate
{
    [RegisterCustomRole]
    public class Firefighter : BaseRole
    {
        public Firefighter(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "消防员";
        public override string Description => "拯救船员于水火之中";
        public override string LongDescription => "";
        public override string TaskText => "通过修复伪装者的破坏行为，将船员从灾难中拯救出来";
        public override Color Color => ModdedPalette.FirefighterColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;
        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "FixCooldown", new CustomNumberOption("FirefighterFixCooldown", "修理冷却时间", 20, 60, 1, 20, NumberSuffixes.Seconds)
            },
            {
                "FirefighterMaxUses", new CustomNumberOption("FirefighterMaxUses", "修理次数", 1, 10, 1, 2, NumberSuffixes.None)
            }
        };

        public CustomButton Button;
        public int TimesFixed;

        public override void OnGameStart()
        {
            Button = CustomButton.AddButton(
                () =>
                {
                    RpcRepairSabotage(PlayerControl.LocalPlayer);
                },
                ((CustomNumberOption) AdvancedOptions["FixCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Default.png"), p => p.IsRole(this) && !p.Data.IsDead, 
                _ => ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().AnyActive && TimesFixed < ((CustomNumberOption)AdvancedOptions["FirefighterMaxUses"]).Value, text: "<size=40%>修复");
        }

        [MethodRpc((uint) CustomRpcCalls.RepairSabotage)]
        public static void RpcRepairSabotage(PlayerControl sender)
        {
            try
            {
                var systems = new SystemTypes[]
                {
                    SystemTypes.Electrical,
                    SystemTypes.Comms,
                    SystemTypes.LifeSupp,
                    SystemTypes.Reactor,
                    SystemTypes.Laboratory
                };
                foreach (var system in systems)
                {
                    if (system == SystemTypes.Electrical)
                    {
                        var light = ShipStatus.Instance.Systems[system].Cast<SwitchSystem>();
                        light.ActualSwitches = light.ExpectedSwitches;
                        continue;
                    }
                    ShipStatus.Instance.RepairSystem(system, PlayerControl.LocalPlayer, 0 | 16);
                    ShipStatus.Instance.RepairSystem(system, PlayerControl.LocalPlayer, 1 | 16);
                    ShipStatus.Instance.RepairSystem(system, PlayerControl.LocalPlayer, 16);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}