using UnityEditor;
using UnityEngine;

/* Editor that edits each asset at a time.
 * used by CSVEditor */
public class POIEditor : MonoBehaviour
{
	/*==== STATE ====*/
	public POI_Manager	_poi_man = null;
	public Camera		_cam = null;

	Editor _poiEditor = null;
	Editor _mcqEditor = null;

	[SerializeField]			POI		_editPOI		= null;
	[SerializeField]			MCQ		_editMCQ		= null;

	int _editPOIId = 1;
	int _editMCQID = 1;

	GUILayoutOption _buttonWidth	= GUILayout.Width(30f);
	GUILayoutOption _labelWidth		= GUILayout.Width(50f);

	public float _yaw	= 0.0f;
	public float _pitch = 0.0f;
	public float _size	= 0.0f;

	public void Clear()
    {
		_editPOI = null;
		_editMCQ = null;
		_editPOIId = 1;
		_editMCQID = 1;
	}

	public void OnInspectorGUI()
	{

		GUILayout.BeginVertical();
		OnManagerGUI();
		GUILayout.BeginHorizontal();
		OnPOIGUI();
		OnMCQGUI();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}

	private void OnManagerGUI()
	{
		bool clickEnd = Event.current.type == EventType.MouseUp;

		GUILayout.BeginHorizontal();
		GUILayout.Label("POI NB: ",EditorStyles.miniLabel, _labelWidth);
		int newId = EditorGUILayout.IntSlider(_editPOIId, 1, Mathf.Max(_poi_man._pois.Count,1));

		if (newId != _editPOIId && _poi_man._pois.Count > 0)
		{
			_editPOIId = newId;
			SetPOI();
		}

		if (GUILayout.Button("+", EditorStyles.miniButtonLeft,_buttonWidth) && clickEnd)
			AddPOI();
		if (GUILayout.Button("-", EditorStyles.miniButtonRight,_buttonWidth) && clickEnd)
			ClearPOI();

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("MCQ NB: ", EditorStyles.miniLabel, _labelWidth);
		newId = EditorGUILayout.IntSlider(_editMCQID, 1, Mathf.Max(_poi_man._mcqs.Count,1));

		if (newId != _editMCQID && _poi_man._mcqs.Count > 0)
		{
			_editMCQID = newId;
			SetMCQ();
		}

		if (GUILayout.Button("+", EditorStyles.miniButtonLeft,_buttonWidth) && clickEnd)
			AddMCQ();
		if (GUILayout.Button("-", EditorStyles.miniButtonRight,_buttonWidth) && clickEnd)
			ClearMCQ();

		GUILayout.EndHorizontal();
	}

	public void ApplyCamRot()
	{
		if (_editPOI)
		{
			_editPOI.transform.rotation = _cam.transform.rotation;
			_pitch	= _cam.transform.rotation.eulerAngles.x;
			_yaw	= _cam.transform.rotation.eulerAngles.y;
		}
	}


	private void OnPOIGUI()
	{
		/* POI Edit */
		GUILayout.BeginVertical();
		if (_editPOI)
		{
			_editPOI._sequence++;
			/* changing size */
			GUILayout.BeginHorizontal();
			GUILayout.Label("Size:", EditorStyles.miniLabel, _labelWidth);
			float sliderResult = EditorGUILayout.Slider(_size,0.0f,1.0f);
			if (sliderResult != _size)
			{
				_size = sliderResult;
				_editPOI.transform.localScale = new Vector3(_size, _size, 1.0f);
			}
			GUILayout.EndHorizontal();

			/* changing position depending on angles */
			GUILayout.BeginHorizontal();
			GUILayout.Label("Yaw:", EditorStyles.miniLabel, _labelWidth);
			float yaw = EditorGUILayout.Slider(_yaw, 0.0f, 360.0f);
			GUILayout.Label("Pitch:", EditorStyles.miniLabel, _labelWidth);
			sliderResult = EditorGUILayout.Slider(_pitch, 0.0f, 360.0f);

			if (yaw != _yaw || _pitch != sliderResult)
			{
				_yaw	= yaw;
				_pitch	= sliderResult;
				_editPOI.transform.rotation = Quaternion.Euler(_pitch, _yaw, 0.0f);
			}
			GUILayout.EndHorizontal();

			/* POI real thing */
			_poiEditor.OnInspectorGUI();

			int mcq_nb		= EditorGUILayout.IntField(_editPOI._mcq ? (_editPOI._mcq._serialID+1) : -1);
			_editPOI._mcq	= mcq_nb > 0  && mcq_nb <= _poi_man._mcqs.Count ? _poi_man._mcqs[mcq_nb - 1] : null;
			_editPOI._sequence--;
		}
		GUILayout.EndVertical();
	}

