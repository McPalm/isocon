using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;
using System.Data.Common;
using IsoconUILibrary;
using SimpleJSON;
using System.Reflection;

public class Icon_v1_5 : GameSystem {

    public int PartyResolve = 0;
    
    public override string SystemName()
    {
        return "ICON 1.5";
    }

    public override void InterpreterMethod(string name, object[] args)
    {
        Type classType = Type.GetType("Icon1_5");
        MethodInfo method = classType.GetMethod(name, BindingFlags.Public | BindingFlags.Static);
        method.Invoke(null, args);
    }

    public override void Setup()
    {
        SetupPanel("SelectedTokenPanel", true);
        SetupPanel("SelectedTokenPanel", true);
    }

    private void SetupPanel(string elementName, bool editable) {
        VisualElement panel = UI.System.Q(elementName);
        VisualElement unitPanel = UI.CreateFromTemplate("UITemplates/GameSystem/IconUnitPanel");

        unitPanel.Q("Damage").Q<Label>("Label").text = "DMG/FRAY";
        unitPanel.Q("Range").Q<Label>("Label").text = "RNG";
        unitPanel.Q("Speed").Q<Label>("Label").text = "SPD/DASH";
        unitPanel.Q("Defense").Q<Label>("Label").text = "DEF";
        // unitPanel.Q<Button>("AlterVitals").RegisterCallback<ClickEvent>(AlterVitalsModal);
        // unitPanel.Q<Button>("AddStatus").RegisterCallback<ClickEvent>(AddStatusModal);
        panel.Q("Data").Add(unitPanel);
        panel.Q("ExtraInfo").Add(new Label(){ name = "Class" });
        panel.Q("ExtraInfo").Add(new Label(){ name = "Job" });
        panel.Q("ExtraInfo").Add(new Label(){ name = "Elite", text = "Elite" });
    }

    public override void GameDataSetValue(string value) {
        FileLogger.Write($"Game system changed - {value}");
        if (value == "IncrementTurn") {
            TurnNumber++;
            UI.System.Q<Label>("TurnNumber").text = TurnNumber.ToString();
            foreach(GameObject g in GameObject.FindGameObjectsWithTag("TokenData")) {
                TokenData2 data = g.GetComponent<TokenData2>();
                TokenDataSetValue(data.Id, "LoseStatus|Turn Ended");
            }
        }
    }

    public override string[] GetEffectList() {
        return new string[]{"Difficult", "Pit", "Dangerous", "Impassable", "Interactive", "Demon Slayer/Flash Step - Afterimage", "Demon Slayer/Six Hells Trigram", "Demon Slayer/Heroic Six Hells Trigram", "Fool/Party Favor", "Freelancer/Showdown - Quench", "Freelancer/Warding Bolts", "Shade/Shadow Cloud (Blinded+ exc Caster)", "Harvester/Plant", "Harvester/Blood Grove", "Harvester/Mote of Life (Blessing))", "Harvester/Mote of Life (Regen)", "Spellblade/Lightning Spike 1", "Spellblade/Lightning Spike 2", "Spellblade/Lightning Spike 3", "Spellblade/Lightning Spike 4", "Spellblade/Lightning Spike 5", "Spellblade/Lightning Spike 6", "Stormbender/Selkie", "Stormbender/Salt Sprite", "Stormbender/Pit", "Stormbender/Tsunami", "Stormbender/Tsunami - Stormlash", "Stormbender/Dangerous", "Stormbender/Geyser I", "Stormbender/Gust", "Stormbender/Gust I", "Stormbender/Gust II", "Stormbender/Waterspout", "Stormbender/Waterspout - Hurricane", "Stormbender/Waterspout I", "Stormbender/Waterspout I - Hurricane", "Stormbender/Waterspout II", "Stormbender/Waterspout II - Hurricane"};
    }    

