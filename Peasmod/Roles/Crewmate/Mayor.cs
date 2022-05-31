﻿using System.Linq;
using BepInEx.IL2CPP;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.Roles;
using UnityEngine;

namespace Peasmod.Roles.Crewmate
{
    [RegisterCustomRole]
    public class Mayor : BaseRole
    {
        public Mayor(BasePlugin plugin) : base(plugin) { }

        public override string Name => "市长";
        public override string Description => "你的票数算作两票";
        public override string LongDescription => "";
        public override string TaskText => "你的票数算作两票，请合理使用!";
        public override Color Color => ModdedPalette.MayorColor;
        public override Visibility Visibility => Visibility.NoOne;
        public override Team Team => Team.Crewmate;
        public override bool HasToDoTasks => true;

        private static Dictionary<byte, int> CalculateVotes(MeetingHud __instance)
        {
            Dictionary<byte, int> results = new Dictionary<byte, int>();

            foreach (var playerVoteArea in __instance.playerStates)
            {
                if (playerVoteArea.VotedFor != 252 && playerVoteArea.VotedFor != 255 && playerVoteArea.VotedFor != 254)
                {
                    if (results.ContainsKey(playerVoteArea.VotedFor))
                    {
                        int num = results[playerVoteArea.VotedFor];
                        results[playerVoteArea.VotedFor] = num + 1;

                        if (playerVoteArea.TargetPlayerId.GetPlayer().IsRole<Mayor>())
                            results[playerVoteArea.VotedFor] = num + 2;
                    }
                    else
                    {
                        results[playerVoteArea.VotedFor] = 1;

                        if (playerVoteArea.TargetPlayerId.GetPlayer().IsRole<Mayor>())
                            results[playerVoteArea.VotedFor] = 2;
                    }
                }
            }

            return results;
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CheckForEndVoting))]
        public static class MeetingHudCheckEndPatch
        {
            public static bool Prefix(MeetingHud __instance)
            {
                if (__instance.playerStates.All(ps => ps.AmDead || ps.DidVote))
                {
                    Dictionary<byte, int> self = CalculateVotes(__instance);
                    bool tie;
                    KeyValuePair<byte, int> max = global::Extensions.MaxPair(self, out tie);
                    GameData.PlayerInfo exiled = GameData.Instance.AllPlayers.ToArray()
                        .FirstOrDefault(v => !tie && v.PlayerId == max.Key);
                    MeetingHud.VoterState[] array = new MeetingHud.VoterState[__instance.playerStates.Length];
                    for (int i = 0; i < __instance.playerStates.Length; i++)
                    {
                        PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                        array[i] = new MeetingHud.VoterState
                        {
                            VoterId = playerVoteArea.TargetPlayerId,
                            VotedForId = playerVoteArea.VotedFor
                        };
                    }

                    __instance.RpcVotingComplete(array, exiled, tie);
                }

                return false;
            }
        }
    }
}