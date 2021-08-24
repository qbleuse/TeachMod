using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndMenu : MonoBehaviour
{
	/* an enum to store the state of the EndMenu */
	private enum State
	{
		QUESTION,
		SUMMARY,
		SCORE,

		NB
	}

	/*==== SINGLETON ====*/
	public static EndMenu Instance = null;

	/*==== STATE ====*/
	/* we start with the question, it usually corresponds more to the next state 
	 * we will be in rather than the current one. Look at ChangeState for more precision */
	private State _menuState = State.QUESTION;

	/* the nb in the array of the POI question that is asked right now. */
	private int		_currQuestNb	= 0;
	private MCQ		_currMCQ		= null;

	/* the score gained when finding the POI 
	 * and having interpreted it the way it shoud */
	public int _POI_Score = 0;

	/* the score gained when answering good to the MCQ */
	public int _MCQ_Score = 0;

	/*==== COMPONENTS ====*/
	private GameObject		_summarySection		= null;
	private SummaryManager	_summaryManager		= null;

	private GameObject	_buttonGo			= null;

	private GameObject			_scoreSection	= null;
	private TextMeshProUGUI[]	_scoreText		= null;


	/*==== STARTUP METHODS ====*/
	private void Awake()
	{
		Instance = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		/* getting the gameobjects we need in the summary section,
		 * you may want to open the EndMenu prefab to see how it is stored */
		_summarySection = transform.GetChild(0).transform.GetChild(1).gameObject;
		_summaryManager	= GetComponent<SummaryManager>();

		/* we will make appear after */
		_summarySection.SetActive(false);

		/* getting the gameobject and the text of the button to change it after */
		_buttonGo	= transform.GetChild(0).transform.GetChild(0).gameObject;
		_buttonGo.SetActive(false);

		/* get the score section to print score at te end */
		_scoreSection	= transform.GetChild(0).transform.GetChild(2).gameObject;
		_scoreText		= _scoreSection.GetComponentsInChildren<TextMeshProUGUI>();
		_scoreSection.SetActive(false);


		/* set EndMenu Inactive for the moment */
		gameObject.SetActive(false);
	}

	/* typically used to enable the EndMenu and setup needed when video has stoped */
	public void WakeUp()
	{
		POI_Manager.Instance.gameObject.SetActive(false);
		VideoController.Instance._framePerFrame = true;

		gameObject.SetActive(true);
		MCQ_Manager.Instance._OnSubmitEvent -= VideoController.Instance.PauseAndResume;
		MCQ_Manager.Instance._OnSubmitEvent += SetQuestion;

		//if (Input.GetKeyDown(KeyCode.E))
		//{
		//	_menuState++;
		//}

		ChangeState();
	}


	/*==== RUNTIME METHODS ====*/
	// Update is called once per frame
	void Update()
	{
	}

	/* a method called by the _button to change what is showed by the endMenu */
	public void ChangeState()
	{
		switch(_menuState)
		{
			case State.QUESTION:
				CleanPOI();
				SetQuestion();
				break;
			case State.SUMMARY:
				Camera.main.GetComponent<Player>().enabled = false;
				_summaryManager.Init();
				MCQ_Manager.Instance.gameObject.SetActive(false);
				_summarySection.SetActive(true);
				_menuState++;
				break;
			case State.SCORE:
				_buttonGo.SetActive(true);
				_summarySection.SetActive(false);
				_scoreSection.SetActive(true);
				SetScore();
				_menuState++;
				break;
			default:
				LevelManager.Instance.LoadLevel(0);
				break;
		}
	}

	private void CleanPOI()
    {
		List<POI> pois = POI_Manager.Instance._pois;
		for (int i = 0; i < pois.Count; i++)
        {
			if (pois[i]._mcq && pois[i]._askOnHit && !pois[i]._mcq._answered)
			{
				pois[i]._mcq._answered = true;
				pois[i]._mcq._results = new Color[pois[i]._mcq._answers.Count];
				for (int j = 0; j < pois[i]._mcq._answers.Count; j++)
				{
					pois[i]._mcq._results[j] = Color.white;
				}
				for (int j = 0; j < pois[i]._mcq._rightAnswerNb.Count; j++)
                {
					pois[i]._mcq._results[pois[i]._mcq._rightAnswerNb[j]] = Color.yellow;
				}
			}

			pois[i].gameObject.SetActive(false);

		}
    }

	/* helper to setup the question panel depending on the nb o question */
	private void SetQuestion()
	{
		int mcqCount	= POI_Manager.Instance._mcqs.Count;

Search:
		/* look for a POI that has a question */
		while (_currQuestNb < mcqCount && (_currMCQ == null || _currMCQ._answered))
		{
			_currMCQ = POI_Manager.Instance._mcqs[_currQuestNb];
			_currQuestNb++;
		}

		/* this methods is called multiple times, 
		 * every time we want to print the MCQ of a POI
		 * so this is possible/intended */
		if (_currQuestNb >= mcqCount)
		{
			_menuState++;
			ChangeState();
			return;
		}

		/* we found one that can be ok, we check if it is applicable,
		 if it is not we go to search for one that works */
		if (!MCQ_Manager.Instance.SetMCQ(_currMCQ))
		{
			_currQuestNb++;
			goto Search;
		}
	}

	/* method that append the score to the string of the text score "_scoreText"*/
	void SetScore()
	{
		/* append POI found/POI there was */
		_scoreText[0].text += _POI_Score.ToString() + "/" + POI_Manager.Instance._pois.Count.ToString();

		/* append MCQ well answered/MCQ there was (there should be as much POI than there is MCQ as they're contained in th POI) */
		_scoreText[1].text += _MCQ_Score.ToString() + "/" + POI_Manager.Instance._mcqs.Count.ToString();
	}

}