    public override void AddTokenModal()
    {
        JSONNode gamedata = JSON.Parse(GameSystem.DataJson);
        List<string> playerJobs = new();
        foreach (JSONNode pjob in gamedata["Icon1_5"]["PlayerJobs"].AsArray) {
            playerJobs.Add(pjob);
        }
        List<string> foeClasses = new();
        foreach (JSONNode fclass in gamedata["Icon1_5"]["FoeClasses"].AsArray) {
            foeClasses.Add(fclass);
        }

        base.AddTokenModal();
        Modal.AddDropdownField("Type", "Type", "Player", new string[]{"Player", "Foe", "Object"}, (evt) => AddTokenModalEvaluateConditions());
        Modal.AddSearchField("PlayerJob", "Job", "Stalwart/Bastion", playerJobs.ToArray());
        Modal.AddDropdownField("FoeClass", "Class", foeClasses[0], foeClasses.ToArray(), (evt) => AddTokenModalEvaluateConditions());
        Modal.AddTextField("FoeJob", "Job", "");
        Modal.AddToggleField("Elite", "Elite", false);
        Modal.AddDropdownField("LegendHP", "Legend HP Multiplier", "x4", new string[]{"x2", "x3", "x4", "x5", "x6", "x7", "x8"});
        Modal.AddDropdownField("Size", "Size", "1x1", new string[]{"1x1", "2x2", "3x3"});
        Modal.AddIntField("ObjectHP", "Object HP", 1);
        Modal.AddIntField("CloneCount", "Clone Count", 1);

        AddTokenModalEvaluateConditions();
    }

    private static void AddTokenModalEvaluateConditions() {
        bool playerJob = UI.Modal.Q<DropdownField>("Type").value == "Player";
        bool foeClass = UI.Modal.Q<DropdownField>("Type").value == "Foe";
        bool foeJob = UI.Modal.Q<DropdownField>("Type").value == "Foe";
        bool elite = foeClass && !StringUtility.InList(UI.Modal.Q<DropdownField>("FoeClass").value, "Legend", "Mob");
        bool legendHP = foeClass && UI.Modal.Q<DropdownField>("FoeClass").value == "Legend";
        bool size = foeClass;
        bool objectHP = UI.Modal.Q<DropdownField>("Type").value == "Object";
        bool cloneCount = UI.Modal.Q<DropdownField>("Type").value == "Object" || UI.Modal.Q<DropdownField>("FoeClass").value == "Mob";

        UI.ToggleDisplay(UI.Modal.Q("PlayerJob"), playerJob);
        UI.ToggleDisplay(UI.Modal.Q("FoeClass"), foeClass);
        UI.ToggleDisplay(UI.Modal.Q("FoeJob"), foeJob);
        UI.ToggleDisplay(UI.Modal.Q("Elite"), elite);
        UI.ToggleDisplay(UI.Modal.Q("LegendHP"), legendHP);
        UI.ToggleDisplay(UI.Modal.Q("Size"), size);
        UI.ToggleDisplay(UI.Modal.Q("ObjectHP"), objectHP);
        UI.ToggleDisplay(UI.Modal.Q("CloneCount"), cloneCount);
    }
}

[Serializable]
public class Icon1_5Data {
    public int CurrentHP;
    public int MaxHP;
    public int Vigor;
    public int Resolve;
    public string Job;
    public string Class;
    public bool Elite;
    public int HPMultiplier;
    public int Wounds;
    public int Damage;
    public int Fray;
    public int Range;
    public int Speed;
    public int Dash;
    public int Defense;
    public string[] Conditions;
    
