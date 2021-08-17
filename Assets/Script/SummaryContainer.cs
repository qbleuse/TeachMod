using System;
using UnityEngine;
using TMPro;

public class SummaryContainer : MonoBehaviour, IComparable<SummaryContainer>
{
    /*==== SETTINGS ====*/
    /* says what sequence it is suppose to appear */
    [SerializeField] public int _sequence = 0;
    /* the timestamp when it starts acting */
    [SerializeField] public float _timestamp = 0.0f;

    /* the rot that should be applied when setting this as the main summary */
    [SerializeField] Quaternion rot = Quaternion.identity;

    /*====  COMPONENTS ====*/
    public TextMeshProUGUI content = null;

    public void Init(POI poi_)
    {
        content = GetComponent<TextMeshProUGUI>();

        _sequence       = poi_._sequence;
        _timestamp      = poi_._timestamp;

        rot = poi_.transform.rotation;
    }

    public void Init(MCQ mcq_)
    {
        content = GetComponent<TextMeshProUGUI>();

        _sequence   = mcq_._sequence;
        _timestamp  = mcq_._timestamp;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public int CompareTo(SummaryContainer other)
    {
        if (_sequence < other._sequence || (_sequence == other._sequence && _timestamp < other._timestamp))
        {
            return -1;
        }

        return 1;
    }
}
