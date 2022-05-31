using BepInEx.IL2CPP;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomEndReason;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Neutral
{
    [RegisterCustomRole]
    public class Troll : BaseRole
    {
        public Troll(BasePlugin plugin) : base(plugin) { }

        public override string Name => "钓鱼人";
        public override string Description => "被一个伪装者杀死";
        public override string LongDescription => "";
        public override string TaskText => "被一个伪装者杀死";
        public override Color Color => ModdedPalette.TrollColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Alone;
        public override bool HasToDoTasks => false;

        public override bool PreKill(PlayerControl killer, PlayerControl victim)
        {
            if (victim.IsRole<Troll>() && victim.IsLocal())
                new CustomEndReason(victim);
            return true;
        }
    }
}