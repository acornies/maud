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
	private Animator _purchaseContinueAnimator;

	public Sprite purchasedContinueIcon;
	public Sprite purchasedCartIcon;

	public Vector2 holdingPosition;

	void OnEnable()
	{
		GameController.OnGameOver += HandleOnGameOver;
		GameController.OnPlayerReward += HandleOnPlayerReward;
		//StoreController.Instance.Native.OnTransactionComplete += HandleOnTransactionComplete;
		PlayerState.OnSuccessfullContinuationPurchase += HandleOnSuccessfullContinuationPurchase;
	}

	void HandleOnSuccessfullContinuationPurchase ()
	{
		_purchaseContinueAnimator.enabled = true;
	}

	void HandleOnPlayerReward ()
	{
		//_continueText.enabled = false;
		_continueText.rectTransform.anchoredPosition = holdingPosition;

		//_adContinueButtonImage.enabled = false;
		_adContinueButtonImage.rectTransform.anchoredPosition = holdingPosition;

		//_adContinueVideoImage.enabled = false;
		//_adContinueVideoImage.rectTransform.anchoredPosition = holdingPosition;

		//_adContinueText.enabled = false;
		_adContinueText.rectTransform.anchoredPosition = holdingPosition;


		//_adContinueButtonBehaviour.enabled = true;
		//_adContinueButtonBehaviour.interactable = GameController.Instance.promptAdContinue;
		
		//_purchaseContinueButtonImage.enabled = false;
		_purchaseContinueButtonImage.rectTransform.anchoredPosition = holdingPosition;

		//_purchaseContinueCartImage.enabled = false;
		//_purchaseContinueCartImage.rectTransform.anchoredPosition = holdingPosition;

		//_purchaseContinueText.enabled = false;
		_purchaseContinueText.rectTransform.anchoredPosition = holdingPosition;
		//ShowBuyOrContinue ();
		//_purchaseContinueButtonBehaviour.enabled = false;

	}

	void ShowBuyOrContinue()
	{
		if (!GameController.Instance.promptPurchaseContinue)
		{
			_purchaseContinueText.text = string.Format ("Go ({0} left)", PlayerState.Instance.Data.gameOverContinues);
			_purchaseContinueCartImage.sprite = purchasedContinueIcon;
			_purchaseContinueCartImage.rectTransform.anchoredPosition = new Vector2(4f, 0);
		}
		else
		{
			_purchaseContinueText.text = string.Format ("Buy x10");
			_purchaseContinueCartImage.sprite = purchasedCartIcon;
			_purchaseContinueCartImage.rectTransform.anchoredPosition = new Vector2(0, 0);
		}
	}

	void HandleOnGameOver ()
	{
		//_continueText.enabled = true;
		_continueText.rectTransform.anchoredPosition = new Vector2 (0, 0);

		//_adContinueButtonImage.enabled = true;
		_adContinueButtonImage.rectTransform.anchoredPosition = new Vector2 (-100f, -100f);
		//_adContinueVideoImage.enabled = true;
		_adContinueVideoImage.color = new Color (_adContinueText.color.r, _adContinueText.color.g, _adContinueText.color.b, 
		                                        (GameController.Instance.promptAdContinue) ? 1f : .4f);
		_adContinueText.rectTransform.anchoredPosition = new Vector2 (-100f, -160f);
		_adContinueText.text = string.Format ("Earn ({0} left)", GameController.Instance.advertisingContinues);
		//_adContinueButtonBehaviour.enabled = true;
		_adContinueButtonBehaviour.interactable = GameController.Instance.promptAdContinue;

		//_purchaseContinueButtonImage.enabled = true;
		_purchaseContinueButtonImage.rectTransform.anchoredPosition = new Vector2 (100f, -100f);
		//_purchaseContinueCartImage.enabled = true;
		//_purchaseContinueText.enabled = true;
		_purchaseContinueText.rectTransform.anchoredPosition = new Vector2 (100f, -160f);
		ShowBuyOrContinue ();
		//_purchaseContinueButtonBehaviour.enabled = true;
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
		PlayerState.OnSuccessfullContinuationPurchase += HandleOnSuccessfullContinuationPurchase;
	}

	void Awake()
	{
		var adContinue = transform.FindChild("AdCont");
		_adContinueButtonImage = adContinue.GetComponent<Image> ();
		_adContinueButtonBehaviour = adContinue.GetComponent<Button> ();
		_adContinueText = transform.FindChild ("EarnText").GetComponent<Text>();
		_adContinueVideoImage = adContinue.FindChild ("VideoAd").GetComponent<Image>();

		_continueText = transform.FindChild ("ContinueText").GetComponent<Text>();

		var purchaseContinue = transform.FindChild("PurchaseCont");
		_purchaseContinueButtonImage = purchaseContinue.GetComponent<Image> ();
		_purchaseContinueButtonBehaviour = purchaseContinue.GetComponent<Button> ();
		_purchaseContinueText = transform.FindChild ("PurchaseText").GetComponent<Text>();
		_purchaseContinueCartImage = purchaseContinue.FindChild ("Cart").GetComponent<Image>();

		_purchaseContinueAnimator = purchaseContinue.GetComponent<Animator> ();
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
		if (GameController.Instance.gameState != LegendPeak.GameState.Over) return;
		ShowBuyOrContinue ();
	}
}
