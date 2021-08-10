using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

/* Window to help creating POI that connects with the video.
 * An Editor to help create the assets to serialize. */
public class CSVEditor : EditorWindow
{
	/* */
	Camera			_camera		= null;
	POI_Manager		_poi_man	= null;
	POIEditor		_poiEditor	= null;
	EditVideoPlayer _videoPlayer;
	Scene _editScene;


	Rect	_videoRect;
	Rect	_editRect;
	Vector2 _csvScroll = Vector2.zero;

	[SerializeField] CSVSerializer _editSave = null;

	[MenuItem("Window/CSVEditor")]
	public static void ShowWindow()
	{
		CSVEditor editor = GetWindow<CSVEditor>("POIEditor");
		editor.Enable();
	}

	private void Enable()
	{
		_editScene		= EditorSceneManager.OpenScene("Assets/Edit/EditScene.unity", OpenSceneMode.Additive);
		_camera			= _editScene.GetRootGameObjects()[0].GetComponent<Camera>();
		_poi_man		= _editScene.GetRootGameObjects()[2].GetComponent<POI_Manager>();
		_poiEditor		= _poi_man.GetComponent<POIEditor>();
		_videoPlayer	= _camera.GetComponent<EditVideoPlayer>();

		_poiEditor._poi_man = _poi_man;
		_poiEditor._cam		= _camera;

		_videoRect	= new Rect();
		_editRect	= new Rect();

		/* put the texture of the video player as target */
		_videoPlayer._texture = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0);
		_camera.targetTexture = _videoPlayer._texture;

		_videoPlayer.Start();
	}

	private void OnDestroy()
	{
		EditorSceneManager.CloseScene(_editScene, true);
	}

	private void RectUpdate()
	{
		_videoRect.x = 0;
		_videoRect.y = 0;
		_videoRect.width = position.width;
		_videoRect.height = position.height / 2.0f;

		_camera.aspect = _videoRect.width / _videoRect.height;

		_editRect.x = 0;
		_editRect.y = position.height / 2.0f;
		_editRect.width = position.width;
		_editRect.height = position.height / 2.0f;

	}

	private void OnGUI()
	{

		RectUpdate();
		_camera.Render();

		_videoPlayer.DrawVideo(_videoRect);

		if (Event.current.type == EventType.ContextClick)
        {
			_poiEditor.ApplyCamRot();
        }

		GUILayout.BeginArea(_editRect);

		if (GUILayout.Button("Save"))
        {
			Debug.LogWarning("Save");
			CSVSaver saver = new CSVSaver();

			saver.Save(_editSave);
        }

		_videoPlayer.OnInspectorGUI();
		OnInspectorGUI();

		GUILayout.EndArea();

	}

	public void OnInspectorGUI()
	{
		CSVSerializer save = EditorGUILayout.ObjectField("CSVSerializer", _editSave, typeof(CSVSerializer), false) as CSVSerializer;

		_csvScroll = GUILayout.BeginScrollView(_csvScroll);

		OnSaveGUI(ref save);
		_poiEditor.OnInspectorGUI();

		GUILayout.EndScrollView();
	}

	private void OnSaveGUI(ref CSVSerializer serializer_)
	{
		if (serializer_ != null && _editSave == null)
		{
			_editSave = serializer_;
			_editSave._targetScene = _poi_man.transform;
			_editSave.Load();

			_poi_man._csvSerial = _editSave;

			_poi_man._pois = _editSave._pois;
			_poi_man._mcqs = _editSave._mcqs;
			_poi_man._comments = _editSave._comments;

			for (int i = 0;  i < _poi_man._pois.Count; i++)
            {
				_poi_man._pois[i].PutToSleep();
            }

			if (serializer_._pois.Count > 0)
				_poiEditor.SetPOI();
			if (serializer_._mcqs.Count > 0)
				_poiEditor.SetMCQ();
		}
		else if (serializer_ == null && _editSave)
		{
			_poi_man._csvSerial = null;
			_editSave.Clear();
		}
	}

	private void Update()
	{
		if (_videoPlayer._player.isPlaying || mouseOverWindow)
			Repaint();
	}
}