    public void Change(string value, Token token, bool placed) {
        if (value.StartsWith("GainWound")) {
            Wounds++;
            Wounds = Math.Min(Wounds, 3);
            int woundMaxHP = MaxHP / 4 * (4 - Wounds);
            CurrentHP = Math.Min(CurrentHP, woundMaxHP);
            OnVitalChange(token);
        }
        if (value.StartsWith("LoseWound")) {
            Wounds--;
            Wounds = Math.Max(Wounds, 0);
            OnVitalChange(token);
        }
        if (value.StartsWith("GainHP")) {
            int diff = int.Parse(value.Split("|")[1]);
            int woundMaxHP = MaxHP / 4 * (4 - Wounds);
            if (CurrentHP + diff > woundMaxHP) {
                diff = woundMaxHP - CurrentHP;
            }
            if (diff > 0) {
                CurrentHP+=diff;
                PopoverText.Create(token, $"/+{diff}|_HP", Color.white);
                token.SetDefeated(CurrentHP <= 0);
            }
            OnVitalChange(token);
        }
        if (value.StartsWith("LoseHP")) {
            int diff = int.Parse(value.Split("|")[1]);
            if (CurrentHP - diff < 0) {
                diff = CurrentHP;
            }
            if (diff > 0) {
                CurrentHP-=diff;
                PopoverText.Create(token, $"/-{diff}|_HP", Color.white);
                token.SetDefeated(CurrentHP <= 0);
            }
            OnVitalChange(token);
        }
        if (value.StartsWith("GainVIG")) {
            int diff = int.Parse(value.Split("|")[1]);
            if (Vigor + diff > MaxHP/4) {
                diff = MaxHP/4 - Vigor;
            }
            if (diff > 0) {
                Vigor+=diff;
                PopoverText.Create(token, $"/+{diff}|_VIG", Color.white);
            }
            OnVitalChange(token);
        }
        if (value.StartsWith("LoseVIG")) {
            int diff = int.Parse(value.Split("|")[1]);
            if (Vigor - diff < 0) {
                diff = Vigor;
            }
            if (diff > 0) {
                Vigor-=diff;
                PopoverText.Create(token, $"/-{diff}|_VIG", Color.white);
            }
            OnVitalChange(token);
        }
        if (value.StartsWith("GainRES")) {
            int diff = int.Parse(value.Split("|")[1]);
            if (diff + Resolve > 6) {
                diff = 6 - Resolve;
            }
            if (diff > 0) {
                Resolve+=diff;
                PopoverText.Create(token, $"/+{diff}|_RES", Color.white);
            }
            OnVitalChange(token);
        }
        // if (value.StartsWith("GainPRES")) {
        //     int diff = int.Parse(value.Split("|")[1]);
        //     Icon_v1_5.PartyResolve+=diff;
        //     OnVitalChange(token);
        // }
        if (value.StartsWith("LoseRES")) {
            int diff = int.Parse(value.Split("|")[1]);
            Resolve = Math.Max(0, Resolve - diff);
            OnVitalChange(token);
        }
        // if (value.StartsWith("LosePRES")) {
        //     int diff = int.Parse(value.Split("|")[1]);
        //     if (diff > Icon_v1_5.PartyResolve) {
        //         diff = Icon_v1_5.PartyResolve;
        //     }
        //     Icon_v1_5.PartyResolve-=diff;
        //     OnVitalChange(token);
        // }
        if (value.StartsWith("Damage")) {
            int diff = int.Parse(value.Split("|")[1]);
            if (Vigor + CurrentHP - diff < 0) {
                diff = Vigor+CurrentHP;
            }
            if (diff <= 0) {
                return;
            }
            if (diff < Vigor) {
                // Vig damage only
                Vigor -= diff;
                PopoverText.Create(token, $"/-{diff}|_VIG", Color.white);
            }
            else if (diff > Vigor && Vigor > 0) {
                // Vig zeroed and HP damage
                CurrentHP -= (diff - Vigor);
                Vigor = 0;
                PopoverText.Create(token, $"/-{diff}|_HP/VIG", Color.white);
            }
            else if (Vigor <= 0) {
                // HP damage only
                CurrentHP -= diff;
                PopoverText.Create(token, $"/-{diff}|_HP", Color.white);
                token.SetDefeated(CurrentHP <= 0);
            }
            OnVitalChange(token);
        }
        // if (value.StartsWith("LoseStatus")) {
        //     string[] parts = value.Split("|");
        //     Conditions.Remove(parts[1]);
        //     PopoverText.Create(token, $"/-|_{parts[1].ToUpper()}", Color.white);
        //     OnStatusChange();
        // }
        // if (value.StartsWith("GainStatus")) {
        //     string[] parts = value.Split("|");
        //     if (!Conditions.ContainsKey(parts[1])) {
        //         Conditions.Add(parts[1], new StatusEffect(){Name = parts[1], Type = parts[2], Color = parts[3], Number = int.Parse(parts[4])});
        //     }
        //     else {
        //         Toast.Add($"Condition { parts[1] } is already set on { Name }.");
        //     }
        //     PopoverText.Create(token, $"/+|_{parts[1].ToUpper()}", Color.white);
        //     OnStatusChange();
        // }
        // if (value.StartsWith("IncrementStatus")) {
        //     string status = value.Split("|")[1];
        //     StatusEffect se = Conditions[status];
        //     se.Number++;
        //     Conditions[status] = se;
        //     OnStatusChange();
        // }
        // if (value.StartsWith("DecrementStatus")) {
        //     string status = value.Split("|")[1];
        //     StatusEffect se = Conditions[status];
        //     se.Number--;
        //     Conditions[status] = se;
        //     OnStatusChange();
        // }
    }

    private void OnVitalChange(Token token) {
        List<string> conditions = Conditions.ToList();

        token.SetDefeated(CurrentHP <= 0);
        if (CurrentHP <= 0) {
            Conditions = CollectionUtility.AddToArray(Conditions, "Defeated", true);
        }
        else if (CurrentHP <= MaxHP/2) {
            Conditions = CollectionUtility.AddToArray(Conditions, "Bloodied", true);
        }
        else {
            Conditions = CollectionUtility.RemoveAllFromArray(Conditions, "Defeated");
            Conditions = CollectionUtility.RemoveAllFromArray(Conditions, "Bloodied");
        }
    }    
}

