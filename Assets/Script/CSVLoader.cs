using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class CSVLoader
{
	/*==== COMPONENT ====*/
	[HideInInspector] public List<string> _lines = new List<string>();


	/*==== STATE ====*/
	private char _sepChar = CultureInfo.InvariantCulture.TextInfo.ListSeparator[0];
	private NumberStyles _style  = NumberStyles.Float;
	private CultureInfo _culture;

	/* method loading the lines of the csv file in the lines component */
	public void LoadFile(string filePath_)
	{
		_lines.Clear();
		_culture	= CultureInfo.CurrentCulture;

		using (FileStream fStream = new FileStream(Application.dataPath + '/' + filePath_, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		using (StreamReader reader = new StreamReader(fStream))
		{

			GetReadInfo(reader);

			StringBuilder strBuilder = new StringBuilder();
			string line = null;
			string multLineSep = '"'.ToString();
			bool mulLineText = false;
			bool hasMulSep = false;

			while (!reader.EndOfStream)
			{
				line = reader.ReadLine();
				hasMulSep = line.Contains(multLineSep);


				/* beginning of a multiline text */
				if (hasMulSep && !mulLineText)
				{
					mulLineText = true;
					strBuilder.Append(line);
				}/* in a multi line text */
				else if (!hasMulSep && mulLineText)
				{
					strBuilder.Append("\n");
					strBuilder.Append(line);
				}/* the end of a multiline text */
				else if (hasMulSep && mulLineText)
				{
					strBuilder.Append("\n");
					strBuilder.Append(line);
					_lines.Add(strBuilder.ToString());
					strBuilder.Clear();
					mulLineText = false;
				}
				else/* a normal other line, without a multiline text */
				{
					_lines.Add(line);
				}
			}
		}
	}

	public void GetReadInfo(StreamReader reader)
	{
		/* chooses between the french and the english way of parsing float 
		 * depending on the separating value*/
		string description = null;

		description = reader.ReadLine();

		/* tecnically it is more about choosing or not the european way of writing 
		 * floating point numbers but we choose french as placeholder */
		CultureInfo fr = CultureInfo.CreateSpecificCulture("fr-FR");
		if (description.Contains(fr.TextInfo.ListSeparator))
		{
			_sepChar = fr.TextInfo.ListSeparator[0];
			_culture = CultureInfo.CreateSpecificCulture("fr-FR");
		}
		else
		{
			_culture = CultureInfo.InvariantCulture;
		}
	}


	/* create and fill the serializer's array of POI. 
	 * Be careful, if you have a reference on a mcq in your pois,
	 * We use the array that is in the serializer to get it.
	 * So populate your mcq before and do not sort them. */
	public void PopulatePOI(CSVSerializer serializer_)
	{
		if (_lines.Count <= 0)
			return;

		float yaw = 0;
		float pitch = 0;
		float size = 0.0f;

		serializer_._pois = new List<POI>(_lines.Count - 1);

		for (int i = 0; i < _lines.Count; i++)
		{
			string[] values = _lines[i].Split(_sepChar);

			POI newPoi = serializer_.InstantiatePOI(i);

			if (newPoi == null)
				continue;

			/* get timestamp and sequence */
			int.TryParse(values[0], out newPoi._sequence);
			newPoi._sequence -= 1;
			float.TryParse(values[1], _style, _culture, out newPoi._timestamp);
			float.TryParse(values[2], _style, _culture, out newPoi._endTimestamp);

			/* get behavior info */
			bool.TryParse(values[3], out newPoi._askOnHit);

			/* get transform info */
			float.TryParse(values[4], _style, _culture, out yaw);
			float.TryParse(values[5], _style, _culture, out pitch);
			float.TryParse(values[6], _style, _culture, out size);

			newPoi.transform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
			newPoi.transform.localScale = new Vector3(size, size, 1.0f);

			int mcqId = -1;
			/* get mcq ref if have one */
			if (int.TryParse(values[7], out mcqId) && (mcqId - 2) < serializer_._mcqs.Count)
				newPoi._mcq = serializer_._mcqs[mcqId - 2];
		}
	}

	/* create and fill the serializer's array of MCQ. */
	public void PopulateMCQ(CSVSerializer serializer_)
	{
		if (_lines.Count <= 0)
			return;

		serializer_._mcqs = new List<MCQ>(_lines.Count - 1);

		for (int i = 0; i < _lines.Count; i++)
		{
			string[] values = _lines[i].Split(_sepChar);

			MCQ newMCQ = null;

			/* we consider that there is no MCQ is there is no question */
			if (values[0].Length > 1)
			{
				newMCQ = ScriptableObject.CreateInstance<MCQ>();

				/* get the question in UTF8 */
				newMCQ._question = values[0];

				/* removing the "" */
				newMCQ._question = newMCQ._question.Substring(1, newMCQ._question.Length - 2);

				/* get the nb of answer, and if it is a 
				 * multiple or single choice questionnaire */
				int.TryParse(values[1], out newMCQ._answerNb);
				bool.TryParse(values[2], out newMCQ._singleAnswer);

				/* get the right answer, no need to do hard thing 
				 * if there is a single answer */
				if (newMCQ._singleAnswer)
				{
					/* handle lower case and upper case */
					values[3].ToUpper();
					newMCQ._rightAnswerNb = new List<int>(1);
					newMCQ._rightAnswerNb.Add(values[3][0] - 'A');
				}
				else
				{
					/* each answer will be split by a forward slash */
					string[] answers = values[3].Split('/');
					newMCQ._rightAnswerNb = new List<int>(answers.Length);

					/* now it should be only a character in those string so we recover it */
					for (int j = 0; j < answers.Length; j++)
					{
						answers[j].ToUpper();
						newMCQ._rightAnswerNb.Add(answers[j][0] - 'A');
					}
				}

				/* pause info */
				bool.TryParse(values[4], out newMCQ._pause);

				if (newMCQ._pause)
				{
					int.TryParse(values[5], out newMCQ._sequence);
					newMCQ._sequence -= 1;
					float.TryParse(values[6], _style, _culture, out newMCQ._timestamp);
				}

				newMCQ._serialID = i;
			}

			serializer_._mcqs.Insert(i, newMCQ);
		}
	}
}
