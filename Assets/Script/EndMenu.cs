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

	/*==== STATE ====*/
	/* we start with the question, it usually corresponds more to the next state 
	 * we will be in rather than the current one. Look at ChangeState for more precision */
	private State menuState = State.QUESTION;

	/* the nb in the array of the POI question that is asked right now. */
	private int		_currQuestNb	= 0;
	private MCQ		_currMCQ		= null;

	/* the score gained when finding the POI 
	 * and having interpreted it the way it shoud */
	public int _POI_Score = 0;

	/* the score gained when answering good to the MCQ */
	public int _MCQ_Score = 0;

	/*==== COMPONENTS ====*/
	private GameObject	_commentSection		= null;
	private Text		_commentsText		= null;

	private GameObject	_buttonGo			= null;
	private Text		_buttonText			= null;

	private GameObject	_scoreSection	= null;
	private Text[]		_scoreText		= null;


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
		_buttonGo.SetActive(false);

		/* get the score section to print score at te end */
		_scoreSection	= transform.GetChild(0).transform.GetChild(3).gameObject;
		_scoreText		= _scoreSection.GetComponentsInChildren<Text>();
		_scoreSection.SetActive(false);


		/* set EndMenu Inactive for the moment */
		gameObject.SetActive(false);
	}

	/* typically used to enable the EndMenu and setup needed when video has stoped */
	public void WakeUp()
	{
		gameObject.SetActive(true);
		MCQ_Manager.Instance._OnSubmitEvent -= VideoController.Instance.PauseAndResume;
		MCQ_Manager.Instance._OnSubmitEvent += SetQuestion;
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
		switch(menuState)
		{
			case State.QUESTION:
				SetQuestion();
				break;
			case State.COMMENT:
				_buttonGo.SetActive(true);
				MCQ_Manager.Instance.gameObject.SetActive(false);
				_commentSection.SetActive(true);
				menuState++;
				break;
			case State.SCORE:
				_commentSection.SetActive(false);
				_scoreSection.SetActive(true);
				SetScore();
				_buttonText.text = "Quit";
				menuState++;
				break;
			default:
				LevelManager.Instance.LoadLevel(0);
				break;
		}
	}

	/* helper to setup the question panel depending on the nb o question */
	private void SetQuestion()
	{
		int poiCount	= POI_Manager.Instance._pois.Count;

Search:
		/* this methods is called multiple times, 
		 * every time we want to print the MCQ of a POI
		 * so this is possible/intended */
		if (_currQuestNb >= poiCount)
		{
			menuState++;
			ChangeState();
		}

		/* look for a POI that has a question */
		while (_currQuestNb < poiCount && (_currMCQ == null || _currMCQ.answered))
		{
			_currMCQ = POI_Manager.Instance._pois[_currQuestNb]._question;
			_currQuestNb++;
		}

		/* we found one that can be ok, we check if it is applicable,
		 if it is not we go to search for one that works */
		if (!MCQ_Manager.Instance.SetMCQ(_currMCQ))
			goto Search;
	}

	/* method that append the score to the string of the text score "_scoreText"*/
	void SetScore()
    {
		int poiCount = POI_Manager.Instance._pois.Count;
		/* append POI found/POI there was */
		_scoreText[0].text += _POI_Score.ToString() + "/" + poiCount.ToString();

		/* append MCQ well answered/MCQ there was (there should be as much POI than there is MCQ as they're contained in th POI) */
		_scoreText[1].text += _MCQ_Score.ToString() + "/" + (poiCount + POI_Manager.Instance._pausePois.Count).ToString();
	}

}
