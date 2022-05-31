﻿using System.Collections.Generic;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.Options;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Crewmate
{
    [RegisterCustomRole]
    public class Sheriff : BaseRole
    {
        public Sheriff(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "警长";
        public override string Description => "处决伪装者";
        public override string LongDescription => "";
        public override string TaskText => "处决伪装者";
        public override Color Color => ModdedPalette.SheriffColor;
        public override Team Team => Team.Crewmate;
        public override Visibility Visibility => Visibility.NoOne;
        public override bool HasToDoTasks => true;
        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "CanKillNeutrals", new CustomToggleOption("sheriffkillneutrals", "可以击杀中立阵营", false)
            }
        };
        public override bool CanKill(PlayerControl victim = null) => true;

        public override void OnKill(PlayerControl killer, PlayerControl victim)
        {
            if (killer.IsRole(this) && killer.IsLocal() && !victim.IsLocal())
                if (!(victim.Data.Role.IsImpostor || victim.GetRole() != null && (victim.GetRole().Team == Team.Role ||
                        victim.GetRole().Team == Team.Alone) &&
                    ((CustomToggleOption) AdvancedOptions["CanKillNeutrals"]).Value))
                    PlayerControl.LocalPlayer.RpcMurderPlayer(PlayerControl.LocalPlayer);
        }
    }
}