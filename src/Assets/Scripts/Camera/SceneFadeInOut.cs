using System;
using UnityEngine;
using System.Collections;

public class SceneFadeInOut : MonoBehaviour
{

    public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.


    private bool sceneStarting = true;      // Whether or not the scene is still fading in.
    private int _sceneToEnd;
    private bool _sceneEnding;
    private bool _shouldRestart;

    // Subscribe to events
    void OnEnable()
    {
        GameController.OnGameRestart += HandleOnGameRestart;
        GameController.OnGamePause += HandleOnGamePause;
        GameController.OnGameResume += HandleOnGameResume;
        GameController.OnGameOver += HandleOnGameOver;
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
        GameController.OnGameRestart -= HandleOnGameRestart;
        GameController.OnGamePause -= HandleOnGamePause;
        GameController.OnGameResume -= HandleOnGameResume;
        GameController.OnGameOver -= HandleOnGameOver;
    }


    void Awake()
    {
        // Set the texture so that it is the the size of the screen and covers it.
        guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
    }

    void Update()
    {
        // If the scene is starting...
        if (sceneStarting)
        {
            // ... call the StartScene function.
            StartScene();
        }

        else if (_sceneEnding && _shouldRestart)
        {
            EndScene(_sceneToEnd);
        }

        else if (_sceneEnding && !_shouldRestart)
        {
            EndScene();
        }
    }


    void FadeToClear()
    {
        // Lerp the colour of the texture between itself and transparent.
        guiTexture.color = Color.Lerp(guiTexture.color, Color.clear, fadeSpeed * Time.deltaTime);
		//Debug.Log ("Fading in...");
    }


    void FadeToBlack(float alpha)
    {
        // Lerp the colour of the texture between itself and black.
		var blackWithAlpha = new Color (0, 0, 0, alpha);
		//guiTexture.color = blackWithAlpha;

		guiTexture.color = Color.Lerp(guiTexture.color, blackWithAlpha, fadeSpeed * Time.deltaTime);

    }


    void StartScene()
    {
        // Fade the texture to clear.
		guiTexture.enabled = true;
        FadeToClear();

        // If the texture is almost clear...
        if (guiTexture.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the GUITexture.
            guiTexture.color = Color.clear;
            guiTexture.enabled = false;

            // The scene is no longer starting.
            sceneStarting = false;
        }
	}

    public void EndScene()
    {
        // Make sure the texture is enabled.
        if (!guiTexture.enabled) {
			guiTexture.enabled = true;
		}

        // Start fading towards black.
        FadeToBlack(1f);
    }

    public void EndScene(int sceneIndex, bool shouldRestart = true)
    {  
        EndScene();
        // If the screen is almost black...
        if (guiTexture.color.a >= 0.95f && shouldRestart)
        {
            // ... reload the level.
            Application.LoadLevel(sceneIndex);   
        }
    }

    private void HandleOnGameRestart(int sceneIndex)
    {
        _sceneEnding = true;
        _shouldRestart = true;
        _sceneToEnd = sceneIndex;
    }
    private void HandleOnGameResume()
    {
		if (!_sceneEnding)
		{
			guiTexture.color = Color.clear;
			guiTexture.enabled = false;	
		}
		//Debug.Log ("Hit scene resume");
    }

    private void HandleOnGamePause()
    {
		guiTexture.enabled = true;
		//FadeToBlack (0.75f);
		guiTexture.color = new Color (0, 0, 0, 0.4f);
    }

    private void HandleOnGameOver()
    {
        _sceneEnding = true;
    }
}
