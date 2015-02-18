using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialUI : MonoBehaviour 
{
	private bool _tutorialOpen;
	public GameObject[] tutorialPanels;

	void OnEnable()
	{
		CameraMovement.OnMovePlayerToGamePosition += HandleOnMovePlayerToGamePosition;
		EasyTouch.On_SimpleTap += HandleOn_SimpleTap;
	}

	void HandleOn_SimpleTap (Gesture gesture)
	{
		if (_tutorialOpen)
		{
			ButtonTutorialClose();
		}
	}

	void HandleOnMovePlayerToGamePosition (Vector3 playerPosition)
	{
		if (!PlayerState.Instance.Data.viewedTutorialGeneral)
		{
			var introPanel = Instantiate(tutorialPanels[0]) as GameObject;
			introPanel.name = "TutorialGeneral";
			introPanel.transform.SetParent(transform);

			var closeButton = introPanel.GetComponentInChildren<Button>();
			closeButton.onClick.AddListener(ButtonTutorialClose);

			var panel = introPanel.GetComponent<Image>();
			panel.rectTransform.offsetMin = new Vector2(-175f, -100f);
			panel.rectTransform.offsetMax = new Vector2(175f, 100f);
			introPanel.transform.localScale = new Vector2(1f, 1f);

			_tutorialOpen = true;
			Time.timeScale = 0;
		}
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
		CameraMovement.OnMovePlayerToGamePosition -= HandleOnMovePlayerToGamePosition;
		EasyTouch.On_SimpleTap -= HandleOn_SimpleTap;
	}

	// Use this for initialization
	void Start () 
	{
		if (tutorialPanels == null || tutorialPanels.Length == 0)
		{
			Debug.LogError("No tutorial prefabs assigned to TutorialUI component.");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void ButtonTutorialClose()
	{
		Destroy (transform.FindChild("TutorialGeneral").gameObject);
		PlayerState.Instance.Data.viewedTutorialGeneral = true;
		PlayerState.Instance.Save ();

		_tutorialOpen = false;
		Time.timeScale = 1;
	}
}
