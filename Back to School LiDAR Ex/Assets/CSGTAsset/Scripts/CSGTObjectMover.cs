using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CSGTObjectMover : MonoBehaviour {

    public float objectSpeed = -3.0f;
    public float xForce;
    public float maxForce = 1.5f;

    private Rigidbody rigBody2D;

    private float suspendDelta = 0;

    void Start()
    {
        rigBody2D = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (suspendDelta > 0)
        {
            suspendDelta -= Time.deltaTime;
            rigBody2D.velocity = Vector2.zero;
            return;
        }

        xForce = Mathf.Clamp(xForce, -maxForce, maxForce);

        if (xForce == 0)
             xForce = -rigBody2D.transform.localPosition.x / (rigBody2D.transform.parent as RectTransform).rect.xMax; 
        
        if (xForce > 0)
            xForce -= Time.deltaTime;
        else
            xForce += Time.deltaTime;

        if (Mathf.Abs(xForce) < 0.01f)
            xForce = 0;

        rigBody2D.velocity = new Vector2(xForce, objectSpeed);
    }

    public void Suspend(float time)
    {
        suspendDelta = time;
    }
}
