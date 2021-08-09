using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

/* acts as an intermediary between CSV files and assets in the game.
 * Used in Editor to be edited and in POI_Manager at init to create the assets we need */
[CreateAssetMenu(fileName = "CSVSerializer", menuName = "CSVSerializer")]
public class CSVSerializer : ScriptableObject
{
	/*==== SETTINGS ====*/
	[SerializeField] private POI _poiGo = null;


	/*==== SETTINGS ====*/
	[SerializeField] public string _poiList = null;
	[SerializeField] public string _mcqList = null;

	/*==== STATE ====*/
	public List<POI>		_pois		= null;
	public List<MCQ>		_mcqs		= null;
	[HideInInspector] public List<string>	_comments	= null;
	[HideInInspector] public Transform		_targetScene = null;

	public void Load()
    {
		CSVLoader loader = new CSVLoader();

		if (_poiGo == null)
        {
			GetPrefab();
        }

		loader.LoadFile(_mcqList);
		loader.PopulateMCQ(this);

		loader.LoadFile(_poiList);
		loader.PopulatePOI(this);
	}

	public void Clear()
	{
		_pois.Clear();
		_mcqs.Clear();
		_comments.Clear();
	}

	public POI InstantiatePOI(int i_)
	{
		if (_targetScene)
		{
			/* create game object in target scene then put on root */
			POI newPoi = Instantiate(_poiGo, _targetScene);
			newPoi.transform.SetParent(null);
			_pois.Insert(i_, newPoi);
		}
		else
		{
			_pois.Insert(i_, Instantiate(_poiGo));
		}

		return _pois[i_];
	}

	private void GetPrefab()
    {
		_poiGo = AssetDatabase.LoadAssetAtPath<POI>("Assets/Prefabs/POI.prefab");
    }
}
