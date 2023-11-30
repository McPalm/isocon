using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class Generic : GameSystem
{
    public override string SystemName()
    {
        return "Generic System";
    }

    public override string GetTokenDataRawJson() {
        return GenericTokenDataRaw.ToJson();
    }

    public override Texture2D GetGraphic(string json) {
        GenericTokenDataRaw raw = JsonUtility.FromJson<GenericTokenDataRaw>(json);
        return TextureSender.LoadImageFromFile(raw.GraphicHash, true);
    }

    public override void TokenDataSetup(GameObject g, string json, string id) {
        g.GetComponent<GenericTokenData>().TokenDataSetup(json, id);
    }

    public override GameObject GetDataPrefab() {
        return Instantiate(Resources.Load<GameObject>("Prefabs/GenericTokenData"));
    }

    public override void UpdateTokenPanel(GameObject data, string elementName)
    {
        if (data == null) {
            UI.ToggleDisplay(elementName, false);
            return;
        }
        UI.ToggleDisplay(elementName, true);
        data.GetComponent<GenericTokenData>().UpdateTokenPanel(elementName);
    }

    public override string[] GetEffectList()
    {
        return new string[]{"Wavy", "Spiky", "Hand", "Hole", "Blocked", "Corners"};
    }

    public override bool HasCustomEffect(List<string> effects)
    {
        return effects.Count > 0 && 
            !HasEffect("Wavy", effects) &&
            !HasEffect("Spiky", effects) &&
            !HasEffect("Hand", effects) &&
            !HasEffect("Hole", effects) &&
            !HasEffect("Blocked", effects);
    }

    public override void AddTokenModal()
    {
        base.AddTokenModal();

        DropdownField sizeField = new DropdownField("Size");
        sizeField.choices = new List<string>(){"1x1", "2x2", "3x3"};
        sizeField.value = "1x1";
        sizeField.name = "SizeField";
        sizeField.focusable = false;
        sizeField.AddToClassList("no-margin");
        Modal.AddContents(sizeField);
        
        IntegerField hpField = new IntegerField("HP");
        hpField.name = "HPField";
        hpField.value = 100;
        hpField.AddToClassList("no-margin");
        Modal.AddContents(hpField);
    }

}
