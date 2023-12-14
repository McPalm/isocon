using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mirror;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class Token : MonoBehaviour
{
    public int Size = 1;
    public Texture2D Image;

    public GameObject offlineDataObject;
    public GameObject onlineDataObject;

    public float ShareOffsetX;
    public float ShareOffsetY;

    public Token LastFocused;

    public bool Selected = false;
    public bool Focused = false;

    void Update()
    {
        alignToCamera();
        UpdateVisualEffect();
        Offset();

        if (Focused && this != LastFocused) {
            Unfocus();
        }

    }

    private void alignToCamera() {
        Transform camera = GameObject.Find("CameraOrigin").transform;
        transform.Find("Offset").transform.rotation = Quaternion.Euler(0, camera.eulerAngles.y + 90, 0);
    }

    private void Offset() {
        float x = ShareOffsetX;
        float y = ShareOffsetY;
        if (Size == 2) {
            x = 0;
            y = -.73f;
        }
        else if (Size == 3) {
            x = 0;
            y = 0;
        }
        transform.Find("Offset").transform.localPosition = new Vector3(x, .2f, y);
        transform.Find("Base").transform.localPosition = new Vector3(x, .2f,y);
    }

    public void SetImage(Texture2D image) {
        Image = image;
        float aspectRatio = Image.width/(float)Image.height;
        transform.Find("Offset/Avatar/Cutout/Cutout Quad").GetComponent<MeshRenderer>().material.SetTexture("_Image", Image);
        transform.Find("Offset/Avatar/Cutout/Cutout Quad").transform.localScale = new Vector3(aspectRatio, 1f, 1f);
    }

    public void LeftClick() {
        switch (Cursor.Mode) {
            case CursorMode.Placing:
            case CursorMode.Moving:
                TokenMenu.EndCursorMode();
                break;
        }

        if (Selected) {
            Deselect();
        }
        else {
            Select();
        }
    }

    public void RightClick() {

    }

    public void Place(Block block) {
        Vector3 v = block.transform.position + new Vector3(0, .25f, 0);
        Player.Self().CmdRequestPlaceToken(onlineDataObject, v);
        Cursor.Mode = CursorMode.Default;
    }

    public void Move(Block block) {
        Vector3 v = block.transform.position + new Vector3(0, .25f, 0);
        Player.Self().CmdMoveToken(onlineDataObject, v, false);
    }

    public void Select() {
        UnfocusAll();
        DeselectAll();
        Selected = true;
        TokenData data = onlineDataObject.GetComponent<TokenData>();
        data.Select();
        UI.ToggleDisplay(data.UnitBarElement.Q("Selected"), true); // selected indicator in unit bar
        UI.ToggleDisplay("SelectedTokenPanel", true); // selected token panel
        TokenMenu.ShowMenu();
    }

    public void Deselect() {
        Selected = false;
        UI.ToggleDisplay(onlineDataObject.GetComponent<TokenData>().UnitBarElement.Q("Selected"), false);
        UI.ToggleDisplay("SelectedTokenPanel", false);
        SelectionMenu.Hide();
        Cursor.Mode = CursorMode.Default;
    }

    public static void DeselectAll() {
        Token t = GetSelected();
        if (t) {
            t.Deselect();
        }
    }

    public static Token GetSelected() {
        GameObject[] tokens = GameObject.FindGameObjectsWithTag("Token");
        for (int i = 0; i < tokens.Length; i++) {
            if (tokens[i].GetComponent<Token>().Selected) {
                return tokens[i].GetComponent<Token>();
            }
        }
        return null;
    }

    public static GameObject GetSelectedData() {
        Token selected = GetSelected();
        if (selected != null && selected.onlineDataObject != null) {
            return selected.onlineDataObject;
        }
        return null;
    }

    public void Focus() {
        if (Token.GetSelected() == this) {
            return;
        }
        UnfocusAll();
        TokenData data = onlineDataObject.GetComponent<TokenData>();
        data.Focus();
        LastFocused = this;
        Focused = true;
    }

    public void Unfocus() {
        Focused = false;
    }

    public static Token GetFocused() {
        GameObject[] tokens = GameObject.FindGameObjectsWithTag("Token");
        for (int i = 0; i < tokens.Length; i++) {
            if (tokens[i].GetComponent<Token>().Focused) {

                return tokens[i].GetComponent<Token>();
            }
        }
        return null;
    }

    public static GameObject GetFocusedData() {
        Token focused = GetFocused();
        if (focused != null && focused.onlineDataObject != null) {
            return focused.onlineDataObject;
        }
        return null;
    }

    public static void UnfocusAll() {
        Token t = GetFocused();
        if (t) {
            t.Unfocus();
        }
    }

    public void UpdateVisualEffect() {
        if (Selected && Cursor.Mode == CursorMode.Moving) {
            SetVisualArrows();
        }
        else if (Selected) {
            SetVisualSquareYellow();
        }
        else if (Focused) {
            SetVisualSquareBlue();
        }
        else {
            SetVisualNone();
        }
    }

    private void SetVisualArrows() {
        SetVisual(false, true, false, false);
    }

    private void SetVisualSquareYellow() {
        SetVisual(true, false, false, false);
    }

    private void SetVisualSquareBlue() {
        SetVisual(false, false, true, false);
    }

    private void SetVisualNone() {
        SetVisual(false, false, false, false);
    }

    private void SetVisual(bool yellowSquare, bool yellowArrows, bool blueSquare, bool blueArrows) {
        transform.Find("Offset/Select").GetComponent<MeshRenderer>().material.SetInt("_Selected", yellowSquare ? 1:0);
        transform.Find("Offset/Select").GetComponent<MeshRenderer>().material.SetInt("_Moving", yellowArrows ? 1:0);
        transform.Find("Offset/Focus").GetComponent<MeshRenderer>().material.SetInt("_Selected", blueSquare ? 1:0);
        transform.Find("Offset/Focus").GetComponent<MeshRenderer>().material.SetInt("_Moving", blueArrows ? 1:0);
    }


    public void SetDefeated(bool defeated) {
        transform.Find("Offset/Avatar/Cutout/Cutout Quad").GetComponent<MeshRenderer>().material.SetInt("_Dead", defeated ? 1 : 0);
    }
    
}
