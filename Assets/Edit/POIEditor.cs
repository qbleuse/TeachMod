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

	[SerializeField, Multiline] string	_editComment	= null;
	[SerializeField]			POI		_editPOI		= null;
	[SerializeField]			MCQ		_editMCQ		= null;

	int _editPOIId = 0;
	int _editMCQID = 0;

	GUILayoutOption _buttonWidth	= GUILayout.Width(30f);
	GUILayoutOption _labelWidth		= GUILayout.Width(50f);

	public float _yaw	= 0.0f;
	public float _pitch = 0.0f;
	public float _size	= 0.0f;

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
		int newId = EditorGUILayout.IntSlider(_editPOIId, 0, Mathf.Max(_poi_man._pois.Count - 1,0));

		if (newId != _editPOIId)
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
		newId = EditorGUILayout.IntSlider(_editMCQID, 0, Mathf.Max(_poi_man._mcqs.Count - 1,0));

		if (newId != _editMCQID)
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
			_editComment = EditorGUILayout.TextArea(_editComment);
		}
		GUILayout.EndVertical();
	}

	private void OnMCQGUI()
	{
		/* MCQ Edit */
		GUILayout.BeginVertical();
		if (_editMCQ)
		{
			_mcqEditor.OnInspectorGUI();
		}
		GUILayout.EndVertical();
	}

	private void AddPOI()
	{
		if (_poi_man._csvSerial)
		{
			_poi_man._csvSerial.InstantiatePOI(_poi_man._pois.Count );
			_editPOIId = _poi_man._pois.Count - 1;
			SetPOI();
			return;
		}

		Debug.LogError("Please register a CSVSerializer before adding a new POI");
	}


	public void SetPOI()
	{
		if (_editPOI)
			_editPOI.PutToSleep();

		_editPOI	= _poi_man._pois[_editPOIId];
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
			_poi_man._pois.RemoveAt(_editPOIId);

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
			_poi_man._csvSerial._mcqs.Insert(_poi_man._mcqs.Count, _editMCQ);
			_editPOIId = _poi_man._mcqs.Count - 1;
			_mcqEditor = Editor.CreateEditor(_editMCQ);
			return;
		}

		Debug.LogError("Please register a CSVSerializer before adding a new MCQ");
	}

	private void ClearMCQ()
	{
		if (_poi_man._csvSerial)
		{
			_poi_man._mcqs.RemoveAt(_editMCQID);

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
		_editMCQ = _poi_man._mcqs[_editMCQID];

		_mcqEditor = Editor.CreateEditor(_editMCQ);
	}
}
