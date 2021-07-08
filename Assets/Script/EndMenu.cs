using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMenu : MonoBehaviour
{
	/* an enum to store the state of the EndMenu */
	private enum State
	{
		QUESTION,
		COMMENT,
		SCORE,

		NB
	}

	/*==== SINGLETON ====*/
	public static EndMenu Instance = null;

	/*==== SETTINGS ====*/
	[SerializeField, Range(0.0f, 0.5f)] private float togglePlacement = 0.25f;

	/*==== STATE ====*/
	/* we start with the question, it usually corresponds more to the next state 
	 * we will be in rather than the current one. Look at ChangeState for more precision */
	private State menuState = State.QUESTION;

	/* the nb in the array of the POI question that is asked right now. */
	private int		_currQuestNb	= 0;
	private MCQ		_currMCQ		= null;
	private float	_canvasWidth	= 0.0f;

	/*==== COMPONENTS ====*/
	private GameObject	_commentSection		= null;
	private Text		_commentsText		= null;

	private GameObject	_buttonGo			= null;
	private Text		_buttonText			= null;

	private GameObject		_questionPanel	= null;
	private Toggle[]		_answers		= null;
	private RectTransform[] _answersRect	= null;
	private Text			_question		= null;


	/*==== STARTUP METHODS ====*/
	private void Awake()
	{
		Instance = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		/* getting the gameobjects we need in the comment section,
		 * you may want to open the EndMenu prefab to see how it is stored */
		_commentSection = transform.GetChild(0).transform.GetChild(2).gameObject;
		_commentsText	= _commentSection.GetComponentInChildren<Text>();

		FillComment();

		/* we will make appear after */
		_commentSection.SetActive(false);

		/* getting the gameobject and the text of the button to change it after */
		_buttonGo	= transform.GetChild(0).transform.GetChild(1).gameObject;
		_buttonText = _buttonGo.GetComponentInChildren<Text>();

		_buttonText.text = "Next";

		/* getting the gameobject of the question panel, to disbale it after we answered all of them */
		_questionPanel	= transform.GetChild(0).transform.GetChild(3).gameObject;
		/* we need to move them depending on the nulmber of question asked */
		_answers		= _questionPanel.GetComponentsInChildren<Toggle>();

		_answersRect = new RectTransform[4];

		for (int i = 0; i<_answers.Length;i++)
        {
			_answersRect[i] = _answers[i].GetComponent<RectTransform>();
        }

		/* get the width of the canvas to move the toggle afterward */
		_canvasWidth	= _questionPanel.GetComponent<RectTransform>().rect.width;
		/* get the text to change the question */
		_question		= _questionPanel.transform.GetChild(0).GetComponent<Text>();

		/* set EndMenu Inactive for the moment */
		gameObject.SetActive(false);
	}

	/* typically used to enable the EndMenu and setup needed when video has stoped */
	public void WakeUp()
	{
		gameObject.SetActive(true);
		SetQuestion();
	}

	/* a method called at startup to fill the _commentsText with the comments of the POIs */
	private void FillComment()
	{
		/* getting the list of the pois */
		List<POI> poisRef = POI_Manager.Instance._pois;

		/* make the string empty */
		_commentsText.text = "";

		for (int i = 0; i < poisRef.Count; i++)
		{
			if (poisRef[i]._comments.Length > 0)
			{
				_commentsText.text += poisRef[i]._comments;
				_commentsText.text += "\n\n";
			}
		}

		/* extending height of the text in order to show all the comments */
		int lineNb = 0;

		for (int i = 0; i < _commentsText.text.Length; i++)
		{
			if (_commentsText.text[i] == '\n')
			{
				lineNb++;
			}
		}

		/* should be at least to the size of the scroll container (the one in the prefabs) */
		float rectHeight = Mathf.Max((float)lineNb * _commentsText.fontSize + (float)lineNb * _commentsText.lineSpacing, _commentsText.rectTransform.rect.height);

		/* changing height and y pos in order to be at the exact height we need for all the line be covered,
		 * and puting the top of the text in the top of the scroll rectangle */
		_commentsText.rectTransform.rect.Set(_commentsText.rectTransform.rect.x,
											-(rectHeight - _commentsText.rectTransform.rect.height) / 2.0f,
											_commentsText.rectTransform.rect.width,
											rectHeight);
	}


	/*==== RUNTIME METHODS ====*/
	// Update is called once per frame
	void Update()
	{
		
	}

	/* a method called by the _button to change what is showed by the endMenu */
	public void ChangeState()
	{
		menuState++;
		switch (menuState)
		{
			case State.QUESTION:
				ManageQuestion();
				
				break;
			case State.COMMENT:
				_questionPanel.SetActive(false);
				_commentSection.SetActive(true);
				break;
			case State.SCORE:
				_commentSection.SetActive(false);
				_buttonText.text = "Quit";
				break;
			default:
				LevelManager.Instance.LoadLevel(0);
				break;
		}
	}

	private void ManageQuestion()
	{
		if (!_currMCQ.answered)
		{
			_currMCQ.answered = true;
			return;
		}

		SetQuestion();
	}

	/* helper to setup the question panel depending on the nb o question */
	private void SetQuestion()
	{
		int poiCount	= POI_Manager.Instance._pois.Count;
		_currMCQ		= POI_Manager.Instance._pois[_currQuestNb]._question;

		/* look for a POI that has a question */
		while (_currMCQ == null && _currQuestNb < poiCount)
		{
			_currQuestNb++;
			_currMCQ = POI_Manager.Instance._pois[_currQuestNb]._question;
		}

		/* we move the toggle depending on the number of answer the questi has.
		 * we could theoratically do an infinite number but realistically, such question will not appear.
		 * so we will stick with the good old 4 MCQ, and we do it by hand (it will be faster this way).*/
		switch (_currMCQ._answerNb)
		{
			case 2:
				/* no need for C and D so disable them */
				_answers[2].gameObject.SetActive(false);
				_answers[3].gameObject.SetActive(false);

				//Debug.LogError(-togglePlacement * _canvasWidth * 0.5f);

				_answersRect[0].rect.Set(-togglePlacement * _canvasWidth * 0.5f,
							_answers[0].graphic.rectTransform.rect.y,
							_answers[0].graphic.rectTransform.rect.width,
							_answers[0].graphic.rectTransform.rect.height);

				_answers[1].graphic.rectTransform.rect.Set(togglePlacement*_canvasWidth*0.5f,
							_answers[1].graphic.rectTransform.rect.y,
							_answers[1].graphic.rectTransform.rect.width,
							_answers[1].graphic.rectTransform.rect.height);
				break;
			case 3:
				/* need for C, not D */
				_answers[2].gameObject.SetActive(true);
				_answers[3].gameObject.SetActive(false);

				_answers[0].graphic.rectTransform.rect.Set(-togglePlacement * _canvasWidth * 0.5f,
							_answers[0].graphic.rectTransform.rect.y,
							_answers[0].graphic.rectTransform.rect.width,
							_answers[0].graphic.rectTransform.rect.height);

				_answers[1].graphic.rectTransform.rect.Set(togglePlacement * _canvasWidth * 0.5f,
							_answers[1].graphic.rectTransform.rect.y,
							_answers[1].graphic.rectTransform.rect.width,
							_answers[1].graphic.rectTransform.rect.height);

				_answers[2].graphic.rectTransform.rect.Set(0,
							_answers[2].graphic.rectTransform.rect.y,
							_answers[2].graphic.rectTransform.rect.width,
							_answers[2].graphic.rectTransform.rect.height);

				break;
			case 4:
				/* need for C and D */
				_answers[2].gameObject.SetActive(true);
				_answers[3].gameObject.SetActive(true);

				_answers[0].graphic.rectTransform.rect.Set(-togglePlacement * _canvasWidth * 0.5f,
							_answers[0].graphic.rectTransform.rect.y,
							_answers[0].graphic.rectTransform.rect.width,
							_answers[0].graphic.rectTransform.rect.height);

				_answers[1].graphic.rectTransform.rect.Set(togglePlacement * _canvasWidth * 0.5f,
							_answers[1].graphic.rectTransform.rect.y,
							_answers[1].graphic.rectTransform.rect.width,
							_answers[1].graphic.rectTransform.rect.height);

				_answers[2].graphic.rectTransform.rect.Set(-togglePlacement * _canvasWidth * 0.5f,
							_answers[2].graphic.rectTransform.rect.y,
							_answers[2].graphic.rectTransform.rect.width,
							_answers[2].graphic.rectTransform.rect.height);

				_answers[3].graphic.rectTransform.rect.Set(togglePlacement * _canvasWidth * 0.5f,
							_answers[3].graphic.rectTransform.rect.y,
							_answers[3].graphic.rectTransform.rect.width,
							_answers[3].graphic.rectTransform.rect.height);
				break;
			/* if a question does not corresponds, 
			 * we just ignore it and search an other that works*/
			default:
				if (_currQuestNb < poiCount)
				{
					_currQuestNb++;
					SetQuestion();
					return;
				}
				/* if there's no question that match, just leave */
				ChangeState();
				return;
		}

		_question.text = _currMCQ._question;
	}
}
