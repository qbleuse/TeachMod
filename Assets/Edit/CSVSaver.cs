using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class CSVSaver
{
	/*==== STATE ====*/
	private char			_sepChar	= CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];
	private CultureInfo		_culture	= CultureInfo.CurrentCulture;

	string _content = null;

	public void Save(CSVSerializer serial_)
	{
		SavePOI(serial_);
		SaveMCQ(serial_);
	}

	public void SavePOI(CSVSerializer serial_)
	{
		/* erasing the entire content of the file */
		using (FileStream fStream = new FileStream(Application.dataPath + '/' + serial_._poiList, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
		{
			lock (fStream)
			{
				fStream.SetLength(0);
			}
		}

		/* get the whole file in one string */
		_content = null;
		WritePOIContent(serial_);

		/* reopening it and filling it */
		using (FileStream fStream = new FileStream(Application.dataPath + '/' + serial_._poiList, FileMode.Open, FileAccess.Write, FileShare.None))
		using (StreamWriter writer = new StreamWriter(fStream))
		{
			writer.Write(_content);
		}
	}

	private void WritePOIContent(CSVSerializer serial_)
	{
		StringBuilder stringBuilder = new StringBuilder();

		stringBuilder.Append("sequence");		stringBuilder.Append(_sepChar);
		stringBuilder.Append("timestamp");		stringBuilder.Append(_sepChar);
		stringBuilder.Append("end timestamp");	stringBuilder.Append(_sepChar);
		stringBuilder.Append("ask On Hit");		stringBuilder.Append(_sepChar);
		stringBuilder.Append("Yaw");			stringBuilder.Append(_sepChar);
		stringBuilder.Append("Pitch");			stringBuilder.Append(_sepChar);
		stringBuilder.Append("Size");			stringBuilder.Append(_sepChar);
		stringBuilder.Append("mcq Id");			stringBuilder.AppendLine();


		for (int i = 0; i < serial_._pois.Count; i++)
		{
			stringBuilder.Append(serial_._pois[i]._sequence + 1);									stringBuilder.Append(_sepChar);
			stringBuilder.AppendFormat(_culture, "{0}", serial_._pois[i]._timestamp);				stringBuilder.Append(_sepChar);
			stringBuilder.AppendFormat(_culture,"{0}",serial_._pois[i]._endTimestamp);				stringBuilder.Append(_sepChar);
			stringBuilder.Append(serial_._pois[i]._askOnHit);										stringBuilder.Append(_sepChar);
			stringBuilder.AppendFormat(_culture, "{0}", serial_._pois[i].transform.eulerAngles.y);	stringBuilder.Append(_sepChar);
			stringBuilder.AppendFormat(_culture,"{0}",serial_._pois[i].transform.eulerAngles.x);	stringBuilder.Append(_sepChar);
			stringBuilder.AppendFormat(_culture,"{0}",serial_._pois[i].transform.localScale.x);		stringBuilder.Append(_sepChar);
			if (serial_._pois[i]._mcq)
				stringBuilder.Append(serial_._pois[i]._mcq._serialID + 2); 
			stringBuilder.AppendLine();
		}

		_content = stringBuilder.ToString();
	}

	public void SaveMCQ(CSVSerializer serial_)
	{
		/* erasing the entire content of the file */
		using (FileStream fStream = new FileStream(Application.dataPath + '/' + serial_._mcqList, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
		{
			lock (fStream)
			{
				fStream.SetLength(0);
			}
		}

		/* get the whole file in one string */
		_content = null;
		WriteMCQContent(serial_);

		/* reopening it and filling it */
		using (FileStream fStream = new FileStream(Application.dataPath + '/' + serial_._mcqList, FileMode.Open, FileAccess.Write, FileShare.None))
		using (StreamWriter writer = new StreamWriter(fStream))
		{
			writer.Write(_content);
		}
	}


	private void WriteMCQContent(CSVSerializer serial_)
	{
		StringBuilder stringBuilder = new StringBuilder();

		stringBuilder.Append("question");		stringBuilder.Append(_sepChar);
		stringBuilder.Append("answer nb");		stringBuilder.Append(_sepChar);
		stringBuilder.Append("single answer");	stringBuilder.Append(_sepChar);
		stringBuilder.Append("good answers");	stringBuilder.Append(_sepChar);
		stringBuilder.Append("pause");			stringBuilder.Append(_sepChar);
		stringBuilder.Append("sequence");		stringBuilder.Append(_sepChar);
		stringBuilder.Append("timestamp");		stringBuilder.Append(_sepChar);
		stringBuilder.Append("comment");		stringBuilder.AppendLine();


		for (int i = 0; i < serial_._mcqs.Count; i++)
		{
			/* question tends to be multiline, in those case the format of csv needs it to be surrounded by quotation marks (") */
			stringBuilder.Append("\""); stringBuilder.Append(serial_._mcqs[i]._question); stringBuilder.Append("\""); stringBuilder.Append(_sepChar);

			stringBuilder.Append(serial_._mcqs[i]._answerNb);		stringBuilder.Append(_sepChar);
			stringBuilder.Append(serial_._mcqs[i]._singleAnswer);	stringBuilder.Append(_sepChar);

			/* taking every good answer in a string that separated by forward slash (eg: "1/4/5")*/
			stringBuilder.Append((char)(serial_._mcqs[i]._rightAnswerNb[0] + 'A'));
			for (int j = 1; j < serial_._mcqs[i]._rightAnswerNb.Count; j++)
            {
				stringBuilder.Append("/"); stringBuilder.Append((char)(serial_._mcqs[i]._rightAnswerNb[j] + 'A'));
			}
			stringBuilder.Append(_sepChar);

			stringBuilder.Append(serial_._mcqs[i]._pause); stringBuilder.Append(_sepChar);
			if (serial_._mcqs[i]._pause)
				stringBuilder.Append(serial_._mcqs[i]._sequence+1);
			stringBuilder.Append(_sepChar);
			if (serial_._mcqs[i]._pause)
				stringBuilder.AppendFormat(_culture, "{0}", serial_._mcqs[i]._timestamp);
			stringBuilder.Append(_sepChar);

			if (serial_._mcqs[i]._comment != null && serial_._mcqs[i]._comment.Length > 0)
            {
				stringBuilder.Append("\""); stringBuilder.Append(serial_._mcqs[i]._comment); stringBuilder.Append("\"");
			}
			stringBuilder.AppendLine();
		}

		_content = stringBuilder.ToString();

	}
}
