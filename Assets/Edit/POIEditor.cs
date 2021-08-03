using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

/* Window to help creating POI that connects with the video.
 * An Editor to help create the assets to serialize. */
public class POIEditor : EditorWindow
{
	/* */
	Camera                  _camera		= null;
	Scene                   _editScene;

	Rect                    _videoRect;
	EditVideoPlayer         _videoPlayer;

	Rect					_editRect;
	Editor					_poiEditor = null;
	Editor					_mcqEditor = null;

	Vector2					_poiScroll = Vector2.zero;
	Vector2					_mcqScroll = Vector2.zero;

	[SerializeField] POI	_editPOI = null;
	[SerializeField] MCQ	_editMCQ = null;

	[MenuItem("Window/POIEditor")]
	public static void ShowWindow()
	{
		GetWindow<POIEditor>("POIEditor");
	}


	private void OnEnable()
	{ 
		_editScene		= EditorSceneManager.OpenScene("Assets/Edit/EditScene.unity",OpenSceneMode.Additive);
		_camera			= _editScene.GetRootGameObjects()[0].GetComponent<Camera>();
		_videoPlayer	= _camera.GetComponent<EditVideoPlayer>();

		_videoRect	= new Rect();
		_editRect	= new Rect();

		/* put the texture of the video player as target */
		_videoPlayer._texture   = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight,0);
		_camera.targetTexture	= _videoPlayer._texture;

		_videoPlayer.Start();
	}

	private void OnDisable()
	{
		EditorSceneManager.CloseScene(_editScene,true);
	}

	private void RectUpdate()
    {
		_videoRect.x		= 0;
		_videoRect.y		= 0;
		_videoRect.width	= position.width;
		_videoRect.height	= position.height / 2.0f;

		_camera.aspect		= _videoRect.width / _videoRect.height;

		_editRect.x			= 0;
		_editRect.y			= position.height / 2.0f;
		_editRect.width		= position.width;
		_editRect.height	= position.height / 2.0f;

	}

	private void OnGUI()
	{
		RectUpdate();
		_camera.Render();

		_videoPlayer.DrawVideo(_videoRect);

		GUILayout.BeginArea(_editRect);

		_videoPlayer.OnInspectorGUI();
		OnInspectorGUI();
		
		GUILayout.EndArea();

	}

	public void OnInspectorGUI()
    {
		GUILayout.BeginHorizontal();

		/* POI Edit */
		GUILayout.BeginVertical();
		if (_editPOI == null && GUILayout.Button("Add POI"))
			AddPOI();
		else if (_editPOI)
		{
			if (GUILayout.Button("Clear POI"))
				ClearPOI();
			else
			{
				_poiScroll = GUILayout.BeginScrollView(_poiScroll);
				_poiEditor.OnInspectorGUI();
				GUILayout.EndScrollView();
			}
		}
		GUILayout.EndVertical();

		/* MCQ Edit */
		GUILayout.BeginVertical();
		if (_editMCQ == null && GUILayout.Button("Add MCQ"))
			AddMCQ();
		else if (_editMCQ)
		{
			if (GUILayout.Button("Clear MCQ"))
				ClearMCQ();
			else
			{
				_mcqScroll = GUILayout.BeginScrollView(_mcqScroll);
				_mcqEditor.OnInspectorGUI();
				GUILayout.EndScrollView();

			}
		}
		GUILayout.EndVertical();

		GUILayout.EndHorizontal();
	}

	private void AddPOI()
    {
		_editPOI = _camera.gameObject.AddComponent<POI>();

		_poiEditor = Editor.CreateEditor(_editPOI);
    }

	private void AddMCQ()
	{
		_editMCQ = CreateInstance<MCQ>();

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

	private void Update()
	{
		if (_videoPlayer && _videoPlayer._player && _videoPlayer._player.isPlaying)
			Repaint();
	}
}
