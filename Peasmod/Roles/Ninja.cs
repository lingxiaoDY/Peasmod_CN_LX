﻿using System.Collections.Generic;
using BepInEx.IL2CPP;
using HarmonyLib;
using PeasAPI;
using PeasAPI.Components;
using PeasAPI.CustomButtons;
using PeasAPI.Roles;
using Reactor.Networking;
using Reactor.Networking.MethodRpc;
using UnityEngine;

namespace Peasmod.Roles
{
    [RegisterCustomRole]
    public class Ninja : BaseRole
    {
        public Ninja(BasePlugin plugin) : base(plugin)
        {
        }

        public override string Name => "Ninja";
        public override string Description => "Kill players without being seen";
        public override string TaskText => "Kill players without being seen by using your ability";
        public override Color Color => Palette.ImpostorRed;
        public override Visibility Visibility => Visibility.Impostor;
        public override Team Team => Team.Impostor;
        public override bool HasToDoTasks => false;
        public override int Limit => (int)Settings.NinjaAmount.Value;
        public override bool CanVent => true;
        public override bool CanKill(PlayerControl victim = null) => !victim || victim.Data.Role.IsImpostor;
        public override bool CanSabotage(SystemTypes? sabotage) => true;

        public CustomButton Button;
        private static List<byte> _invisiblePlayers = new List<byte>();

        public override void OnGameStart()
        {
            _invisiblePlayers.Clear();
            Button = CustomButton.AddRoleButton(
                () => { RpcGoInvisible(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, true); },
                Settings.InvisibilityCooldown.Value, PeasAPI.Utility.CreateSprite("Peasmod.Resources.Buttons.Hide.png", 794f),
                Vector2.zero, false, this, Settings.InvisibilityDuration.Value,
                () => { RpcGoInvisible(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, false); }, "<size=40%>Hide");
        }

        public override void OnGameStop()
        {
            /*for (int i = 0; i < _invisiblePlayers.Count; i++)
            {
                RpcGoInvisible(PlayerControl.LocalPlayer, _invisiblePlayers[i].GetPlayer(), false);
            }*/
        }

        [MethodRpc((uint)CustomRpcCalls.GoInvisible, LocalHandling = RpcLocalHandling.Before)]
        public static void RpcGoInvisible(PlayerControl player, PlayerControl target, bool enable)
        {
            if (target.IsLocal())
            {
                _invisiblePlayers.Add(target.PlayerId);
                target.myRend.color =
                    target.myRend.color.SetAlpha(enable ? 0.5f : 1f);
                target.SetHatAndVisorAlpha(enable ? 0.5f : 1f);
                target.MyPhysics.Skin.layer.color =
                    target.MyPhysics.Skin.layer.color.SetAlpha(enable ? 0.5f : 1f);
            }
            else
            {
                _invisiblePlayers.Remove(target.PlayerId);
                target.Visible = !enable;
            }
        }

        [HarmonyPatch]
        public static class Patches
        {
            [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
            [HarmonyPrefix]
            private static void PlayerControlRevivePatch(PlayerControl __instance)
            {
                __instance.SetHatAndVisorAlpha(1f);
            }
            
            [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ResetAnimState))]
            [HarmonyPrefix]
            private static bool PlayerPhysicsResetAnimStatePatch(PlayerPhysics __instance)
            {
                if (__instance.myPlayer)
                {
                    __instance.myPlayer.FootSteps.Stop();
                    __instance.myPlayer.FootSteps.loop = false;
                    __instance.myPlayer.HatRenderer.SetIdleAnim();
                    __instance.myPlayer.VisorSlot.gameObject.SetActive(true);
                }

                GameData.PlayerInfo data = __instance.myPlayer.Data;
                if (data != null)
                {
                    __instance.myPlayer.HatRenderer.SetColor(__instance.myPlayer.CurrentOutfit.ColorId);
                }

                if (data == null || !data.IsDead)
                {
                    __instance.Skin.SetIdle(__instance.rend.flipX);
                    __instance.Animator.Play(__instance.IdleAnim, 1f);
                    __instance.myPlayer.Visible = true;
                    //__instance.myPlayer.SetHatAndVisorAlpha(1f); - Disabled so the invisibility effect works local
                    return false;
                }

                __instance.Skin.SetGhost();
                if (data != null && data.Role != null)
                {
                    __instance.Animator.Play(
                        (data.Role.Role == RoleTypes.GuardianAngel)
                            ? __instance.GhostGuardianAngelAnim
                            : __instance.GhostIdleAnim, 1f);
                }

                PlayerControl playerControl = __instance.myPlayer;
                if (playerControl == null)
                {
                    return false;
                }

                playerControl.SetHatAndVisorAlpha(0.5f);
                return false;
            }

            [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
            [HarmonyPrefix]
            private static bool PlayerPhysicsHandleAnimationPatch(PlayerPhysics __instance,
                [HarmonyArgument(0)] bool amDead)
            {
                if (__instance.Animator.IsPlaying(__instance.SpawnAnim))
                    return false;

                if (!GameData.Instance)
                    return false;

                Vector2 velocity = __instance.body.velocity;
                AnimationClip currentAnimation = __instance.Animator.GetCurrentAnimation();
                if (currentAnimation == __instance.ClimbAnim || currentAnimation == __instance.ClimbDownAnim)
                    return false;

                if (!amDead)
                {
                    if (velocity.sqrMagnitude >= 0.05f)
                    {
                        bool flipX = __instance.rend.flipX;
                        if (velocity.x < -0.01f)
                        {
                            __instance.rend.flipX = true;
                        }
                        else if (velocity.x > 0.01f)
                        {
                            __instance.rend.flipX = false;
                        }

                        if (currentAnimation != __instance.RunAnim || flipX != __instance.rend.flipX)
                        {
                            __instance.Animator.Play(__instance.RunAnim, 1f);
                            __instance.Animator.Time = 0.45833334f;
                            __instance.Skin.SetRun(__instance.rend.flipX);
                        }
                    }
                    else if (currentAnimation == __instance.RunAnim || currentAnimation == __instance.SpawnAnim ||
                             !currentAnimation)
                    {
                        __instance.Skin.SetIdle(__instance.rend.flipX);
                        __instance.Animator.Play(__instance.IdleAnim, 1f);
                        //__instance.myPlayer.SetHatAndVisorAlpha(1f); - Disabled so the invisibility effect works local
                    }
                }
                else
                {
                    __instance.Skin.SetGhost();
                    if (__instance.myPlayer.Data.Role.Role == RoleTypes.GuardianAngel)
                    {
                        if (currentAnimation != __instance.GhostGuardianAngelAnim)
                        {
                            __instance.Animator.Play(__instance.GhostGuardianAngelAnim, 1f);
                            __instance.myPlayer.SetHatAndVisorAlpha(0.5f);
                        }
                    }
                    else if (currentAnimation != __instance.GhostIdleAnim)
                    {
                        __instance.Animator.Play(__instance.GhostIdleAnim, 1f);
                        __instance.myPlayer.SetHatAndVisorAlpha(0.5f);
                    }

                    if (velocity.x < -0.01f)
                    {
                        __instance.rend.flipX = true;
                    }
                    else if (velocity.x > 0.01f)
                    {
                        __instance.rend.flipX = false;
                    }
                }

                __instance.Skin.Flipped = __instance.rend.flipX;
                __instance.myPlayer.VisorSlot.SetFlipX(__instance.rend.flipX);
                return false;
            }
        }
    }
}