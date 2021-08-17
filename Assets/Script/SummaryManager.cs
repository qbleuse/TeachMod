using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

/* will handle the snap scroll of the summaries at the end of the game */
public class SummaryManager : MonoBehaviour
{
	/*==== SETTINGS ====*/
	/* prefabs of the summary contents */
	[SerializeField]					private SummaryContainer	_summaryGo		= null;
	/* parent transform of the instanciated Summary Container*/
	[SerializeField]					private RectTransform		_scroller		= null;
	[SerializeField,Min(0.0f)]	private float						_verticalOffset = 0.0f;

	/*==== COMPONENTS ====*/
	public List<SummaryContainer>               _summaries		= new List<SummaryContainer>();
	private StringBuilder						_stringBuilder	= null;

	public void Init()
	{
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

		yield break;
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
