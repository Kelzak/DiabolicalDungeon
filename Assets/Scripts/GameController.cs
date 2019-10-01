using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public Texture2D cursorTextureRed;
    public Texture2D cursorTextureGreen;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 aimingSize = Vector2.zero;

    void Start()
    {
        //cursorTextureRed.Resize((int)aimingSize.x, (int)aimingSize.y);
        Cursor.SetCursor(cursorTextureRed, aimingSize / 2, cursorMode);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Targetable")) && (hit.collider.tag == "Enemy" || hit.collider.tag == "DoorBall"))
        {
            Cursor.SetCursor(cursorTextureGreen, aimingSize / 2, cursorMode);
            Debug.Log("Mouse is over GameObject.");
        }
        else
        {
            Cursor.SetCursor(cursorTextureRed, aimingSize / 2, cursorMode);
            Debug.Log("Mouse is not over GameObject.");
        }
    }
    //void OnMouseOver()
    //{
    //    if (gameObject.tag == "Enemy")
    //    {
    //        Cursor.SetCursor(cursorTextureGreen, hotSpot, cursorMode);
    //        Debug.Log("Mouse is over GameObject.");
    //    }
    //}

    //void OnMouseExit()
    //{
    //    Cursor.SetCursor(null, Vector2.zero, cursorMode);
    //    Debug.Log("Mouse is not over GameObject.");
    //}

}