	private void OnMCQGUI()
	{
		/* MCQ Edit */
		GUILayout.BeginVertical();
		if (_editMCQ)
		{
			_editMCQ._sequence++;
			_mcqEditor.OnInspectorGUI();

			bool visible = false;
			visible = EditorGUILayout.Foldout(true, "Right Answers");
			if (visible)
			{
				bool clickEnd = Event.current.type == EventType.MouseUp;

				EditorGUI.indentLevel ++;

				GUILayout.BeginHorizontal();
				EditorGUILayout.IntField(_editMCQ._rightAnswerNb.Count);
				if (GUILayout.Button("+", EditorStyles.miniButtonLeft, _buttonWidth) && clickEnd && _editMCQ._rightAnswerNb.Count < 5)
					_editMCQ._rightAnswerNb.Add('A');
				if (GUILayout.Button("-", EditorStyles.miniButtonRight, _buttonWidth) && clickEnd && _editMCQ._rightAnswerNb.Count > 0)
					_editMCQ._rightAnswerNb.RemoveAt(_editMCQ._rightAnswerNb.Count - 1);
				GUILayout.EndHorizontal();

				EditorGUI.indentLevel ++;
				string result;
				int nb = 'A';
				for (int i = 0; i < _editMCQ._rightAnswerNb.Count; i++)
				{
					result = ((char)(_editMCQ._rightAnswerNb[i] + 'A')).ToString();
					result = EditorGUILayout.TextField(result.ToString());

					result.ToUpper();
					nb = result[0];
					if (nb < 'A' || nb > 'E')
						nb = 'A';

					_editMCQ._rightAnswerNb[i] = nb - 'A';
				}
				EditorGUI.indentLevel -= 2;
			}

			_editMCQ._sequence--;
		}
		GUILayout.EndVertical();
	}

	private void AddPOI()
	{
		if (_poi_man._csvSerial)
		{
			_poi_man._csvSerial.InstantiatePOI(_poi_man._pois.Count );
			_editPOIId = _poi_man._pois.Count;
			SetPOI();
			return;
		}

		Debug.LogError("Please register a CSVSerializer before adding a new POI");
	}


	public void SetPOI()
	{
		if (_editPOI)
			_editPOI.PutToSleep();

		_editPOI	= _poi_man._pois[_editPOIId - 1];
		_editPOI.gameObject.SetActive(true);

		_size		= _editPOI.transform.localScale.x;
		_pitch		= _editPOI.transform.rotation.eulerAngles.x;
		_yaw		= _editPOI.transform.rotation.eulerAngles.y;

		_poiEditor = Editor.CreateEditor(_editPOI);
	}

	private void ClearPOI()
	{
		if (_poi_man._csvSerial)
		{
			_poi_man._pois.RemoveAt(_editPOIId - 1);

			DestroyImmediate(_editPOI.gameObject);
			_editPOI = null;

			DestroyImmediate(_poiEditor);
			_poiEditor = null;
			return;
		}

		Debug.LogError("Please register a CSVSerializer before removing a POI");
	}


	private void AddMCQ()
	{
		if (_poi_man._csvSerial)
		{
			_editMCQ = ScriptableObject.CreateInstance<MCQ>();
			_poi_man._mcqs.Insert(_poi_man._mcqs.Count, _editMCQ);
			_poi_man._mcqs[_poi_man._mcqs.Count - 1]._serialID = _poi_man._mcqs.Count - 1;
			_editMCQID = _poi_man._mcqs.Count;
			_mcqEditor = Editor.CreateEditor(_editMCQ);
			return;
		}

		Debug.LogError("Please register a CSVSerializer before adding a new MCQ");
	}

	private void ClearMCQ()
	{
		if (_poi_man._csvSerial)
		{
			_poi_man._mcqs.RemoveAt(_editMCQID - 1);

			DestroyImmediate(_editMCQ);
			_editMCQ = null;

			DestroyImmediate(_mcqEditor);
			_mcqEditor = null;
			return;
		}

		Debug.LogError("Please register a CSVSerializer before removing a MCQ");
	}

	public void SetMCQ()
	{
		_editMCQ = _poi_man._mcqs[_editMCQID - 1];

		_mcqEditor = Editor.CreateEditor(_editMCQ);
	}
}
