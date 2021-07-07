using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI_Manager : MonoBehaviour
{
	/*==== SINGLETON ====*/
	public static POI_Manager Instance = null;

	/*==== SETTINGS ====*/
	[SerializeField] public List<POI> _pois = null;

    private void Awake()
    {
		Instance = this;
	}

    // Start is called before the first frame update
    void Start()
	{
		_pois.Sort();
	}

	// Update is called once per frame
	void Update()
	{   
	}
}