public class Icon_v1_5Interpreter {
    public static void CreateToken() {
        string name = UI.Modal.Q<TextField>("NameField").value;
        Texture2D graphic = TextureSender.CopyLocalImage(UI.Modal.Q("ImageSearchField").Q<TextField>("SearchInput").value);
        string graphicHash = TextureSender.GetTextureHash(graphic);

        // string houseJob = SearchField.GetValue(UI.Modal.Q("UnitType"));
        // string house = houseJob.Split("/")[0];
        // string job = houseJob.Split("/")[1];
        // string colorValue = UI.Modal.Q<DropdownField>("PlayerColor").value;

        Icon1_5Data data = new(){

        };
        InitSystemData(data);
        
        int size = 1;

        Color color = Color.black;

        Player.Self().CmdCreateToken("Maleghast", graphicHash, name, size, color, JsonUtility.ToJson(data));
    }

    public static void UpdateData(TokenData2 data) {
        Icon1_5Data mdata = JsonUtility.FromJson<Icon1_5Data>(data.SystemData);

        data.OverheadElement.Q<ProgressBar>("VigorBar").value = mdata.Vigor;
        data.OverheadElement.Q<ProgressBar>("VigorBar").highValue = mdata.MaxHP;
        UI.ToggleDisplay(data.OverheadElement.Q("VigorBar"), mdata.Vigor > 0);

        data.OverheadElement.Q<ProgressBar>("HpBar").value = mdata.CurrentHP;
        data.OverheadElement.Q<ProgressBar>("HpBar").highValue = mdata.MaxHP;

        UI.ToggleDisplay(data.OverheadElement.Q("Wound1"), mdata.Wounds >= 1);
        UI.ToggleDisplay(data.OverheadElement.Q("Wound2"), mdata.Wounds >= 2);
        UI.ToggleDisplay(data.OverheadElement.Q("Wound3"), mdata.Wounds >= 3);
        
        UI.ToggleDisplay(data.OverheadElement.Q("HpBar"), mdata.CurrentHP > 0);    
    }

    private static void InitSystemData(Icon1_5Data data) {
    }

    public static void Change(string tokenId, string value) {
        TokenData2 data = TokenData2.Find(tokenId);
        Debug.Log($"Icon 1.5 Interpreter change registered for {data.Name}: {value}");
        Icon1_5Data sysdata = JsonUtility.FromJson<Icon1_5Data>(data.SystemData);
        sysdata.Change(value, data.WorldObject.GetComponent<Token>(), data.Placed);
        data.SystemData = JsonUtility.ToJson(sysdata);  
    }

