using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneFadeInOut : MonoBehaviour
{

    public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.
    public Color overlayColour = new Color (0, 0, 0, 0.4f);


    private Image _black;
    private bool _sceneStarting = true;      // Whether or not the scene is still fading in.
    private bool _sceneRunning;
    private int _sceneToEnd;
    private bool _sceneEnding;
    private bool _shouldRestart;

    // Subscribe to events
    void OnEnable()
    {
        GameController.OnGameStart += HandleOnGameStart;
        GameController.OnGameRestart += HandleOnGameRestart;
        GameController.OnGamePause += HandleOnGamePause;
        GameController.OnGameResume += HandleOnGameResume;
        GameController.OnGameOver += HandleOnGameOver;
		GameController.OnPlayerReward += HandleOnPlayerReward;
    }

    void HandleOnPlayerReward ()
    {
		_sceneEnding = false;
		_sceneRunning = true;
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void UnsubscribeEvent()
    {
        GameController.OnGameStart -= HandleOnGameStart;
        GameController.OnGameRestart -= HandleOnGameRestart;
        GameController.OnGamePause -= HandleOnGamePause;
        GameController.OnGameResume -= HandleOnGameResume;
        GameController.OnGameOver -= HandleOnGameOver;
		GameController.OnPlayerReward -= HandleOnPlayerReward;
    }


    void Awake()
    {
        // Set the texture so that it is the the size of the screen and covers it.
        //guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
        _black = GetComponent<Image>();
    }

    void Update()
    {
        // If the scene is starting...
        if (_sceneStarting)
        {
            //guiTexture.enabled = true;
            StartScene();
        }

        else if (_sceneRunning)
        {
            // ... call the StartScene function.
            RunScene();
        }

        else if (_sceneEnding && _shouldRestart)
        {
            EndScene(_sceneToEnd);
        }

        else if (_sceneEnding && !_shouldRestart)
        {
            EndScene(false);
        }
    }


    void FadeToClear()
    {
        // Lerp the colour of the texture between itself and transparent.
        _black.color = Color.Lerp(_black.color, Color.clear, fadeSpeed * Time.deltaTime);
		//Debug.Log ("Fading in...");
    }


    void FadeToBlack(float alpha)
    {
        // Lerp the colour of the texture between itself and black.
		var blackWithAlpha = new Color (0, 0, 0, alpha);
		//guiTexture.color = blackWithAlpha;

		_black.color = Color.Lerp(_black.color, blackWithAlpha, fadeSpeed * Time.deltaTime);

    }

    void StartScene()
    {
        _black.color = Color.Lerp(_black.color, overlayColour, fadeSpeed * Time.deltaTime);
    }

    void RunScene()
    {
        // Fade the texture to clear.
		//guiTexture.enabled = true;
        FadeToClear();

        // If the texture is almost clear...
        if (_black.color.a <= 0.01f)
        {
            // ... set the colour to clear and disable the GUITexture.
            _black.color = Color.clear;
            _black.enabled = false;

            // The scene is no longer starting.
            //_sceneStarting = false;
        }
	}

	public void EndScene(bool shouldRestart = true)
    {
        // Make sure the texture is enabled.
        if (!_black.enabled) {
			_black.enabled = true;
		}

        // Start fading towards black.

        FadeToBlack((shouldRestart) ? 1f : overlayColour.a);
    }

    public void EndScene(int sceneIndex, bool shouldRestart = true)
    {  
        EndScene(shouldRestart);
        // If the screen is almost black...
        if (_black.color.a >= 0.99f && shouldRestart)
        {
            // ... reload the level.
            Application.LoadLevel(sceneIndex);   
        }
    }

	private void HandleOnGameStart()
	{
		_sceneStarting = false;
		_sceneRunning = true;
	}

    private void HandleOnGameRestart(int sceneIndex)
    {
		_sceneRunning = false;
		_sceneEnding = true;
        _shouldRestart = true;
        _sceneToEnd = sceneIndex;
    }
    private void HandleOnGameResume()
    {
		if (!_sceneEnding)
		{
			_black.color = Color.clear;
			_black.enabled = false;	
		}
		//Debug.Log ("Hit scene resume");
    }

    private void HandleOnGamePause()
    {
		_black.enabled = true;
		//FadeToBlack (0.75f);
        _black.color = overlayColour;

		transform.localPosition = new Vector3 (0, 0, 1f);
    }

    private void HandleOnGameOver()
    {
        _sceneEnding = true;
        _sceneRunning = false;
        _black.enabled = true;

		transform.localPosition = new Vector3 (0, 0, 1f);
    }
}
