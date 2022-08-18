using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

public class CSGTGameManager : MonoBehaviour {

    public static CSGTGameManager instance = null;

    float startGameSpeed = 1.0f;

    [Header("Camera")]
    public Camera mainCamera;
    public float cameraShakeDecay = 0.002f;
    public float cameraStartShakeIntensity = 0.1f;
    private Vector3 cameraOriginPosition;
    private Quaternion cameraOriginRotation;

    [Header("Spawn")]
    public RectTransform spawnLine;

    [Header("Spawn Time Management")]
    public float startSpawnSpeed = 2.0f;
    public float spawnStep = 0.05f;
    public float minSpawSpeed = 0.48f;


    [Header("Spawn Objects")]
    public GameObject[] spawnGameObjects;
    private GameObject spawnObject;
    public float[] spawnObjectsXPos = new float[6] { -2.25f, -1.5f, -0.75f, 0.75f, 1.5f, 2.25f };

    [Header("Sounds")]
    public AudioClip buttonClickSound;
    public AudioClip gameOverSound;

    [Header("Visuals")]
	public Image[] playerLiveHearts;
	public GameObject[] liveSplashEffect;
    public Text gameScoreText;
    public Text gameBestScoreText;
    public Text gameLastScoreText;
    public Text gameOverScoreText;
    public Text gameOverNewText;
    public Text gameOverHighScoreText;

    [Header("Menus")]
    public GameObject menuCanvas;
    public GameObject gameCanvas;
    public GameObject pauseCanvas;
	public GameObject gameEndCanvas;
    public GameObject gameOverCanvas;

    [Header("Quit")]
    public string gameOverURL = "http://u3d.as/tCH";

    int gameScore = 0;
	int lives = 3;
    int highGameScore = 0;
    int lastGameScore = 0;
    int bonusGameCount = 0;
    int lastHighGameScore = 0;

    float spawnTime = 2.0f;
    float spawnSpeed = 2.0f;

    internal bool isGameOver = true;
    internal bool isGamePaused = false;

    internal Vector3 aimMousePosition;
    internal Vector3 aimTouchPosition;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //LoadGameData();
        //ShowGameMenu();
        GameStart();
        //GoogleAdsManager.Instance.InterstialDoneCallback = InterstialDone;
    }

    void Update()
    {
        if ((!isGameOver) && (!isGamePaused))
        {
            spawnTime -= Time.deltaTime;
            if (spawnTime <= 0.0f)
            {
                SpawnNewObject();
                if (spawnSpeed > minSpawSpeed)
                    spawnSpeed -= spawnStep;
                else
                    spawnSpeed = minSpawSpeed;

                spawnTime = spawnSpeed;
            }

            UpdateInput();
            UpdateGameData();
        }
    }

    public void UpdateInput()
    {
        if (!Application.isMobilePlatform)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                aimMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Shoot(aimMousePosition);
            }
        }

        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0)
            {
                for (int t = 0; t < Input.touchCount; t++)
                {
                    Touch touch = Input.GetTouch(t);
                    TouchPhase phase = touch.phase;

                    if (phase == TouchPhase.Began)
                    {
                        aimTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                        Shoot(aimTouchPosition);
                    }
                }
            }
        }
        
    }

    public void Shoot(Vector3 aimPosition)
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(aimPosition, Vector2.zero);
        if (raycastHit2D.collider != null)
        {
            if (raycastHit2D.collider.tag == "Enemy")
            {
                raycastHit2D.collider.gameObject.GetComponent<CSGTEnemy>().hit();
            }
           
        }
    }

    void SpawnNewObject()
    {
        float spawnObjectXPos = spawnObjectsXPos[Random.Range(0, spawnObjectsXPos.Length)];
        Vector3 spawnObjectPos = new Vector3(spawnObjectXPos, spawnLine.position.y, 0);
        spawnObject = spawnGameObjects[Random.Range(0, spawnGameObjects.Length)];
        GameObject newEnemy = (GameObject)(Instantiate(spawnObject, spawnObjectPos, spawnObject.transform.rotation
            ));
        newEnemy.transform.localScale = new Vector3(200f, 200f, 200f);

        newEnemy.transform.SetParent(spawnLine);
        newEnemy.transform.SetAsFirstSibling();

    }

    void CleanUpScene()
    {
        CSGTSplashPool.instance.Clear();
        for (int i = 0; i < spawnLine.childCount; i++)
            Destroy(spawnLine.GetChild(i).gameObject);
    }

    void LoadGameData()
    {
#if UNITY_5_3_OR_NEWER
        // DELETE ALL GAME DATA !!!!! PlayerPrefs.DeleteAll();
        highGameScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "HIGH_GAMESCORE", 0);
        lastGameScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "LAST_GAMESCORE", 0);
        bonusGameCount = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "BONUS_GAMECOUNT", 0);
        lastHighGameScore = highGameScore;
