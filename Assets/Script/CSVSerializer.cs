using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CSVSerializer : ScriptableObject
{
    [MenuItem("Assets/Create/CSVSerializer")]
    public static void Create()
    {
        CSVSerializer asset = CreateInstance<CSVSerializer>();

        string name = AssetDatabase.GenerateUniqueAssetPath("Assets/NewCSVSerializer.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    /*==== SETTINGS ====*/
    [SerializeField] POI _poiGo = null;

    /*==== COMPONENT ====*/
    [HideInInspector]   public List<string> _lines = new List<string>();


    /*==== STATE ====*/
    [HideInInspector]   public char         _sepChar = ',';
    [HideInInspector]   public NumberStyles _style;
    [HideInInspector]   public CultureInfo  _culture;

    /* method loading the lines of the csv file in the lines component */
    public void LoadFile(string filePath_)
    {
        _lines.Clear();
        using (StreamReader reader = new StreamReader(Application.dataPath + '/' + filePath_))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                _lines.Add(line);
            }
        }

        _style = NumberStyles.AllowDecimalPoint;
        
        /* chooses between the french and the english way of parsing float 
         * depending on the separating value*/
        string description = _lines[0];
        if (_lines[0].Contains(";"))
        {
            _sepChar = ';';
            _culture = CultureInfo.CreateSpecificCulture("fr-FR");
        }
        else
        {
            _culture = CultureInfo.CreateSpecificCulture("en-EN");
        }
    }

    public void PopulatePOI(POI_Manager poi_man_)
    {
        float yaw   = 0;
        float pitch = 0;
        float size = 0.0f;
        for (int i = 1; i < _lines.Count; i++)
        {
            string[] values = _lines[i].Split(_sepChar);

            POI newPoi = Instantiate(_poiGo);

            /* get number to sort */
            int.TryParse(values[0], out newPoi._number);

            /* get timestamp and sequence */
            int.TryParse(values[1], out newPoi._sequence);
            float.TryParse(values[2],_style, _culture, out newPoi._timestamp);
            float.TryParse(values[3], _style, _culture,  out newPoi._endTimestamp);

            /* get behavior info */
            bool.TryParse(values[4], out newPoi._pauseOnTime);
            bool.TryParse(values[5], out newPoi._askOnHit);

            /* get transform info */
            float.TryParse(values[6], _style, _culture, out yaw);
            float.TryParse(values[7], _style, _culture, out pitch);
            float.TryParse(values[8], _style, _culture, out size);

            newPoi.transform.rotation   = Quaternion.Euler(pitch, yaw, 0.0f);
            newPoi.transform.localScale = new Vector3(size,size,1.0f);

            if (newPoi._pauseOnTime)
            {
                poi_man_._pausePois.Add(newPoi);
                continue;
            }
            
            poi_man_._pois.Add(newPoi);
        }
    }
}
