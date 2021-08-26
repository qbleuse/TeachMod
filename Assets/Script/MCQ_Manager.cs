using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* class that handles the MCQ behavior and display on the canvas dedicated.*/
public class MCQ_Manager : MonoBehaviour
{
	/*==== INSTANCE ====*/
	public static MCQ_Manager Instance = null;

	/*==== STATE ====*/
	private MCQ _currMCQ = null;
	private bool[]	_rightAnswerCache	= null;
	bool			_answeredRightOnce	= false;

	/*==== COMPONENT ====*/
	//private RectTransform   _canvasTrs      = null;
	private GameObject		_canvas			= null;
	private Toggle[]        _answers        = null;
	private RectTransform[] _answersRect    = null;
	private TextMeshProUGUI	_question       = null;
	private TextMeshProUGUI _comment		= null;
	private Text			_buttonText		= null;

	/*==== CACHE ====*/
	MonoBehaviour	_camera	= null;
	Player			_player = null;
	AnimationCam	_anim	= null;
	StringBuilder _stringBuilder = new StringBuilder();

	public event Action _OnSubmitEvent;

	/*==== ACCESSOR ====*/
	public bool IsEmpty() { return _currMCQ == null; }


	/*==== UNITY METHODS  ====*/
	private void Awake()
	{
		Instance = this;
	}

	/* you may want to look at the prefabs associated with this to understand what is being done here */
	void Start()
	{
		/*==== Camera ====*/
		/* caching the user input script to disable them */
		Camera camera = Camera.main;
		_player = camera.GetComponent<Player>();
		_anim	= camera.GetComponent<AnimationCam>();

		if (SystemInfo.deviceType == DeviceType.Handheld)
			_camera = camera.GetComponent<GyroCam>();
		else
			_camera = camera.GetComponent<DesktopCam>();

		/*==== Components ====*/
		/* get the canvas */
		_canvas		= transform.GetChild(0).gameObject;

		/* we need to move them depending on the number of question asked */
		_answers	= GetComponentsInChildren<Toggle>();
		/* five is the most possible answer it can have (see Prefabs if needed)*/
		_answersRect = new RectTransform[5];
		for (int i = 0; i < _answers.Length; i++)
		{
			_answersRect[i] = _answers[i].GetComponent<RectTransform>();
		}

		/* get the text to change the question */
		_question	= _canvas.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
		/* get the text to change the comment when the submit button is pressed */
		_comment	= _canvas.transform.GetChild(0).GetChild(6).GetComponent<TextMeshProUGUI>();
		/* get the submit the text of the submit button */
		_buttonText = _canvas.transform.GetChild(0).GetChild(7).transform.GetChild(0).GetComponent<Text>();
		_buttonText.text = "Soumettre";



		/*==== State ====*/
		/* used to precompute the answer */
		_rightAnswerCache = new bool[5];

		_OnSubmitEvent = VideoController.Instance.PauseAndResume;

		/* don't need it right now */
		gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
	}


	/*==== PRIVATE METHODS ====*/

	/* check the answer of the player and return if it has been answered or not */
	private bool CheckAnswer()
	{
		bool answered		= false;
		_answeredRightOnce	= false;
		int answersRight = 0;

		/* the user can give only from an answer he has been given so depending on _answerNb */
		for (int i = 0; i < _currMCQ._answers.Count; i++)
		{
			/* if the user has at least one good answer (it changes how things are displayed) */
			if (_answers[i].isOn && _rightAnswerCache[i])
			{
				answered			= true;
				_answeredRightOnce	= true;
				answersRight++;
			}/* if at least the user chose an answer but was all wrong, we accept it */
			else if (_answers[i].isOn)
			{
				answered = true;
			}
		}

		/*if the user pressed the button without having answered anything*/
		if (!answered)
			return false;

		/* all answers were right */
		if (answersRight == _currMCQ._rightAnswerNb.Count)
		{
			EndMenu.Instance._MCQ_Score++;
		}

		return true;
	}

