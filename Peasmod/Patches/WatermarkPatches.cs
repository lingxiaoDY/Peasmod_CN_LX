using System;
using System.Diagnostics;
using System.Text;
using HarmonyLib;
using PeasAPI;
using Reactor.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Peasmod.Patches
{
    [HarmonyPatch]
    public static class WatermarkPatches
    {
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        [HarmonyPostfix]
        public static void PeasmodLogoPatch()
        {
            var peasmodLogo = Utility.CreateSprite("Peasmod.Resources.Peasmod.png");
                
            GameObject.Find("bannerLogo_AmongUs").GetComponent<SpriteRenderer>().sprite = peasmodLogo;
            GameObject.Find("AmongUsLogo").GetComponent<SpriteRenderer>().sprite = peasmodLogo;
            GameObject.Find("AmongUsLogo").transform.position += new Vector3(0.3f, 0, 0);

            if (GameObject.Find("CreditsButton") != null)
            {
                var creditButton = Object.Instantiate(GameObject.Find("CreditsButton"), GameObject.Find("CreditsButton").transform.parent);
                creditButton.GetComponent<SpriteRenderer>().sprite = Utility.CreateSprite("Peasmod.Resources.Buttons.Credits.png", 95f);
                var button = creditButton.GetComponent<PassiveButton>();
                button.OnClick = new Button.ButtonClickedEvent();
                button.OnClick.AddListener((UnityAction) openPopUp);

                void openPopUp()
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine($"{Utility.StringColor.Green}Peasmod{Utility.StringColor.Reset} 制作： @Grüni");
                    stringBuilder.AppendLine("开发人员： @Peasplayer#2541");
                    stringBuilder.AppendLine("艺术设计：");
                    stringBuilder.AppendLine("@C.A 100 PRO PLAYER#4147 & @Sam.#0006");
                    stringBuilder.AppendLine($"{Utility.StringColor.Red}PeasAPI{Utility.StringColor.Reset}");
                    stringBuilder.AppendLine("开发人员：");
                    stringBuilder.AppendLine("@Peasplayer#2541 @Plix#7013");
                    stringBuilder.AppendLine("@Pandapip1#8943");
                    stringBuilder.AppendLine("中文汉化：");
                    stringBuilder.AppendLine($"{Utility.StringColor.Red}四个憨批汉化组{Utility.StringColor.Reset}");
                    stringBuilder.AppendLine($"翻译：{Utility.StringColor.Orange}凌霄LX{Utility.StringColor.Reset}");
                    var popup = GeneratePopUp();
                    popup.Show(stringBuilder.ToString());
                    popup.TextAreaTMP.transform.SetY(0.25f);
                }
            }
        }

        private static GenericPopup _popup;
        
        private static GenericPopup GeneratePopUp()
        {
            if (_popup == null)
                _popup = Object.Instantiate(DiscordManager.Instance.discordPopup, DiscordManager.Instance.discordPopup.transform.parent);
            _popup.transform.localScale = Vector3.one * 2f;
            
            var transform = CreateButton("discord", "Discord", new Vector3(-1.7f, -0.75f));
            var transform2 = CreateButton("github", "GitHub", new Vector3(0f, -0.75f));
            CreateButton("close", "Close", new Vector3(1.7f, -0.75f));
            
            var component = transform.GetComponent<PassiveButton>();
            component.OnClick.RemoveAllListeners();
            component.OnClick.AddListener((Action) delegate
            {
                Process.Start("https://discord.gg/nQB5EZe");
            });
            
            component = transform2.GetComponent<PassiveButton>();
            component.OnClick.RemoveAllListeners();
            component.OnClick.AddListener((Action) delegate
            {
                Process.Start("https://github.com/Peasplayer/Peasmod");
            });
            
            return _popup;
        }

        private static Transform CreateButton(string name, string text, Vector3 offset)
        {
            var find = _popup.transform.FindChild(name);
            if (find) return find;
            
            var template = _popup.transform.FindChild("ExitGame");
            template.GetComponentInChildren<TextTranslatorTMP>().Destroy();
            template.gameObject.SetActive(false);
            
            var button = Object.Instantiate(template, _popup.transform).DontDestroy();
            var transform = button.transform;
            
            button.gameObject.name = name;
            transform.position += offset;
            transform.localScale /= 2f;
            
            button.GetComponentInChildren<TextMeshPro>().text = text;
            button.gameObject.SetActive(true);
            return button;
        }
    }
}