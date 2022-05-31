﻿using System.Collections.Generic;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Managers;
using PeasAPI.Options;
using PeasAPI.Roles;
using Reactor.Networking;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles.Impostor
{
    [RegisterCustomRole]
    public class Glaciater : BaseRole
    {
        public Glaciater(BasePlugin plugin) : base(plugin)
        {
            Instance = this;
        }

        public override string Name => "冻结者";
        public override Sprite Icon => Utility.CreateSprite("Peasmod.Resources.Buttons.Freezing.png", 851f);
        public override string Description => "阻止船员移动";
        public override string LongDescription => "";
        public override string TaskText => "阻止船员移动";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Impostor;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => false;
        public override int MaxCount => 3;
        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "FreezeCooldown", new CustomNumberOption("freezecooldown", "冻结冷却", 20, 60, 1, 20, NumberSuffixes.Seconds)
            },
            {
                "FreezeDuration", new CustomNumberOption("freezeduration", "冻结持续时间", 10, 30, 1, 10, NumberSuffixes.Seconds)
            }
        };
        public override bool CanVent => true;
        public override bool CanKill(PlayerControl victim = null) => !victim || !victim.Data.Role.IsImpostor;
        public override bool CanSabotage(SystemTypes? sabotage) => true;

        public static Glaciater Instance;

        public CustomButton Button;
        public bool IsFrozen;

        public override void OnGameStart()
        {
            IsFrozen = false;
            Button = CustomButton.AddButton(() => {
                    RpcFreeze(PlayerControl.LocalPlayer, true);
                }, ((CustomNumberOption) AdvancedOptions["FreezeCooldown"]).Value,
                Utility.CreateSprite("Peasmod.Resources.Buttons.Freezing.png", 851f), p => p.IsRole(this) && !p.Data.IsDead, _ => true,
                effectDuration: ((CustomNumberOption) AdvancedOptions["FreezeDuration"]).Value, onEffectEnd: () => {
                    RpcFreeze(PlayerControl.LocalPlayer, false);
                }, text: "<size=40%>冻结");
        }

        public override void OnUpdate()
        {
            var player = PlayerControl.LocalPlayer;
            if (IsFrozen && !player.Data.Role.IsImpostor && !player.Data.IsDead && PeasAPI.PeasAPI.GameStarted)
            {
                if (Minigame.Instance != null)
                    Minigame.Instance.ForceClose();
                player.moveable = false;
                player.MyPhysics.ResetAnimState();
                player.MyPhysics.ResetMoveState();
                player.MyPhysics.body.velocity = Vector2.zero;
            }
        }

        [MethodRpc((uint) CustomRpcCalls.Freeze, LocalHandling = RpcLocalHandling.After)]
        public static void RpcFreeze(PlayerControl sender, bool enable)
        {
            Instance.IsFrozen = enable;
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                PlayerControl.LocalPlayer.moveable = !enable;
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && !sender.IsLocal())
                TextMessageManager.ShowMessage(enable ? "所有人都冻住了" : "所有人都解冻了", 1f);
        }
    }
}