    public static void UpdateTokenPanel(string tokenId, string elementName) {
        TokenData2 data = TokenData2.Find(tokenId);
        UI.ToggleActiveClass(elementName, data != null);
        if (!data) {
            return;
        }

        data.UpdateTokenPanel(elementName);
        MaleghastData sysdata = JsonUtility.FromJson<MaleghastData>(data.SystemData);

        VisualElement panel = UI.System.Q(elementName);

        panel.Q("ClassBackground").style.borderTopColor = data.Color;
        panel.Q("ClassBackground").style.borderRightColor = data.Color;
        panel.Q("ClassBackground").style.borderBottomColor = data.Color;
        panel.Q("ClassBackground").style.borderLeftColor = data.Color;

        panel.Q<Label>("House").text = sysdata.House;
        panel.Q<Label>("House").style.backgroundColor = data.Color;
        panel.Q<Label>("Job").text = sysdata.Job;
        panel.Q<Label>("Job").style.backgroundColor = data.Color;

        panel.Q("HP").Q<Label>("CHP").text = $"{ sysdata.CurrentHP }";
        panel.Q("HP").Q<Label>("MHP").text = $"/{ sysdata.MaxHP }";
        panel.Q("HP").Q<ProgressBar>("HpBar").value = sysdata.CurrentHP;
        panel.Q("HP").Q<ProgressBar>("HpBar").highValue = sysdata.MaxHP;

        panel.Q("SOUL").Q<Label>("CHP").text = $"{ sysdata.Soul }";
        panel.Q("SOUL").Q<Label>("MHP").text = $"/6";
        panel.Q("SOUL").Q<ProgressBar>("HpBar").value = sysdata.Soul;
        panel.Q("SOUL").Q<ProgressBar>("HpBar").highValue = 6;

        panel.Q("Defense").Q<Label>("Value").text = $"{ sysdata.Defense }";
        panel.Q("Move").Q<Label>("Value").text = $"{ sysdata.Move }";
        panel.Q("Type").Q<Label>("Value").text = (sysdata.Type == "Necromancer") ? "NECRO" : sysdata.Type.ToUpper();

        panel.Q("Traits").Clear();
        foreach (string s in sysdata.Traits) {
            if (!s.StartsWith("-")) {
                string s2 = s;
                if (s2.StartsWith("=")) {
                    s2 = s2.Substring(1);
                }
                panel.Q("Traits").Add(new Label(){text = s2});
            }
        }

        panel.Q("ACTAbilities").Clear();
        foreach (string s in sysdata.ActAbilities) {
            if (!s.StartsWith("-")) {
                string s2 = s;
                if (s2.StartsWith("=")) {
                    s2 = s2.Substring(1);
                }
                panel.Q("ACTAbilities").Add(new Label(){text = s2});
            }
        }

        panel.Q("SOULAbilities").Clear();
        foreach (string s in sysdata.SoulAbilities) {
            if (!s.StartsWith("-")) {
                string s2 = s;
                if (s2.StartsWith("=")) {
                    s2 = s2.Substring(1);
                }
                panel.Q("SOULAbilities").Add(new Label(){text = s2});
            }
        }

        panel.Q("Upgrades").Clear();
        foreach (string s in sysdata.Upgrades) {
            if (!s.StartsWith("-")) {
                string s2 = s;
                if (s2.StartsWith("=")) {
                    s2 = s2.Substring(1);
                }
                panel.Q("Upgrades").Add(new Label(){text = s2});
            }
        }

        panel.Q("Conditions").Clear();
        foreach (string s in sysdata.Conditions) {
            panel.Q("Conditions").Add(new Label(){text = s}); 
        }
        
        panel.Q("Tokens").Clear();
        Dictionary<string, int> combinedTokens = new();
        foreach (string s in sysdata.Tokens) {
            if (combinedTokens.Keys.Contains(s)) {
                combinedTokens[s]++;
            }
            else {
                combinedTokens.Add(s, 1);
            }
        }
        foreach (KeyValuePair<string,int> pair in combinedTokens){
            if (pair.Value != 0) {
                panel.Q("Tokens").Add(new Label(){text = $"{pair.Key} ({pair.Value})"});
            }
        }

        UI.ToggleDisplay(panel.Q("SOUL"), sysdata.Type == "Necromancer");
        UI.ToggleDisplay(panel.Q("SOULAbilities"), sysdata.Type == "Necromancer");
        UI.ToggleDisplay(panel.Q("SOULAbilitiesLabel"), sysdata.Type == "Necromancer");

        UI.ToggleDisplay(panel.Q("Upgrades"), sysdata.Type != "Necromancer");
        UI.ToggleDisplay(panel.Q("UpgradesLabel"), sysdata.Type != "Necromancer");

        UI.ToggleDisplay(panel.Q("Configuration"), sysdata.Type != "Object");
        UI.ToggleDisplay(panel.Q("Status"), sysdata.Type != "Object");
    }

}

// public class Icon_v1_5 : GameSystem
// {
//     public static int TurnNumber = 1;
//     public static int PartyResolve = 0;

//     public override string SystemName()
//     {
//         return "ICON 1.5";
//     }

//     public override void Setup()
//     {
//         base.Setup();

//         // Selected
//         VisualElement selectedPanel = UI.System.Q("SelectedTokenPanel");
//         VisualElement unitPanel = UI.CreateFromTemplate("UITemplates/GameSystem/IconUnitPanel");
//         unitPanel.Q("Damage").Q<Label>("Label").text = "DMG/FRAY";
//         unitPanel.Q("Range").Q<Label>("Label").text = "RNG";
//         unitPanel.Q("Speed").Q<Label>("Label").text = "SPD/DASH";
//         unitPanel.Q("Defense").Q<Label>("Label").text = "DEF";
//         unitPanel.Q<Button>("AlterVitals").RegisterCallback<ClickEvent>(AlterVitalsModal);
//         unitPanel.Q<Button>("AddStatus").RegisterCallback<ClickEvent>(AddStatusModal);
//         selectedPanel.Q("Data").Add(unitPanel);
//         selectedPanel.Q("ExtraInfo").Add(new Label(){ name = "Class" });
//         selectedPanel.Q("ExtraInfo").Add(new Label(){ name = "Job" });
//         selectedPanel.Q("ExtraInfo").Add(new Label(){ name = "Elite", text = "Elite" });