	private void ShowAnswer()
	{
		/* make the array of color to show the result at the end of the test */
		_currMCQ._results = new Color[_currMCQ._answers.Count];
		for (int i = 0; i < _currMCQ._answers.Count; i++)
		{
			if (!_answers[i].isOn && _rightAnswerCache[i] && _answeredRightOnce)
			{
				_answers[i].targetGraphic.color = Color.yellow;
				_currMCQ._results[i] = Color.yellow;
			}
			else if (_rightAnswerCache[i])
			{
				_answers[i].targetGraphic.color = Color.green;
				_currMCQ._results[i] = Color.green;
			}
			else if (_answers[i].isOn)
			{
				_answers[i].targetGraphic.color = Color.red;
				_currMCQ._results[i] = Color.red;
			}
			else
			{
				_currMCQ._results[i] = Color.white;
			}
		}
	}

	private void ClearState()
	{
		for (int i = 0; i < _answers.Length; i++)
		{
			_answers[i].targetGraphic.color = Color.white;
			_answers[i].isOn = false;
		}

		_comment.text = null;
		_buttonText.text = "Soumettre";
		_currMCQ = null;
	}

	/*==== PUBLIC METHODS ====*/

	/* take the MCQ, check if it can be used and set as the current one if it is*/
	public bool SetMCQ(MCQ mcq_)
	{
		if (mcq_ == null
			|| mcq_._rightAnswerNb == null 
			|| mcq_._answers.Count < 2 
			|| mcq_._answers.Count > 5)
			return false;


		/* disabling user input */
		_camera.enabled = false;
		_player.enabled = false;

		/* show the MCQ interface */
		gameObject.SetActive(true);

		/* get back state to answer question */
		_currMCQ = mcq_;

		/* we move and enable toggles depending on the number of answer the question has.
		 * there is min 2 answer and max 5, hence the following bounds. */
		for (int i = 2; i <  mcq_._answers.Count; i++)
        {
			_answersRect[i].gameObject.SetActive(true);
		}
		for (int i = mcq_._answers.Count; i < 5; i++)
		{
			_answersRect[i].gameObject.SetActive(false);
		}

		_stringBuilder.Append(_currMCQ._question); _stringBuilder.AppendLine();

		for (int i = 0; i < _currMCQ._answers.Count; i++)
        {
			_stringBuilder.Append((char)(i + 'A')); _stringBuilder.Append(" - ");
			_stringBuilder.Append(_currMCQ._answers[i]); _stringBuilder.AppendLine();
		}

		_question.text = _stringBuilder.ToString();
		_stringBuilder.Clear();

		/* clearing previous state */
		for (int i = 0; i < _rightAnswerCache.Length; i++)
		{
			_rightAnswerCache[i] = false;
		}
		/* filling the right answer to true, made to comapre with the toggles 
		 * (as we use it as an array of bool corresponding to the player's answer) */
		for (int i = 0; i < _currMCQ._rightAnswerNb.Count; i++)
		{
			_rightAnswerCache[_currMCQ._rightAnswerNb[i]] = true;
		}

		VideoController.Instance._pauseButton.SetActive(false);

		return true;
	}

	public bool SetMCQAndRotate(MCQ mcq_, Quaternion rot)
	{
		bool worked = SetMCQ(mcq_);
		if (worked)
		{
			_anim.RotateToTarget(rot);
		}
		return worked;
	}

	/* used to reset all other toggle to alse than the one that have been clicked on */
	public void SetToggle(Toggle activated)
	{
		if (activated.isOn && _currMCQ._singleAnswer)
		{
			for (int i = 0; i < _answers.Length; i++)
			{
				if (_answers[i] == activated)
					continue;

				_answers[i].isOn = false;
			}
		}
	}

	/* called every time the submit button is pressed */
	public void OnSubmit()
	{
		if (!_currMCQ._answered)
		{
			/* check if the question has been answered, 
			 * and make the points go up accordingly */
			if (_currMCQ._answered = CheckAnswer())
			{
				/* show the answer and save the result 
				 * to show the user at the end summary */
				ShowAnswer();
				_comment.text = _currMCQ._comment;
				_buttonText.text = "Continuer";
			}
			return;
		}

		ClearState();
		gameObject.SetActive(false);

		/* reenabling user input */
		_camera.enabled = true;
		_player.enabled = true;

		VideoController.Instance._pauseButton.SetActive(true);

		_OnSubmitEvent?.Invoke();
	}
}
