using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CSGTEnemy : MonoBehaviour {

    public bool isDead = false;
    public bool isFriendly = false;
    public int lives = 1;
    public Transform scoreEffect;
    public Color effectColor;
    public GameObject[] bloodEffect;
    public GameObject[] splashEffect;
    public GameObject[] deadBody;
    public int scoreValue = 10;

	public AudioClip[] soundHit;
	public AudioClip[] soundDead;

    private Animator animator;
    private CSGTObjectMover mover;
	private SpriteRenderer spriteRen;
	private AudioSource audSource;
	private BoxCollider boxCollider;

    void Awake()
    {
        animator = GetComponent<Animator>();
        mover = GetComponent<CSGTObjectMover>();
		spriteRen = GetComponent<SpriteRenderer>();
		audSource = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider> ();
    }

    public void hit()
    {
        lives--;

        if (lives > 0) {
			PlaySound (soundHit [Random.Range (0, soundHit.Length)]);
			animator.Play ("Hit");
			mover.Suspend (1.0f);
		} else {
			PlaySound(soundDead[UnityEngine.Random.Range(0, soundDead.Length)]);
			isDead = true;
		}

        GameObject splashBlood = (GameObject)Instantiate(splashEffect[UnityEngine.Random.Range(0, splashEffect.Length)], transform.position, Quaternion.identity);
        splashBlood.GetComponent<ParticleSystem>().startColor = effectColor;
        Destroy(splashBlood, 1);

        if (isDead)
        {
            if (scoreEffect)
            {
				if (!isFriendly) {
					Transform newScoreTextEffect = Instantiate (scoreEffect, transform.position, Quaternion.identity) as Transform;
					newScoreTextEffect.Find ("Text").GetComponent<Text> ().text = "+" + scoreValue.ToString ();
					CSGTGameManager.instance.UpdateScore (scoreValue);
				}
            }

            GameObject deadEnemy = (GameObject)Instantiate(deadBody[UnityEngine.Random.Range(0, deadBody.Length)], transform.position, Quaternion.identity);
            GameObject bloodSplash = (GameObject)Instantiate(bloodEffect[UnityEngine.Random.Range(0, bloodEffect.Length)], transform.position, Quaternion.identity);
            bloodSplash.GetComponent<SpriteRenderer>().color = effectColor;
            CSGTSplashPool.instance.AddToPool(bloodSplash);
            if (isFriendly) CSGTGameManager.instance.GameOver();
			//spriteRen.enabled = false;
            boxCollider.enabled = false;
			mover.Suspend(2.0f);
			Destroy(gameObject, 2.0f);
        }
    }

	public void PlaySound(AudioClip clip)
	{
		if (!CSGTSoundManager.instance.efxSource.mute) {
			audSource.clip = clip;
			audSource.pitch = Random.Range (0.9f, 1.1f);
			audSource.Play ();
		}
	}

    public void HitDeathLine()
    {
		if (!isFriendly) { }
            //CSGTGameManager.instance.GameOver();
    }

	private void FixedUpdate()
	{
		SetLayerOrder ();
	}

	private void SetLayerOrder()
	{
		float ordNum = transform.localPosition.y * 10;
		//spriteRen.sortingOrder = -Mathf.FloorToInt (ordNum);
	}
}
