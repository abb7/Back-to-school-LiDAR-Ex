using UnityEngine;
using System.Collections;

public class CSGTTrigger : MonoBehaviour {

    public string TargetTag = "Target";
    public string TargetMessage = "Hit";

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == TargetTag)
        {
            //Debug.Log("Enemy noticed");
            other.SendMessage(TargetMessage, transform);
			//other.GetComponent<CSGTEnemy> ().HitDeathLine();
        }
    }
}
