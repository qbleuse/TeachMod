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
	private char _sepChar = ',';
	private NumberStyles _style;
	private CultureInfo _culture;

	/* method loading the lines of the csv file in the lines component */
	public void LoadFile(string filePath_)
	{
		_lines.Clear();
		_style = NumberStyles.AllowDecimalPoint | NumberStyles.Integer;


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


		if (description.Contains(";"))
		{
			_sepChar = ';';
			_culture = CultureInfo.CreateSpecificCulture("fr-FR");
		}
		else
		{
			_culture = CultureInfo.CreateSpecificCulture("en-EN");
		}
	}


	/* create and fill the serializer's array of POI. 
	 * Be careful, if you have a reference on a mcq in your pois,
	 * We use the array that is in the serializer to get it.
	 * So populate your mcq before and do not sort them. */
	public void PopulatePOI(CSVSerializer serializer_)
	{
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


			int mcqId = -1;
			/* get mcq ref if have one */
			if (int.TryParse(values[7], out mcqId))
				newPoi._mcq = serializer_._mcqs[mcqId - 2];

			newPoi.transform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
			newPoi.transform.localScale = new Vector3(size, size, 1.0f);
		}
	}

	/* create and fill the serializer's array of MCQ. */
	public void PopulateMCQ(CSVSerializer serializer_)
	{
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
				uint.TryParse(values[1], out newMCQ._answerNb);
				bool.TryParse(values[2], out newMCQ._singleAnswer);

				/* get the right answer, no need to do hard thing 
				 * if there is a single answer */
				if (newMCQ._singleAnswer)
				{
					/* handle lower case and upper case */
					values[3].ToUpper();
					newMCQ._rightAnswerNb = new uint[1];
					newMCQ._rightAnswerNb[0] = (uint)(values[3][0] - 'A');
				}
				else
				{
					/* each answer will be split by a forward slash */
					string[] answers = values[3].Split('/');
					newMCQ._rightAnswerNb = new uint[answers.Length];

					/* now it should be only a character in those string so we recover it */
					for (int j = 0; j < answers.Length; j++)
					{
						answers[j].ToUpper();
						newMCQ._rightAnswerNb[j] = (uint)(answers[j][0] - 'A');
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
			}

			serializer_._mcqs.Insert(i, newMCQ);
		}
	}
}