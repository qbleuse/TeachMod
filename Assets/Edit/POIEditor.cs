using UnityEditor;
using UnityEngine;

/* Editor that edits each asset at a time.
 * used by CSVEditor */
public class POIEditor : MonoBehaviour
{
	public POI_Manager _poi_man = null;

	Editor _poiEditor = null;
	Editor _mcqEditor = null;

	[SerializeField, Multiline] string	_editComment	= null;
	[SerializeField]			POI		_editPOI		= null;
	[SerializeField]			MCQ		_editMCQ		= null;

    public void OnInspectorGUI()
	{

		GUILayout.BeginHorizontal();
		OnPOIGUI();
		OnMCQGUI();
		GUILayout.EndHorizontal();
	}


	private void OnPOIGUI()
	{
		/* POI Edit */
		GUILayout.BeginVertical();
		if (_editPOI == null)
		{
			if (GUILayout.Button("Add POI"))
				AddPOI();

			if (_editMCQ)
				_editComment = EditorGUILayout.TextArea(_editComment);

		}
		else
		{
			if (GUILayout.Button("Clear POI"))
			{
				ClearPOI();
				if (_editMCQ == null)
					_editComment = null;
			}
			else
			{ 
				_poiEditor.OnInspectorGUI();
				_editComment = EditorGUILayout.TextArea(_editComment);
			}
		}
		GUILayout.EndVertical();
	}

	private void OnMCQGUI()
	{
		/* MCQ Edit */
		GUILayout.BeginVertical();
		if (_editMCQ == null && GUILayout.Button("Add MCQ"))
			AddMCQ();
		else if (_editMCQ)
		{
			if (GUILayout.Button("Clear MCQ"))
			{
				ClearMCQ();
				if (_editPOI == null)
				{
					_editComment = null;
				}
			}
			else
			{
				_mcqEditor.OnInspectorGUI();
			}
		}
		GUILayout.EndVertical();
	}

	private void AddPOI()
	{
		_editPOI = _poi_man._csvSerial.InstantiatePOI(_poi_man._pois.Count - 1);

		_poiEditor = Editor.CreateEditor(_editPOI);
	}

	private void AddMCQ()
	{
		_editMCQ = ScriptableObject.CreateInstance<MCQ>();

		_mcqEditor = Editor.CreateEditor(_editMCQ);
	}

	private void ClearPOI()
	{
		DestroyImmediate(_editPOI);
		_editPOI = null;

		DestroyImmediate(_poiEditor);
		_poiEditor = null;
	}

	private void ClearMCQ()
	{
		DestroyImmediate(_editMCQ);
		_editMCQ = null;

		DestroyImmediate(_mcqEditor);
		_mcqEditor = null;
	}
}
