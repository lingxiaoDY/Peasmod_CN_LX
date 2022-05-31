﻿using System;
using BepInEx.IL2CPP;
using PeasAPI.Components;
using PeasAPI.Roles;
using Peasmod.GameModes;
using UnityEngine;

namespace Peasmod.Roles.GameModes
{
    [RegisterCustomRole]
    public class PropHunter : BaseRole
    {
        public PropHunter(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "猎手";
        public override string Description => "寻找你的猎物";
        public override string LongDescription => "";
        public override string TaskText => "寻找你的猎物";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Crewmate;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => false;
        public override int Count => 3;
        public override int MaxCount => 3;
        public override bool CreateRoleOption => false;
        public override Type[] GameModeWhitelist { get; } = {
            typeof(PropHunt)
        };
        public override bool CanKill(PlayerControl victim = null) => true;
        public override bool CanSabotage(SystemTypes? sabotage) => false;
    }
}