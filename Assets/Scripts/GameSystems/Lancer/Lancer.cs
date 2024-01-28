using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
namespace Lancer
{

    public class LancerSystem : GameSystem
    {
        public override void AddTokenModal()
        {
            base.AddTokenModal();
            Modal.AddTextField("FavoriteFood", "Favorite Food", "");
        }

        public override void CreateToken()
        {
            string name = UI.Modal.Q<TextField>("NameField").value;
            Texture2D graphic = TextureSender.CopyLocalImage(UI.Modal.Q("ImageSearchField").Q<TextField>("SearchInput").value);
            string graphicHash = TextureSender.GetTextureHash(graphic);
            // int size = int.Parse(UI.Modal.Q<DropdownField>("SizeField").value.Substring(0, 1));
            // int hp = UI.Modal.Q<IntegerField>("HPField").value;
            // string extraInfo = UI.Modal.Q<TextField>("ExtraInfo").value;

            LancerData data = LancerData.Everest();

            Player.Self().CmdCreateToken("Lancer", graphicHash, name, 1, Color.black, JsonUtility.ToJson(data));
        }

        public override void GameDataSetValue(string value)
        {
            FileLogger.Write("$Game system changed - {value}");
            if (value == "IncrementTurn")
            {
                RoundNumber++;
                foreach (var g in GameObject.FindGameObjectsWithTag("TokenData"))
                {
                    TokenData data = g.GetComponent<TokenData>();
                    TokenDataSetValue(data.Id, "StartTurn");
                }
            }
        }

        public override string[] GetEffectList()
        {
            return new string[] { "Danger Zone", "Down and Out", "Engaged", "Exposed", "Hidden", "Invisible", "Prone", "Shut Down", "Immobilized", "Impaired", "Jammed", "Lock-On", "Shredded", "Slowed", "Stunned" };
        }

        public override string GetOverheadAsset()
        {
            return "UITemplates/GameSystem/SimpleOverhead";
        }

        public override string GetSystemVars()
        {
            return base.GetSystemVars();
        }

        public override MenuItem[] GetTileMenuItems()
        {
            return base.GetTileMenuItems();
        }

        public override MenuItem[] GetTokenMenuItems(TokenData data)
        {
            // Common actions would beee...  ???
            return base.GetTokenMenuItems(data);
        }

        public override void SetSystemVars(string vars)
        {
            base.SetSystemVars(vars);
        }

        public override void Setup()
        {
            base.Setup();
            SetupPanel("SelectedTokenPanel", true);
            SetupPanel("FocusedTokenPanel", false);
        }

        public override string SystemName()
        {
            return "Lancer";
        }

        public override void Teardown()
        {
            TeardownPanel("SelectedTokenPanel");
            TeardownPanel("FocusedTokenPanel");
        }

        public override void TokenDataSetValue(string tokenId, string value)
        {
            base.TokenDataSetValue(tokenId, value);
        }

        public override string TurnAdvanceMessage()
        {
            return "Increase the round counter? Turns will be reset!";
        }

        public override void UpdateData(TokenData data)
        {
            base.UpdateData(data);

            LancerData ldata = JsonUtility.FromJson<LancerData>(data.SystemData);

            data.OverheadElement.Q<ProgressBar>("HpBar").value = ldata.CurrentHP;
            data.OverheadElement.Q<ProgressBar>("HpBar").highValue = ldata.MaxHP;
            
        }

        public override void UpdateTokenPanel(string tokenId, string elementName)
        {
            TokenData data = TokenData.Find(tokenId);
            UI.ToggleActiveClass(elementName, data != null);
            if (!data)
                return;
            data.UpdateTokenPanel(elementName);
            VisualElement panel = UI.System.Q(elementName);
            LancerData ldata = JsonUtility.FromJson<LancerData>(data.SystemData);
            UpdateTokenPanel(ldata, panel);
        }

        #region UI Stuffs
        void SetupPanel(string elementName, bool editable)
        {
            VisualElement panel = UI.System.Q(elementName);
            VisualElement unitPanel = UI.CreateFromTemplate("UITemplates/GameSystem/LancerUnitPanel");

            unitPanel.Q<ProgressBar>("HpBar").style.minWidth = 150;
            unitPanel.Q("HP").Q<Label>("StatLabel").text = "HP";
            unitPanel.Q("Heat").Q<Label>("StatLabel").text = "Heat";
            panel.Q("Data").Add(unitPanel);
        }

        private void TeardownPanel(string elementName)
        {
            VisualElement panel = UI.System.Q(elementName);
            panel.Q("ExtraInfo").Clear();
            panel.Q("Data").Clear();
        }
        #region UI Helpers
        static private void UpdateTokenPanel(LancerData data, VisualElement unitPanel)
        {
            UpdateHealthBar(unitPanel.Q("HP"), data.CurrentHP, data.MaxHP);
            UpdateHealthBar(unitPanel.Q("Heat"), data.CurrentHeat, data.MaxHeat);
        }

        static void UpdateHealthBar(VisualElement group, int current, int max)
        {
            group.Q<ProgressBar>("HpBar").value = current;
            group.Q<ProgressBar>("HpBar").highValue = max;
            group.Q<Label>("CHP").text = $"{current}";
            group.Q<Label>("MHP").text = $"/{max}";
        }
        #endregion
        #endregion
    }
}