//         // Focused
//         VisualElement focusedPanel = UI.System.Q("FocusedTokenPanel");
//         unitPanel = UI.CreateFromTemplate("UITemplates/GameSystem/IconUnitPanel");
//         unitPanel.Q("Damage").Q<Label>("Label").text = "DMG/FRAY";
//         unitPanel.Q("Range").Q<Label>("Label").text = "RNG";
//         unitPanel.Q("Speed").Q<Label>("Label").text = "SPD/DASH";
//         unitPanel.Q("Defense").Q<Label>("Label").text = "DEF";
//         focusedPanel.Q("Data").Add(unitPanel);
//         focusedPanel.Q("ExtraInfo").Add(new Label(){ name = "Class" });
//         focusedPanel.Q("ExtraInfo").Add(new Label(){ name = "Job" });
//         focusedPanel.Q("ExtraInfo").Add(new Label(){ name = "Elite", text = "Elite" });
//     }

//     public override string GetTokenDataRawJson() {
//         return Icon_v1_5TokenDataRaw.ToJson();
//     }

//     public override void GameDataSetValue(string value) {
//         FileLogger.Write($"Game system changed - {value}");
//         if (value == "IncrementTurn") {
//             TurnNumber++;
//             PartyResolve++;
//             UI.System.Q<Label>("TurnNumber").text = TurnNumber.ToString();
//             foreach(GameObject g in GameObject.FindGameObjectsWithTag("TokenData")) {

//                 // Icon_v1_5TokenData data = g.GetComponent<Icon_v1_5TokenData>();
//                 // data.Change("LoseStatus|Turn Ended");

//             }
//         }
//         if (value.StartsWith("GainPRES")) {
//             int diff = int.Parse(value.Split("|")[1]);
//             PartyResolve+=diff;
//         }
//     }

//     public override Texture2D GetGraphic(string json) {
//         Icon_v1_5TokenDataRaw raw = JsonUtility.FromJson<Icon_v1_5TokenDataRaw>(json);
//         return TextureSender.LoadImageFromFile(raw.GraphicHash, true);
//     }

//     public override void UpdateTokenPanel(string tokenId, string elementName)
//     {
//         Icon_v1_5Interpreter.UpdateTokenPanel(tokenId, elementName);
//         // TokenData2 data = TokenData2.Find(tokenId);
//         // if (data == null) {
//         //     UI.ToggleDisplay(elementName, false);
//         //     return;
//         // }
//         // UI.ToggleDisplay(elementName, true);
//         // data.GetComponent<Icon_v1_5TokenData>().UpdateTokenPanel(elementName);
//     }

//     public override string[] GetEffectList() {
//         return new string[]{"Difficult", "Pit", "Dangerous", "Impassable", "Interactive", "Demon Slayer/Flash Step - Afterimage", "Demon Slayer/Six Hells Trigram", "Demon Slayer/Heroic Six Hells Trigram", "Fool/Party Favor", "Freelancer/Showdown - Quench", "Freelancer/Warding Bolts", "Shade/Shadow Cloud (Blinded+ exc Caster)", "Harvester/Plant", "Harvester/Blood Grove", "Harvester/Mote of Life (Blessing))", "Harvester/Mote of Life (Regen)", "Spellblade/Lightning Spike 1", "Spellblade/Lightning Spike 2", "Spellblade/Lightning Spike 3", "Spellblade/Lightning Spike 4", "Spellblade/Lightning Spike 5", "Spellblade/Lightning Spike 6", "Stormbender/Selkie", "Stormbender/Salt Sprite", "Stormbender/Pit", "Stormbender/Tsunami", "Stormbender/Tsunami - Stormlash", "Stormbender/Dangerous", "Stormbender/Geyser I", "Stormbender/Gust", "Stormbender/Gust I", "Stormbender/Gust II", "Stormbender/Waterspout", "Stormbender/Waterspout - Hurricane", "Stormbender/Waterspout I", "Stormbender/Waterspout I - Hurricane", "Stormbender/Waterspout II", "Stormbender/Waterspout II - Hurricane"};
//     }

//     public override void AddTokenModal()
//     {
//         JSONNode gamedata = JSON.Parse(GameSystem.DataJson);
//         List<string> playerJobs = new();
//         foreach (JSONNode pjob in gamedata["Icon1_5"]["PlayerJobs"].AsArray) {
//             playerJobs.Add(pjob);
//         }
//         List<string> foeClasses = new();
//         foreach (JSONNode fclass in gamedata["Icon1_5"]["FoeClasses"].AsArray) {
//             foeClasses.Add(fclass);
//         }

//         base.AddTokenModal();

//         Modal.AddDropdownField("Type", "Type", "Player", new string[]{"Player", "Foe", "Object"}, (evt) => AddTokenModalEvaluateConditions());