#else
        // DELETE ALL GAME DATA !!!!! PlayerPrefs.DeleteAll();
		highGameScore = PlayerPrefs.GetInt(Application.loadedLevelName + "HIGH_GAMESCORE", 0);
		lastGameScore = PlayerPrefs.GetInt(Application.loadedLevelName + "LAST_GAMESCORE", 0);
		bonusGameCount = PlayerPrefs.GetInt(Application.loadedLevelName + "BONUS_GAMESCORE", 0);
        lastHighGameScore = highGameScore;
#endif
    }

    void UpdateGameData()
    {
        if (gameScore > highGameScore)
            highGameScore = gameScore;

        if (!isGameOver)
        {
            gameScoreText.text = gameScore.ToString();
            gameBestScoreText.text = "Best: " + highGameScore.ToString();
            gameLastScoreText.text = "Last: " + lastGameScore.ToString();
        }
        else
        {
            gameOverScoreText.text = "Score: " + lastGameScore.ToString();
            gameOverHighScoreText.text = "HIGH Score: " + highGameScore.ToString();
        }
    }

    void SaveGameData()
    {
#if UNITY_5_3_OR_NEWER
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HIGH_GAMESCORE", highGameScore);
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "LAST_GAMESCORE", lastGameScore);
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "BONUS_GAMECOUNT", bonusGameCount);
#else
		PlayerPrefs.SetInt(Application.loadedLevelName + "HIGH_GAMESCORE", highGameScore);
		PlayerPrefs.SetInt(Application.loadedLevelName + "LAST_GAMESCORE", lastGameScore);
		PlayerPrefs.SetInt(Application.loadedLevelName + "BONUS_GAMECOUNT", bonusGameCount);
