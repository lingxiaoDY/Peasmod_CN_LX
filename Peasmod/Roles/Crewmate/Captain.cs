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
    public class Captain : BaseRole
    {
        public Captain(BasePlugin plugin) : base(plugin) { }

        public override string Name => "舰长";
        public override Sprite Icon => Utility.CreateSprite("Peasmod.Resources.Buttons.CallMeeting.png", 650f);
        public override string Description => "保护你的船员";
        public override string LongDescription => "";
        public override string TaskText => "保护你的船员";
        public override Color Color => ModdedPalette.CaptainColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;

        public override Dictionary<string, CustomOption> AdvancedOptions { get; set; } = new Dictionary<string, CustomOption>()
        {
            {
                "CallCooldown",
                new CustomNumberOption("captaincooldown", "报警冷却", 10, 60, 1, 20, NumberSuffixes.Seconds)
            }
        };

        public CustomButton Button;

        public override void OnGameStart()
        {
            Button = CustomButton.AddButton(() =>
                {
                    PlayerControl.LocalPlayer.CmdReportDeadBody(null);
                }, ((CustomNumberOption) AdvancedOptions["CallCooldown"]).Value,
                Utility.CreateSprite("Peasmod.Resources.Buttons.CallMeeting.png", 650f), p => p.IsRole(this) && !p.Data.IsDead, _ => true, text: "<size=40%>报警");
        }
    }
}