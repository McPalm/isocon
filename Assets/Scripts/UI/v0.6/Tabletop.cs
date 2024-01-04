using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class Tabletop : MonoBehaviour
{
    void Start()
    {
        Modal.Setup();
        MapEdit.Setup();
        SelectionMenu.Setup();
        DiceRoller.Setup();
        ConnectionSetup();
        BottomBarSetup();
        FloatingControlsSetup();
        TurnIndicatorSetup();

        UI.System.Q("TerrainInfo").Q("AddEffectButton").RegisterCallback<ClickEvent>(AddTerrainEffect.OpenModal);
    }

    void Update()
    {
        UI.ToggleDisplay("Tabletop", NetworkClient.isConnected);
        TileShare.Offsets();

        if (GameSystem.Current() != null) {
            Token selected = Token.GetSelected();
            GameSystem.Current().UpdateTokenPanel(selected != null ? selected.Data.Id : null, "SelectedTokenPanel");

            Token focused = Token.GetFocused();
            GameSystem.Current().UpdateTokenPanel(focused != null ? focused.Data.Id : null, "FocusedTokenPanel");

            if (selected != null) {
                if (selected.Data.Placed) {
                    UI.FollowToken(selected, UI.System.Q("SelectionMenu"), Camera.main, new Vector2(100, 0), true);
                    UI.System.Q("SelectionMenu").style.translate = new StyleTranslate(new Translate(Length.Percent(-50), Length.Percent(-50)));
                }
                else {
                    UI.System.Q("SelectionMenu").style.top = 0;
                    UI.System.Q("SelectionMenu").style.left = 0;
                    UI.System.Q("SelectionMenu").style.translate = new StyleTranslate(new Translate(0, 0));
                }

            }
        }
        
        UI.ToggleDisplay(UI.System.Q("FloatingControls").Q("Config"), Cursor.Mode != CursorMode.Editing);
        UI.ToggleDisplay(UI.System.Q("FloatingControls").Q("Dice"), Cursor.Mode != CursorMode.Editing);
    }

    public void ConnectAsClient() {
        Token.CutoutSize = PlayerPrefs.GetFloat("TokenScale", 1f);
        UI.System.Q("FloatingControls").Q("Connection").Q<Label>("Message").text = "You are connected as a client.";
        UI.ToggleDisplay(UI.System.Q("FloatingControls").Q("Connection"), true);
    }

    public void ConnectAsHost() {
        Token.CutoutSize = PlayerPrefs.GetFloat("TokenScale", 1f);
        TerrainController.InitializeTerrain(8, 8, 1);
        Label message = UI.System.Q("FloatingControls").Q("Connection").Q<Label>("Message");
        message.text = "You are hosting. You need to have port forwarding for port 7777 TCP to your local IP. Other players will connect to your public IP, which appears to be <IP>. If you're unable to use port forwarding you can use a service like Ngrok or Hamachi.";
        IPFinder.GetPublic(message);

        UI.ToggleDisplay(UI.System.Q("FloatingControls").Q("Connection"), true);
        BlockMesh.ToggleBorders(false);
    }

    public void ConnectAsSolo() {
        Token.CutoutSize = PlayerPrefs.GetFloat("TokenScale", 1f);
        TerrainController.InitializeTerrain(8, 8, 1);
        UI.ToggleDisplay(UI.System.Q("FloatingControls").Q("Connection"), false);
        BlockMesh.ToggleBorders(false);
    }

    private void ConnectionSetup() {
        UI.System.Q("FloatingControls").Q("Connection").RegisterCallback<MouseEnterEvent>((evt) =>  {
            UI.ToggleDisplay(UI.System.Q("FloatingControls").Q("Connection").Q("Panel"), true);
        });
        
        UI.System.Q("FloatingControls").Q("Connection").RegisterCallback<MouseLeaveEvent>((evt) =>  {
            UI.ToggleDisplay(UI.System.Q("FloatingControls").Q("Connection").Q("Panel"), false);
        });
    }

    private void FloatingControlsSetup() {
        VisualElement root = UI.System.Q("FloatingControls");
        VisualElement locator = UI.System.Q("FControlLocator");

        root.style.opacity = 0f;
        locator.style.opacity = 1f;
        root.RegisterCallback<MouseEnterEvent>((evt) => {
            root.style.opacity = 1f;
            locator.style.opacity = 0f;
        });
        root.RegisterCallback<MouseLeaveEvent>((evt) => {
            root.style.opacity = 0f;
            locator.style.opacity = 1f;
        });
        UI.SetBlocking(UI.System, "FloatingControls");
        UI.SetBlocking(UI.System, "SelectedTokenPanel");
        UI.SetBlocking(UI.System, "FocusedTokenPanel");
        UI.HoverSetup(root.Q("EditMap"));
        UI.HoverSetup(root.Q("Config"));
        UI.HoverSetup(root.Q("RotateCCW"));
        UI.HoverSetup(root.Q("RotateCW"));
        UI.HoverSetup(root.Q("Connection"));
        UI.HoverSetup(root.Q("FixedView"));
        UI.HoverSetup(root.Q("Indicators"));
        UI.HoverSetup(root.Q("Dice"));

        root.Q("Dice").RegisterCallback<ClickEvent>(DiceRoller.ToggleVisible);
        root.Q("EditMap").RegisterCallback<ClickEvent>(MapEdit.ToggleEditMode);
        root.Q("Config").RegisterCallback<ClickEvent>(Config.OpenModal);
        root.Q("Indicators").RegisterCallback<ClickEvent>((evt) => {
            bool val = !TerrainController.Indicators;
            TerrainController.Indicators = val;
            if (val) {
                root.Q("Indicators").AddToClassList("active");
            }
            else {
                root.Q("Indicators").RemoveFromClassList("active");
            }
        });
    }

    private void BottomBarSetup() {
        UI.System.Q("BottomBar").Q("AddToken").RegisterCallback<ClickEvent>((evt) => {
            AddToken.OpenModal(evt);
        });
    }

    private void TurnIndicatorSetup() {
        UI.System.Q<Button>("TurnAdvance").RegisterCallback<ClickEvent>((evt) => {
            Modal.DoubleConfirm("Advance Turn", GameSystem.Current().TurnAdvanceMessage(), () => {
                Player.Self().CmdRequestGameDataSetValue("IncrementTurn");
            });
        });
         UI.System.Q("TerrainInfo").Q<Button>("ClearSelected").RegisterCallback<ClickEvent>((evt) => {
            Block.DeselectAll();
         });
    }
}
