using UnityEngine;
using System.Collections.Generic;


public class CSGTSplashPool : MonoBehaviour
{

    public static CSGTSplashPool instance;

    public int MaxCapacity = 100;
    private List<GameObject> SplashPool = new List<GameObject>();

    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Update()
    {
        while ((SplashPool.Count > MaxCapacity) && (SplashPool.Count > 0))
        {
            Destroy(SplashPool[0]);
            SplashPool.RemoveAt(index: 0);
        }
    }

    public void Clear()
    {
        foreach (GameObject splash in SplashPool)
            Destroy(splash);
        SplashPool.Clear();
    }

    public void AddToPool(GameObject blood)
    {
        SplashPool.Add(blood);
    }
}
