using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
		Instance = this;
    }

	/* you may want to look at the prefabs associated with this to understand what is being done here */
	void Start()
    {
		/* get the canvas */
		_canvas = transform.GetChild(0).gameObject;

        /* we need to move them depending on the number of question asked */
        _answers = GetComponentsInChildren<Toggle>();

        _answersRect = new RectTransform[4];

        for (int i = 0; i < _answers.Length; i++)
        {
            _answersRect[i] = _answers[i].GetComponent<RectTransform>();
        }

		/* get the width of the canvas to move the toggle afterward */
		//_canvasTrs = GetComponent<RectTransform>();

		/* get the text to change the question */
		_question = _canvas.transform.GetChild(1).GetComponent<Text>();

		_buttonText = _canvas.transform.GetChild(6).GetComponent<Text>();

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
					_answers[_currMCQ._rightAnswerNb].graphic.material.SetColor("Tint", Color.red);
				}
            }
        }
		else
        {
			EndMenu.Instance._MCQ_Score++;
        }

		_answers[_currMCQ._rightAnswerNb].graphic.material.SetColor("Tint", Color.green);
    }

	private void ClearState()
    {
		for (int i = 0; i < _answers.Length; i++)
		{
			_answers[_currMCQ._rightAnswerNb].graphic.material.SetColor("Tint", Color.white);
		}

		_buttonText.text = "Submit";
		_currMCQ = null;
	}


    public bool SetMCQ(MCQ mcq_)
    {
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
				return false;
		}

		_question.text = _currMCQ._question;
		return true;
	}

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
		gameObject.SetActive(false);
	}
}
