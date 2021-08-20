using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

/* will handle the snap scroll of the summaries at the end of the game */
public class SummaryManager : MonoBehaviour
{
	/*==== SETTINGS ====*/
	/* prefabs of the summary contents */
	[SerializeField]			private SummaryContainer	_summaryGo		= null;
	/* parent transform of the instanciated Summary Container*/
	[SerializeField]			private RectTransform		_scroller		= null;
	[SerializeField]			private RectTransform		_middleTrs		= null;
	[SerializeField]			private RectTransform		_buttonTrs		= null;
	[SerializeField,Min(0.0f)]	private float				_verticalOffset = 0.0f;
	[SerializeField,Min(0.1f)]	private float				_lerpDuration	= 1.0f;

	/*==== COMPONENTS ====*/
	public List<SummaryContainer>	_summaries		= new List<SummaryContainer>();
	private StringBuilder			_stringBuilder	= null;

	/*==== STATE ====*/
	private AnimationCam _cam			= null;

	private int			_currFocus		= 0;
	private Coroutine	_focusCoroutine = null;
	private float		_prevScrollerY	= 0.0f;

	public void Init()
	{
		_cam = Camera.main.GetComponent<AnimationCam>();

		/* getting the list of the pois */
		List<MCQ> mcq = POI_Manager.Instance._mcqs;
		List<POI> poi = POI_Manager.Instance._pois;

		_stringBuilder = new StringBuilder();

		for (int i = 0; i < poi.Count;i++)
		{
			if (poi[i]._mcq)
			{
				SummaryContainer newContainer = Instantiate(_summaryGo, _scroller);
				newContainer.Init(poi[i]);
				mcq.Remove(poi[i]._mcq);
				FillContent(newContainer, poi[i]._mcq);
				_summaries.Add(newContainer);
			}
		}

		

		for (int i = 0; i < mcq.Count; i++)
		{
			SummaryContainer newContainer = Instantiate(_summaryGo, _scroller);
			newContainer.Init(mcq[i]);
			FillContent(newContainer, mcq[i]);
			_summaries.Add(newContainer);
		}

		
		
		StartCoroutine(PlaceSummaries());
	}

	public void OnBeginDrag()
	{
		if (_focusCoroutine != null)
			StopCoroutine(_focusCoroutine);
		/* saving previous y */
		_prevScrollerY = _scroller.anchoredPosition.y;
	}

	public void OnEndDrag()
	{
		/* we find the closest to focus on, we allow to move only one at a time */
		int nextPossibleFocus;
		/* we look if the next possible focus is before or after the current index */
		if (_prevScrollerY < _scroller.anchoredPosition.y)
			nextPossibleFocus = _currFocus + 1;
		else
			nextPossibleFocus = _currFocus - 1;

		if (nextPossibleFocus < 0 || nextPossibleFocus >= _summaries.Count)
		{
			_focusCoroutine = StartCoroutine(FocusOnSummary());
			return;
		}

		if (	Mathf.Abs(_summaries[nextPossibleFocus].transform.position.y - _middleTrs.transform.position.y) 
			<	Mathf.Abs(_summaries[_currFocus].transform.position.y - _middleTrs.transform.position.y))
		{
			_summaries[_currFocus].ShutOff();
			_currFocus = nextPossibleFocus;
		}

		_focusCoroutine = StartCoroutine(FocusOnSummary());
	}

	public void FocusNext()
	{
		_currFocus++;
		if (_currFocus >= _summaries.Count)
		{
			_currFocus--;
			return;
		}

		_summaries[_currFocus - 1].ShutOff();

		if (_focusCoroutine != null) StopCoroutine(_focusCoroutine);
		_focusCoroutine = StartCoroutine(FocusOnSummary());
	}

	public void FocusPrev()
	{
		_currFocus--;
		if (_currFocus < 0)
		{
			_currFocus++;
			return;
		}

		_summaries[_currFocus + 1].ShutOff();

		if (_focusCoroutine != null) StopCoroutine(_focusCoroutine);
		_focusCoroutine = StartCoroutine(FocusOnSummary());
	}



	private void FillContent(SummaryContainer sumContainer_, MCQ mcq_)
	{
		/* make the string empty */
		sumContainer_.content.text = "";

		_stringBuilder.Append(mcq_._question); _stringBuilder.AppendLine(); _stringBuilder.AppendLine();

		for (int j = 0; j < mcq_._answers.Count; j++)
		{
			/* colored answer to show the result */
			if (mcq_._results != null)
			{
				_stringBuilder.Append("<color=#"); _stringBuilder.Append(ColorUtility.ToHtmlStringRGB(mcq_._results[j])); _stringBuilder.Append(">");
				_stringBuilder.Append((char)(j + 'A')); _stringBuilder.Append("</color>");
			}
			else
			{
				_stringBuilder.Append((char)(j + 'A'));
			}

			_stringBuilder.Append(" - "); _stringBuilder.Append(mcq_._answers[j]); _stringBuilder.AppendLine();
		}

		_stringBuilder.AppendLine();

		if (mcq_._comment.Length > 0)
		{
			_stringBuilder.Append(mcq_._comment);
			_stringBuilder.AppendLine(); _stringBuilder.AppendLine();
		}

		sumContainer_.content.text = _stringBuilder.ToString();
		_stringBuilder.Clear();
	}

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{

	}

	IEnumerator PlaceSummaries()
	{
		yield return new WaitForFixedUpdate();

		_summaries.Sort();

		/*represent the height of the whole screen */
		float height = _scroller.rect.height;

		float x = _summaries[0].content.rectTransform.anchoredPosition.x;
		float y = 0;

		for (int i = 0; i < _summaries.Count; i++)
		{

			y -= (height - _summaries[i].content.rectTransform.rect.height) / 2.0f;
			_summaries[i].content.rectTransform.anchoredPosition = new Vector2(x, y);
			y = (-(i+1)*height) + (i+1) * _verticalOffset;
		}

		height = (height - _summaries[_summaries.Count - 1].content.rectTransform.rect.height) / 2.0f;
		_buttonTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height/2.0f);
		_buttonTrs.anchoredPosition = new Vector2(_buttonTrs.anchoredPosition.x, y + height * 0.75f);

		yield break;
	}

	IEnumerator FocusOnSummary()
	{
		_summaries[_currFocus].Set(_cam);

		Vector2 newPos = Vector2.zero;
		float prevY = _scroller.anchoredPosition.y;
		float targetY = -((-(_currFocus) * _scroller.rect.height) + (_currFocus) * _verticalOffset);

		float elapsedTime = 0.0f;

		while(elapsedTime < _lerpDuration)
		{
			newPos.y = Mathf.Lerp(prevY, targetY, elapsedTime/_lerpDuration);
			_scroller.anchoredPosition = newPos;
			elapsedTime += Time.unscaledDeltaTime;
			yield return null;
		}

		newPos.y = targetY;
		_scroller.anchoredPosition = newPos;

		yield break;
	}
}
