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
	private State menuState = State.QUESTION;

	/*==== COMPONENTS ====*/
	private GameObject	_commentSection		= null;
	private Text		_commentsText		= null;

	private GameObject	_buttonGo			= null;
	private Text		_buttonText			= null;


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

		//_commentsText.text =
		/* getting the gameobject and the text of the button to change it after */
		_buttonGo	= transform.GetChild(0).transform.GetChild(1).gameObject;
		_buttonText = _buttonGo.GetComponentInChildren<Text>();

		_buttonText.text = "Next";

		/* set Inactive for the moment */
		gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		
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
			_commentsText.text += poisRef[i]._comments;
			_commentsText.text += "\n\n";
        }
    }

	/* a method called by the _button to change what is showed by the endMenu */
	public void ChangeState()
	{
		menuState++;

		switch(menuState)
        {
			case State.COMMENT:
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
}
