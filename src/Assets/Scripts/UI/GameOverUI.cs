using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverUI : MonoBehaviour 
{
	private Text _continueText;

	private Image _adContinueButtonImage;
	private Button _adContinueButtonBehaviour;
	private Image _adContinueVideoImage;
	private Text _adContinueText;

	private Image _purchaseContinueButtonImage;
	private Button _purchaseContinueButtonBehaviour;
	private Text _purchaseContinueText;
	private Image _purchaseContinueCartImage;

	void OnEnable()
	{
		GameController.OnGameOver += HandleOnGameOver;
		GameController.OnPlayerReward += HandleOnPlayerReward;
	}

	void HandleOnPlayerReward ()
	{
		_continueText.enabled = false;

		_adContinueButtonImage.enabled = false;
		_adContinueVideoImage.enabled = false;
		_adContinueText.enabled = false;
		_adContinueButtonBehaviour.enabled = true;
		//_adContinueButtonBehaviour.interactable = GameController.Instance.promptAdContinue;
		
		_purchaseContinueButtonImage.enabled = false;
		_purchaseContinueCartImage.enabled = false;
		_purchaseContinueText.enabled = false;
		_purchaseContinueButtonBehaviour.enabled = false;
	}

	void HandleOnGameOver ()
	{
		_continueText.enabled = true;

		_adContinueButtonImage.enabled = true;
		_adContinueVideoImage.enabled = true;
		_adContinueVideoImage.color = new Color (_adContinueText.color.r, _adContinueText.color.g, _adContinueText.color.b, 
		                                        (GameController.Instance.promptAdContinue) ? 1f : .4f);
		_adContinueText.enabled = true;
		_adContinueText.text = string.Format ("Earn ({0} left)", GameController.Instance.advertisingContinues);
		_adContinueButtonBehaviour.enabled = true;
		_adContinueButtonBehaviour.interactable = GameController.Instance.promptAdContinue;

		_purchaseContinueButtonImage.enabled = true;
		_purchaseContinueCartImage.enabled = true;
		_purchaseContinueText.enabled = true;
		_purchaseContinueButtonBehaviour.enabled = true;
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
		GameController.OnGameOver -= HandleOnGameOver;
		GameController.OnPlayerReward -= HandleOnPlayerReward;
	}

	void Awake()
	{
		var adContinue = transform.FindChild("AdCont");
		_adContinueButtonImage = adContinue.GetComponent<Image> ();
		_adContinueButtonBehaviour = adContinue.GetComponent<Button> ();
		_adContinueText = adContinue.FindChild ("EarnText").GetComponent<Text>();
		_adContinueVideoImage = adContinue.FindChild ("VideoAd").GetComponent<Image>();

		_continueText = transform.FindChild ("ContinueText").GetComponent<Text>();

		var purchaseContinue = transform.FindChild("PurchaseCont");
		_purchaseContinueButtonImage = purchaseContinue.GetComponent<Image> ();
		_purchaseContinueButtonBehaviour = purchaseContinue.GetComponent<Button> ();
		_purchaseContinueText = purchaseContinue.FindChild ("PurchaseText").GetComponent<Text>();
		_purchaseContinueCartImage = purchaseContinue.FindChild ("Cart").GetComponent<Image>();
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	void OnGUI()
	{

	}
}
