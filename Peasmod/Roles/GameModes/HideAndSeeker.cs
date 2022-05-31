﻿using System;
using BepInEx.IL2CPP;
using PeasAPI.Components;
using PeasAPI.Roles;
using Peasmod.GameModes;
using UnityEngine;

namespace Peasmod.Roles.GameModes
{
    [RegisterCustomRole]
    public class HideAndSeeker : BaseRole
    {
        public HideAndSeeker(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "寻觅者";
        public override string Description => "找到所有玩家";
        public override string LongDescription => "";
        public override string TaskText => "找到所有玩家";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Crewmate;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => false;
        public override int Count => 3;
        public override int MaxCount => 3;
        public override bool CreateRoleOption => false;
        public override Type[] GameModeWhitelist { get; } = {
            typeof(HideAndSeek)
        };
        public override bool CanKill(PlayerControl victim = null) => true;
        public override bool CanVent => Settings.HideAndSeekSeekerVenting.Value;
    }
}