//         Modal.AddSearchField("PlayerJob", "Job", "Stalwart/Bastion", playerJobs.ToArray());

//         Modal.AddDropdownField("FoeClass", "Class", foeClasses[0], foeClasses.ToArray(), (evt) => AddTokenModalEvaluateConditions());

//         Modal.AddTextField("FoeJob", "Job", "");

//         Modal.AddToggleField("Elite", "Elite", false);

//         Modal.AddDropdownField("LegendHP", "Legend HP Multiplier", "x4", new string[]{"x2", "x3", "x4", "x5", "x6", "x7", "x8"});

//         Modal.AddDropdownField("Size", "Size", "1x1", new string[]{"1x1", "2x2", "3x3"});

//         Modal.AddIntField("ObjectHP", "Object HP", 1);

//         Modal.AddIntField("CloneCount", "Clone Count", 1);

//         AddTokenModalEvaluateConditions();
//     }

//     public override void CreateToken()
//     {
//         // string json = GetTokenDataRawJson();
//         // FileLogger.Write($"Token added: {json}");
//         // if (UI.Modal.Q<DropdownField>("Type").value == "Object" || UI.Modal.Q<DropdownField>("FoeClass").value == "Mob") {
//         //     int count = UI.Modal.Q<IntegerField>("CloneCount").value;
//         //     for(int i = 0; i < count; i++) {
//         //         Player.Self().CmdCreateTokenData(json);
//         //     }
//         // }
//         // else {
//         //     Player.Self().CmdCreateTokenData(json);
//         // }
//     }

//     private static void AddTokenModalEvaluateConditions() {
//         bool playerJob = UI.Modal.Q<DropdownField>("Type").value == "Player";
//         bool foeClass = UI.Modal.Q<DropdownField>("Type").value == "Foe";
//         bool foeJob = UI.Modal.Q<DropdownField>("Type").value == "Foe";
//         bool elite = foeClass && !StringUtility.InList(UI.Modal.Q<DropdownField>("FoeClass").value, "Legend", "Mob");
//         bool legendHP = foeClass && UI.Modal.Q<DropdownField>("FoeClass").value == "Legend";
//         bool size = foeClass;
//         bool objectHP = UI.Modal.Q<DropdownField>("Type").value == "Object";
//         bool cloneCount = UI.Modal.Q<DropdownField>("Type").value == "Object" || UI.Modal.Q<DropdownField>("FoeClass").value == "Mob";

//         UI.ToggleDisplay(UI.Modal.Q("PlayerJob"), playerJob);
//         UI.ToggleDisplay(UI.Modal.Q("FoeClass"), foeClass);
//         UI.ToggleDisplay(UI.Modal.Q("FoeJob"), foeJob);
//         UI.ToggleDisplay(UI.Modal.Q("Elite"), elite);
//         UI.ToggleDisplay(UI.Modal.Q("LegendHP"), legendHP);
//         UI.ToggleDisplay(UI.Modal.Q("Size"), size);
//         UI.ToggleDisplay(UI.Modal.Q("ObjectHP"), objectHP);
//         UI.ToggleDisplay(UI.Modal.Q("CloneCount"), cloneCount);
//     }

//     private static void AlterVitalsModal(ClickEvent evt) {
//         Modal.Reset("Alter Vitals");
//         Modal.AddIntField("Number", "Value", 0);
//         Modal.AddContentButton("Damage HP/VIG", (evt) => AlterVitals("Damage"));
//         Modal.AddContentButton("Reduce HP", (evt) => AlterVitals("LoseHP"));
//         Modal.AddContentButton("Recover HP", (evt) => AlterVitals("GainHP"));
//         Modal.AddContentButton("Reduce VIG", (evt) => AlterVitals("LoseVIG"));
//         Modal.AddContentButton("Recover VIG", (evt) => AlterVitals("GainVIG"));
//         Modal.AddSeparator();
//         Modal.AddContentButton("Add Wound", (evt) => AlterVitals("GainWound"));
//         Modal.AddContentButton("Remove Wound", (evt) => AlterVitals("LoseWound"));
//         Modal.AddButton("Cancel", Modal.CloseEvent);
//     }

//     private static void AlterVitals(string cmd) {
//         int val = UI.Modal.Q<IntegerField>("Number").value;
//         Player.Self().CmdRequestTokenDataSetValue(Token.GetSelected().Data.Id, $"{cmd}|{val}");
//     }