#endif
    }

	private void SetHeartState(int index, int state, bool doSplash)
	{
		if ((index >= 0) && (index < playerLiveHearts.Length))
		{
			if (state >= 0) {
				playerLiveHearts [index].enabled = true;
			} else {
				playerLiveHearts [index].enabled = false;
				Vector3 heartPos = mainCamera.ScreenToWorldPoint (playerLiveHearts [index].transform.position);
				Vector3 splashPos = new Vector3 (heartPos.x, heartPos.y, 1);
				GameObject liveSplash = (GameObject)Instantiate(liveSplashEffect[UnityEngine.Random.Range(0, liveSplashEffect.Length)], splashPos, Quaternion.identity);
				Destroy(liveSplash, 1);
			}
		}
	}

	private void UpdateHearts()
	{
		int activeHeart = playerLiveHearts.Length - lives;
		SetHeartState (activeHeart, -1, true);
	}

	private void ResetHearts()
	{
		for (int i = 0; i < 3; i++)
		{
			int activeHeart = playerLiveHearts.Length - lives;
			int state = 1;
			SetHeartState(i, state, state < 0); 
		}
	}

    public IEnumerator CameraShake()
    {
        cameraOriginPosition = mainCamera.transform.position;
        cameraOriginRotation = mainCamera.transform.rotation;

        float cameraShakeIntensity = cameraStartShakeIntensity;
        while (cameraShakeIntensity > 0)
        {
            mainCamera.transform.position = cameraOriginPosition + Random.insideUnitSphere * cameraShakeIntensity;
            mainCamera.transform.rotation = new Quaternion(
                cameraOriginRotation.x + Random.Range(-cameraShakeIntensity, cameraShakeIntensity) * .2f,
                cameraOriginRotation.y + Random.Range(-cameraShakeIntensity, cameraShakeIntensity) * .2f,
                cameraOriginRotation.z + Random.Range(-cameraShakeIntensity, cameraShakeIntensity) * .2f,
                cameraOriginRotation.w + Random.Range(-cameraShakeIntensity, cameraShakeIntensity) * .2f);
            cameraShakeIntensity -= cameraShakeDecay;
            yield return false;
        }
    }
    
    #region --------------- MENUS AND GAME CONTROL --------------- 

    public void ShowGameMenu()
    {
        gameOverCanvas.SetActive(false);
		gameEndCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        menuCanvas.SetActive(true);

        DisplayBanner(true);
    }

    public void ShowGamePlayMenu()
    {
        gameCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
		gameEndCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        menuCanvas.SetActive(false);

        DisplayBanner(false); 
    }

    public void ShowPauseMenu()
    {
        gameCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
		gameEndCanvas.SetActive(false);
        pauseCanvas.SetActive(true);
        menuCanvas.SetActive(false);

        DisplayBanner(true);
    }

	public void ShowGameEndMenu()
	{
		gameOverCanvas.SetActive(false);
		gameEndCanvas.SetActive(true);
		pauseCanvas.SetActive(false);
		gameCanvas.SetActive(false);
		menuCanvas.SetActive(false);

		//DisplayBanner(true);
	}

    public void ShowGameOverMenu()
    {
        gameOverCanvas.SetActive(true);
		gameEndCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        menuCanvas.SetActive(false);

        DisplayBanner(true);
    }

    public void GameMenu()
    {
        Time.timeScale = startGameSpeed;
        ButtonSound();
        ShowGameMenu();
    }

    public void GameStart()
    {
        ButtonSound();
        if (isGamePaused)
            GameResume();

		lives = 3;
		isGameOver = false;
        gameScore = 0;

        spawnSpeed = startSpawnSpeed;
        spawnTime = spawnSpeed;

        RequestInterstial();

        CleanUpScene();
        UpdateGameData();
		ResetHearts ();

        ShowGamePlayMenu();

        Time.timeScale = startGameSpeed;
    }

    public void GameRestart()
    {
        ButtonSound();
        GameStart();
    }

    public void GamePause()
    {
        ButtonSound();
        isGamePaused = true;
        Time.timeScale = 0;

        ShowPauseMenu();
    }

    public void GameResume()
    {
        ButtonSound();
        isGamePaused = false;
        Time.timeScale = startGameSpeed;

        ShowGamePlayMenu();
    }

    public void GameStop()
    {
        ButtonSound();
		if (isGamePaused)
			GameResume();

		isGameOver = true;
		ShowGameEndMenu();
		StartCoroutine(GameOverAsync()); 
    }

	public void GameOver()
	{
		if (isGamePaused)
			GameResume ();
        

        if (!isGameOver) {
            if (gameOverSound != null)
                CSGTSoundManager.instance.PlaySound(gameOverSound);
            StartCoroutine(CameraShake());
            UpdateHearts();
			if (lives > 0)
				lives--;
		}

		if ((!isGameOver) && (lives <= 0)) {
			isGameOver = true;
			lives = 0;
			ShowGameEndMenu();
			StartCoroutine(GameOverAsync()); 
		}
	}

	IEnumerator GameOverAsync()
	{
		yield return new WaitForSeconds (3);
		CleanUpScene();

		lastGameScore = gameScore;
		UpdateGameData();
		SaveGameData();
		if (gameScore > lastHighGameScore)
		{
			gameOverNewText.text = "NEW";
			lastHighGameScore = gameScore;
		}
		else
			gameOverNewText.text = "";
		gameScore = 0;
		ShowGameOverMenu();
        DisplayInterstial();
    }

    public void GameQuit()
    {
        ButtonSound();
        SaveGameData();
    }

    public void GameQuitNow()
    {
        ButtonSound();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
            Application.OpenURL(gameOverURL);
#else
            Application.Quit();
#endif   
    }

    public void UpdateScore(int scoreValue)
    {
        gameScore += scoreValue;
        UpdateGameData();
    }

    void ButtonSound()
    {
        if(buttonClickSound != null)
            CSGTSoundManager.instance.PlaySound(buttonClickSound);
    }

    #endregion

    #region --------------- ADVERTISING ---------------

    public void InterstialDone()
    {
        GameQuitNow();
    }

    private void RequestInterstial()
    {
        //GoogleAdsManager.Instance.RequestInterstitial();
    }

    private void DisplayInterstial()
    {
        /*if (GoogleAdsManager.Instance.displayInterstial)
            GoogleAdsManager.Instance.ShowInterstitial();
        else
            InterstialDone();
    */
        }

    private void DisplayBanner(bool show)
    {
        /*if (GoogleAdsManager.Instance.displayBanner)
        {
            if (show)
                GoogleAdsManager.Instance.ShowBanner();
            else
                GoogleAdsManager.Instance.HideBanner();
        }*/
    }

    #endregion

}
