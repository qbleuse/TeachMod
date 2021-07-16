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



	public event Action OnSubmitEvent;

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

        _answersRect = new RectTransform[4];

        for (int i = 0; i < _answers.Length; i++)
        {
            _answersRect[i] = _answers[i].GetComponent<RectTransform>();
        }

		OnSubmitEvent = VideoController.Instance.PauseAndResume;

		/* get the text to change the question */
		_question = _canvas.transform.GetChild(1).GetComponent<Text>();

		_buttonText = _canvas.transform.GetChild(6).transform.GetChild(0).GetComponent<Text>();

		/* don't need it right now */
		gameObject.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
    }

	private void CheckAnswer()
    {
		if (!_answers[_currMCQ._rightAnswerNb].isOn)
        {
			for (int i = 0; i < _answers.Length; i++)
            {
				if (_answers[i].isOn)
                {
					_answers[i].targetGraphic.color = Color.red;
				}
            }
        }
		else
        {
			EndMenu.Instance._MCQ_Score++;
        }

		_answers[_currMCQ._rightAnswerNb].targetGraphic.color = Color.green;
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
		/* disabling user input */
		_camera.enabled = false;
		_player.enabled = false;

		/* show the MCQ interface */
		gameObject.SetActive(true);

		/* get back state to answer question */
		_currMCQ = mcq_;

		/* we move the toggle depending on the number of answer the question has.
		 * we could theoratically do an infinite number but realistically, such question will not appear.
		 * so we will stick with the good old 4 MCQ, and we do it by hand (it will be faster this way).*/
		switch (_currMCQ._answerNb)
		{
			case 2:
				/* no need for C and D so disable them */
				_answersRect[2].gameObject.SetActive(false);
				_answersRect[3].gameObject.SetActive(false);
				break;
			case 3:
				/* need for C, not D */
				_answersRect[2].gameObject.SetActive(true);
				_answersRect[3].gameObject.SetActive(false);

				break;
			case 4:
				/* need for C and D */
				_answersRect[2].gameObject.SetActive(true);
				_answersRect[3].gameObject.SetActive(true);

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
		if (activated.isOn)
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
		if (!_currMCQ.answered)
		{
			CheckAnswer();
			_currMCQ.answered	= true;
			_buttonText.text	= "Continue";
			return;
		}

		ClearState();
		OnSubmitEvent?.Invoke();
		gameObject.SetActive(false);

		/* reenabling user input */
		_camera.enabled = true;
		_player.enabled = true;
	}
}
