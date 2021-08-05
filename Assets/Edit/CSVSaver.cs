using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class CSVSaver
{
	/*==== STATE ====*/
	private char			_sepChar	= CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];
	private NumberStyles	_style		= NumberStyles.Float;
	private CultureInfo		_culture	= CultureInfo.CurrentCulture;

	string _content = null;

	public void Save(CSVSerializer serial_)
	{
		SavePOI(serial_);
	}

	private void SavePOI(CSVSerializer serial_)
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
		stringBuilder.Append("mcq Id");			stringBuilder.Append(_sepChar);
		stringBuilder.Append("comment");		stringBuilder.Append("\n");


		for (int i = 0; i < serial_._pois.Count; i++)
		{
			stringBuilder.Append(serial_._pois[i]._sequence);					stringBuilder.Append(_sepChar);
			stringBuilder.Append(serial_._pois[i]._timestamp);					stringBuilder.Append(_sepChar);
			stringBuilder.Append(serial_._pois[i]._endTimestamp);				stringBuilder.Append(_sepChar);
			stringBuilder.Append(serial_._pois[i]._askOnHit);					stringBuilder.Append(_sepChar);
			stringBuilder.Append(serial_._pois[i].transform.eulerAngles.y);		stringBuilder.Append(_sepChar);
			stringBuilder.Append(serial_._pois[i].transform.eulerAngles.x);		stringBuilder.Append(_sepChar);
			stringBuilder.Append(serial_._pois[i].transform.localScale.x);		stringBuilder.Append(_sepChar);
			if (serial_._pois[i]._mcq)
				stringBuilder.Append(serial_._pois[i]._mcq._serialID); 
			stringBuilder.Append(_sepChar);
			stringBuilder.Append("\"\"");stringBuilder.Append("\n");
		}

		_content = stringBuilder.ToString();
	}


	private void WriteMCQContent()
	{
		//question nb de réponse réponse unique bonne réponse(s)   pause sequence    timestamp
	}
}
