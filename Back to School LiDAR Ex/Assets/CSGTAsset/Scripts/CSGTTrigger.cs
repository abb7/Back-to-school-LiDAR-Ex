using UnityEngine;
using System.Collections;

public class CSGTTrigger : MonoBehaviour {

    public string TargetTag = "Target";
    public string TargetMessage = "Hit";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == TargetTag)
        {
            //other.SendMessage(TargetMessage, transform);
			other.GetComponent<CSGTEnemy> ().HitDeathLine();
        }
    }
}
