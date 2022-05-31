﻿using System.Collections.Generic;
using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Options;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Crewmate
{
    [RegisterCustomRole]
    public class Cloak : BaseRole
    {
        public Cloak(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "隐藏者";
        public override Sprite Icon => Utility.CreateSprite("Peasmod.Resources.Buttons.Hide.png", 794f);
        public override string Description => "你可以隐身";
        public override string LongDescription => "";
        public override string TaskText => "用你的隐身能力找出伪装者";
        public override Color Color => ModdedPalette.CloakColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;
        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "InvisibilityCooldown", new CustomNumberOption("cloakcooldown", "隐身冷却时间", 20, 60, 1, 20, NumberSuffixes.Seconds)
            },
            {
                "InvisibilityDuration", new CustomNumberOption("cloakduration", "隐身持续时间", 10, 60, 1, 10, NumberSuffixes.Seconds)
            }
        };

        public CustomButton Button;

        public override void OnGameStart()
        {
            Button = CustomButton.AddButton(
                () => { PlayerControl.LocalPlayer.RpcGoInvisible(true); },
                ((CustomNumberOption) AdvancedOptions["InvisibilityCooldown"]).Value, Utility.CreateSprite("Peasmod.Resources.Buttons.Hide.png", 794f), p => p.IsRole(this) && !p.Data.IsDead, _ => true, effectDuration: ((CustomNumberOption) AdvancedOptions["InvisibilityDuration"]).Value,
                onEffectEnd: () => { PlayerControl.LocalPlayer.RpcGoInvisible(false); }, text: "<size=40%>隐身");
        }
    }
}