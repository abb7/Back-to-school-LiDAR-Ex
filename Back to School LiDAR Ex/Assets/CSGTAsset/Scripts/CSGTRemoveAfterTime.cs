using UnityEngine;
using System.Collections;

public class CSGTRemoveAfterTime : MonoBehaviour {

    public float removeAfterTime = 0.1f;

    void Start()
    {
        Destroy(gameObject, removeAfterTime);
    }
}
