using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public void onButtonClicked()
    {
        Debug.Log("test");
    }

    private void OnMouseOver()
    {
        gameObject.GetComponent<Image>().color = Color.red;
        Debug.Log("Test");
    }

    private void OnMouseDrag()
    {
        Debug.Log("test2");
    }

    
}
