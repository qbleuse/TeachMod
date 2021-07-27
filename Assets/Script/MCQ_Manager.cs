using System;
using UnityEngine;
using UnityEngine.UI;

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
    private Text            _question       = null;
	private Text			_buttonText		= null;

	/*==== CACHE ====*/
	MonoBehaviour	_camera	= null;
	Player			_player = null;
	AnimationCam	_anim	= null;



	public event Action _OnSubmitEvent;

    private void Awake()
    {
		Instance = this;
    }

	/* you may want to look at the prefabs associated with this to understand what is being done here */
	void Start()
    {
		/* caching the user input script to disable them */
		Camera camera = Camera.main;
		_player = camera.GetComponent<Player>();
		_anim	= camera.GetComponent<AnimationCam>();

		if (SystemInfo.deviceType == DeviceType.Handheld)
			_camera = camera.GetComponent<GyroCam>();
		else
			_camera = camera.GetComponent<DesktopCam>();

		/* get the canvas */
		_canvas = transform.GetChild(0).gameObject;

        /* we need to move them depending on the number of question asked */
        _answers = GetComponentsInChildren<Toggle>();

		/* five is the most answer it can have (see Prefabs if needed)*/
        _answersRect = new RectTransform[5];

		/* used to precompute the answer */
		_rightAnswerCache = new bool[5];

        for (int i = 0; i < _answers.Length; i++)
        {
            _answersRect[i] = _answers[i].GetComponent<RectTransform>();
        }

		_OnSubmitEvent = VideoController.Instance.PauseAndResume;

		/* get the text to change the question */
		_question = _canvas.transform.GetChild(1).GetComponent<Text>();

		_buttonText = _canvas.transform.GetChild(7).transform.GetChild(0).GetComponent<Text>();

		/* don't need it right now */
		gameObject.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
		_answers[2].targetGraphic.material.color = Color.white;

	}

	/* check the answer of the player and return if it has been answered or not */
	private bool CheckAnswer()
    {
		bool answered		= false;
		_answeredRightOnce	= false;
		int answersRight = 0;

		/* the user can give only from an answer he has been given so depending on _answerNb */
		for (int i = 0; i < _currMCQ._answerNb; i++)
		{
			if (_answers[i].isOn && _rightAnswerCache[i])
		    {
				answered			= true;
				_answeredRightOnce	= true;
				answersRight++;
			}
			else if (_answers[i].isOn)
			{
				answered = true;
			}
		}

		if (!answered)
			return false;

		if (answersRight == _currMCQ._rightAnswerNb.Length)
        {
			EndMenu.Instance._MCQ_Score++;
        }

		return true;
    }

	private void ShowAnswer()
    {
		for (int i = 0; i < _currMCQ._answerNb; i++)
		{
			if (!_answers[i].isOn && _rightAnswerCache[i] && _answeredRightOnce)
			{
				_answers[i].targetGraphic.color = Color.yellow;
			}
			else if (_rightAnswerCache[i])
			{
				_answers[i].targetGraphic.color = Color.green;
			}
			else if (_answers[i].isOn)
			{
				_answers[i].targetGraphic.color = Color.red;
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

		_buttonText.text = "Submit";
		_currMCQ = null;
	}


    public bool SetMCQ(MCQ mcq_)
    {
		if (mcq_._rightAnswerNb == null)
			return false;


		/* disabling user input */
		_camera.enabled = false;
		_player.enabled = false;

		/* show the MCQ interface */
		gameObject.SetActive(true);

		/* get back state to answer question */
		_currMCQ = mcq_;

		/* we move the toggle depending on the number of answer the question has.
		 * we could theoratically do an infinite number but realistically, such question will not appear.
		 * 5 has been chosen to be the most we'll get, and we do it by hand (it will be faster this way).*/
		switch (_currMCQ._answerNb)
		{
			case 2:
				/* no need for C, D and E so disable them */
				_answersRect[2].gameObject.SetActive(false);
				_answersRect[3].gameObject.SetActive(false);
				_answersRect[4].gameObject.SetActive(false);
				break;
			case 3:
				/* need for C, not D and E */
				_answersRect[2].gameObject.SetActive(true);
				_answersRect[3].gameObject.SetActive(false);
				_answersRect[4].gameObject.SetActive(false);

				break;
			case 4:
				/* need for C and D, not E*/
				_answersRect[2].gameObject.SetActive(true);
				_answersRect[3].gameObject.SetActive(true);
				_answersRect[4].gameObject.SetActive(false);

				break;
			case 5:
				/* need for C and D */
				_answersRect[2].gameObject.SetActive(true);
				_answersRect[3].gameObject.SetActive(true);
				_answersRect[4].gameObject.SetActive(true);

				break;
			/* if a question does not corresponds, 
			 * we just ignore it and search an other that works*/
			default:
				/* reenabling user input */
				_camera.enabled = true;
				_player.enabled = true;

				/* hiding interface */
				gameObject.SetActive(false);
				return false;
		}

		_question.text = _currMCQ._question;

		/* clearing previous state */
		for (int i = 0; i < _rightAnswerCache.Length; i++)
        {
			_rightAnswerCache[i] = false;
        }
		/* filling the right answer to true */
		for (int i = 0; i < _currMCQ._rightAnswerNb.Length; i++)
        {
			_rightAnswerCache[_currMCQ._rightAnswerNb[i]] = true;
        }
		

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

	public void OnSubmit()
    {
		if (!_currMCQ._answered)
		{
			/* check if the question has been answered, 
			 * and make the point go up accordingly */
			if (_currMCQ._answered = CheckAnswer())
            {
				ShowAnswer();
				_buttonText.text = "Continue";
			}
			return;
		}

		ClearState();
		_OnSubmitEvent?.Invoke();
		gameObject.SetActive(false);

		/* reenabling user input */
		_camera.enabled = true;
		_player.enabled = true;
	}
}
