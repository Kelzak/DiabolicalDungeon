using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
   
    public Texture2D cursorTextureRed;
    public Texture2D cursorTextureGreen;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    void OnMouseEnter()
    {
        Cursor.SetCursor(cursorTextureRed, hotSpot, cursorMode);
    }


    void OnMouseOver()
    {
        if (gameObject.tag == "Enemy")
        {
            Cursor.SetCursor(cursorTextureGreen, hotSpot, cursorMode);
            Debug.Log("Mouse is over GameObject.");
        }
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
        Debug.Log("Mouse is not over GameObject.");
    }
    
}

