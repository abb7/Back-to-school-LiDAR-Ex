using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualClicks : StandaloneInputModule
{
    public CSGTGameManager cSGTGameManager;
    public HummerController hummerController;
    public void ClickAt(float x, float y)
    {
        Input.simulateMouseWithTouches = true;
        var pointerData = GetTouchPointerEventData(new Touch()
        {
            position = new Vector2(x, y),
        }, out bool b, out bool bb);

        ProcessTouchPress(pointerData, true, true);
        //ShootAt(pointerData);
        HitAMale(pointerData);
    }

    public void ShootAt(PointerEventData pointerData)
    {
        cSGTGameManager.Shoot(pointerData.position);
    }

    public void HitAMale(PointerEventData pointerData)
    {
        hummerController.HitAtPoint(pointerData.position);
    }
}