//     private static void AddStatusModal(ClickEvent evt) {
//         Modal.Reset("Add Status");
//         Modal.AddDropdownField("Type", "Type", "Predefined", StringUtility.Arr("Predefined", "Simple", "Number", "Detail"), (evt) => AddStatusModalEvaluateConditions());

//         JSONNode gamedata = JSON.Parse(GameSystem.DataJson);
//         List<string> statuses = new();
//         foreach (JSONNode s in gamedata["Icon1_5"]["StatusEffects"].AsArray) {
//             statuses.Add(s["Name"]);
//         }
//         Modal.AddSearchField("PregenStatuses", "Status", "", statuses.ToArray());
//         Modal.AddTextField("Name", "Name", "");
//         Modal.AddDropdownField("Color", "Color", "Gray", StringUtility.Arr("Gray", "Green", "Red", "Blue", "Purple", "Yellow", "Orange"));
//         Modal.AddIntField("Number", "Number", 0);
//         Modal.AddTextField("Detail", "Detail", "");
//         Modal.AddPreferredButton("Add", AddStatus);
//         Modal.AddButton("Cancel", Modal.CloseEvent);

//         AddStatusModalEvaluateConditions();
//     }

//     private static void AddStatusModalEvaluateConditions() {
//         bool pregenStatus = UI.Modal.Q<DropdownField>("Type").value == "Predefined";
//         bool name = !pregenStatus;
//         bool color = !pregenStatus;
//         bool number = UI.Modal.Q<DropdownField>("Type").value == "Number";
//         bool detail = UI.Modal.Q<DropdownField>("Type").value == "Detail";

//         UI.ToggleDisplay(UI.Modal.Q("PregenStatuses"), pregenStatus);
//         UI.ToggleDisplay(UI.Modal.Q("Name"), name);
//         UI.ToggleDisplay(UI.Modal.Q("Color"), color);
//         UI.ToggleDisplay(UI.Modal.Q("Number"), number);
//         UI.ToggleDisplay(UI.Modal.Q("Detail"), detail);
//     }

//     private static void AddStatus(ClickEvent evt) {
//         string type = UI.Modal.Q<DropdownField>("Type").value;
//         string pregenStatus = SearchField.GetValue(UI.Modal.Q("PregenStatuses"));
//         string customStatus = UI.Modal.Q<TextField>("Name").value;
//         string color = UI.Modal.Q<DropdownField>("Color").value;
//         // string detail = UI.Modal.Q<TextField>("Detail").value;
//         int number = UI.Modal.Q<IntegerField>("Number").value;
//         StatusEffect s;
//         if (type == "Predefined") {
//             s = FindStatusEffect(pregenStatus);
//         }
//         else {
//             s = new StatusEffect() {
//                 Name = customStatus,
//                 Type = type,
//                 Color = color,
//                 Number = number
//             };
//         }
        
//         // Strip characters that would break parse
//         s.Name = s.Name.Replace("|", "");

//         string statusData = $"{s.Name}|{s.Type}|{s.Color}|{s.Number}";

//         Player.Self().CmdRequestTokenDataSetValue(Token.GetSelectedData().GetComponent<TokenData>(), $"GainStatus|{statusData}");        
//         Modal.Close();
//     }

//     private static StatusEffect FindStatusEffect(string name) {
//         JSONNode gamedata = JSON.Parse(GameSystem.DataJson);
//         foreach (JSONNode s in gamedata["Icon1_5"]["StatusEffects"].AsArray) {
//             if (s["Name"] == name) {
//                 return new StatusEffect() {
//                     Name = s["Name"],
//                     Color = s["Color"],
//                     Type = s["Type"]
//                 };
//             }
//         }
//         throw new Exception("Status Effect not found");
//     }
// }

// public class Icon_v1_5Interpreter {
//     public static void CreateToken() {
//         string name = UI.Modal.Q<TextField>("NameField").value;
//         Texture2D graphic = TextureSender.CopyLocalImage(UI.Modal.Q("ImageSearchField").Q<TextField>("SearchInput").value);
//         string graphicHash = TextureSender.GetTextureHash(graphic);


//         Player.Self().CmdCreateToken("Maleghast", graphicHash, name, 1, "");
//     }

//     public static void Change(string tokenId, string value) {
//         TokenData2 data = TokenData2.Find(tokenId);
//         Debug.Log($"Icon v1.5 change registered for {data.Name}: {value}");
//     }

//     public static void UpdateTokenPanel(string tokenId, string elementName) {
//         TokenData2 data = TokenData2.Find(tokenId);
//         if (!data) {
//             return;
//         }

//         data.UpdateTokenPanel(elementName);
//         VisualElement panel = UI.System.Q(elementName);
//     }    
// }