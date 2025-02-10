using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    [SerializeField] private Texture2D cursorTexture; 
    [SerializeField] private Texture2D dragCursorTexture;
    [SerializeField] private Texture2D clickCursorTexture;
    [SerializeField] private GameObject crosshair;

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
    
    public void ChangeCursorMode(CursorLockMode mode, bool isVisible)
    {
        Cursor.lockState = mode == CursorLockMode.Locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = isVisible;

        if (crosshair == null) return;

        crosshair.SetActive(!isVisible);
    }

    public void SetDragCursor(){
        Cursor.SetCursor(dragCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void SetDefaultCursor(){
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void SetClickCursor(){
        Cursor.SetCursor(clickCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DisableCursor()
    {
        Cursor.visible = false;
    }
}
