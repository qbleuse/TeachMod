using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI_Manager : MonoBehaviour
{
	/*==== SINGLETON ====*/
	public static POI_Manager Instance = null;

	/*==== STATE ====*/
	[HideInInspector] public	List<POI> _pois = null;
	[HideInInspector] public	List<POI> _pausePois = null;
	private int _index = 0;

    private void Awake()
    {
		Instance = this;
	}

    // Start is called before the first frame update
    IEnumerator Start()
	{
		/* wait for all POI to register themselves */
		yield return new WaitForSeconds(0.5f);

		_pois.Sort();
	}

	// Update is called once per frame
	void Update()
	{
		TryEnablePOI();
	}

	void TryEnablePOI()
    {
		for (; _index < _pois.Count; _index++)
        {
			if (_pois[_index]._sequence == VideoController.Instance._currentVideoIndex 
				&& _pois[_index]._movement[0].timestamp < VideoController.Instance.GetVideoTimeStamp())
            {
				_pois[_index].WakeUp();
            }
			else
            {
				break;
            }
        }
    }
}
