using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

[RequireComponent(typeof(ARPointCloudManager))]
public class SwitchPointCloudVisualizationMode : MonoBehaviour
{
    public Button btn;
    public Button btn2;
    string path = "";
    string folderName = "xyz";
    string locations = "";
    int numPoints = 0;
    string readStr = "";
    List<Vector3> mPoints = new List<Vector3>(); // empty now
    static List<Vector3> newPoints = new List<Vector3>(); // empty now
    public GameObject go, go2;


    [SerializeField]
    Button m_ToggleButton;

    public Button toggleButton
    {
        get => m_ToggleButton;
        set => m_ToggleButton = value;
    }

    [SerializeField]
    Text m_Log;

    public Text log
    {
        get => m_Log;
        set => m_Log = value;
    }

    [SerializeField]
    ARAllPointCloudPointsParticleVisualizer.Mode m_Mode = ARAllPointCloudPointsParticleVisualizer.Mode.All;

    public ARAllPointCloudPointsParticleVisualizer.Mode mode
    {
        get => m_Mode;
        set => SetMode(value);
    }

    void Start()
    {

        //mPoints.Add(new Vector3(1f, 1f, 1f)); // adding an example Vector3
        foreach (var point in mPoints)
        {
            float myX = point.x;
        }

        btn.onClick.AddListener(SaveFile);
        btn2.onClick.AddListener(ReadFile);

        path = Application.persistentDataPath;
        if (!Directory.Exists(path + folderName))
        {
            Directory.CreateDirectory(path + folderName);
        }
        SaveFile();

        // You can find the created folder in your phone's memory
    }
    void SaveFile()
    {
        locations = SerializeVector3Array(mPoints);

        string fileName = "androidtxt";
        System.IO.File.WriteAllText(path + folderName + "/" + fileName + ".txt", locations);
    }

    void ReadFile()
    {

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            newPoints = mPoints;
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            string path = "Assets/StreamingAssets/dots.txt";

            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(path);
            readStr = reader.ReadToEnd();
            reader.Close();
            newPoints = DeserializeVector3Array(readStr);
        }
        

        
        for (int i = 0; i < newPoints.Count; i++) {
            if (i%10 == 0 && newPoints[i].y > -1 && newPoints[i].y < 1.3 && Mathf.Abs(newPoints[i].z) < 12 && Mathf.Abs(newPoints[i].x) < 12)
            {
                GameObject.Instantiate(go, newPoints[i],Quaternion.identity);
        }

    }
        
    }
    

    public string SerializeVector3Array(List<Vector3> aVectors)
    {
        StringBuilder sb = new StringBuilder();
        foreach (Vector3 v in aVectors)
        {
            sb.Append(v.x).Append(",").Append(v.y).Append(",").Append(v.z).Append("|");
        }
        if (sb.Length > 0) // remove last "|"
            sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }

    public List<Vector3> DeserializeVector3Array(string aData)
    {
        string[] vectors = aData.Split('|');
        List<Vector3> result = new List<Vector3>();
        for (int i = 0; i < vectors.Length; i++)
        {
            string[] values = vectors[i].Split(',');
            if (values.Length != 3)
                throw new System.FormatException("component count mismatch. Expected 3 components but got " + values.Length);
            result.Add(new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2])));
        }
        return result;
    }

    private RaycastHit hit;

    void Update()
    {
        
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0 && Input.touchCount < 2)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
                {
                    Debug.Log("I actually hit something");
                    Debug.Log(hit.point);
                    GameObject.Instantiate(go2, hit.point, Quaternion.identity);

                }
            }
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
            {
                Debug.Log("I actually hit something");
                Debug.Log(hit.point);
                GameObject.Instantiate(go2, hit.point, Quaternion.identity);
            }
        }
    }





    public void SwitchVisualizationMode()
    {
        SetMode((ARAllPointCloudPointsParticleVisualizer.Mode)(((int)m_Mode + 1) % 2));
    }

    void OnEnable()
    {
        SetMode(m_Mode);
        GetComponent<ARPointCloudManager>().pointCloudsChanged += OnPointCloudsChanged;
    }

    StringBuilder m_StringBuilder = new StringBuilder();

    void OnPointCloudsChanged(ARPointCloudChangedEventArgs eventArgs)
    {
        m_StringBuilder.Clear();
        foreach (var pointCloud in eventArgs.updated)
        {
            m_StringBuilder.Append($"\n{pointCloud.trackableId}: ");
            if (m_Mode == ARAllPointCloudPointsParticleVisualizer.Mode.CurrentFrame)
            {
                if (pointCloud.positions.HasValue)
                {
                    int maxLen = 1000;
                    if (pointCloud.positions.Value.Length < maxLen) {
                        maxLen = pointCloud.positions.Value.Length;
                    }
                    for (int i = 0; i < maxLen; i++) {
                        var points = pointCloud.positions.Value[i];
                        //locations += points.x + "," + points.y + "," + points.z + "|";
                        mPoints.Add(new Vector3(points.x, points.y, points.z));
                        numPoints++;
                    }
                    
                    m_StringBuilder.Append($"{pointCloud.positions.Value.Length}");
                    m_StringBuilder.Append(pointCloud.positions.Value[0]);
                }
                else
                {
                    m_StringBuilder.Append("0");
                }

                m_StringBuilder.Append(" points in current frame.");
            }
            else
            {
                var visualizer = pointCloud.GetComponent<ARAllPointCloudPointsParticleVisualizer>();
                if (visualizer)
                {
                    m_StringBuilder.Append($"{visualizer.totalPointCount} total points");
                    if (pointCloud.positions.HasValue)
                    {
                        int maxLen = 1000;
                        if (pointCloud.positions.Value.Length < maxLen)
                        {
                            maxLen = pointCloud.positions.Value.Length;
                        }
                        for (int i = 0; i < maxLen; i++)
                        {
                            var points = pointCloud.positions.Value[i];
                            //locations += points.x + "," + points.y + "," + points.z + "|";
                            mPoints.Add(new Vector3(points.x, points.y, points.z));
                            numPoints++;
                        }

                        m_StringBuilder.Append($"{pointCloud.positions.Value.Length}");
                        m_StringBuilder.Append(pointCloud.positions.Value[0]);
                    }
                }
            }
        }
        if (log)
        {
            //log.text = m_StringBuilder.ToString();
            log.text = numPoints.ToString();
            //log.text = pointCloud.positions.Value
        }
    }

    void SetMode(ARAllPointCloudPointsParticleVisualizer.Mode mode)
    {
        m_Mode = mode;
        if (toggleButton)
        {
            var text = toggleButton.GetComponentInChildren<Text>();
            switch (mode)
            {
                case ARAllPointCloudPointsParticleVisualizer.Mode.All:
                    text.text = "All";
                    break;
                case ARAllPointCloudPointsParticleVisualizer.Mode.CurrentFrame:
                    text.text = "Current Frame";
                    break;
            }
        }

        var manager = GetComponent<ARPointCloudManager>();
        foreach (var pointCloud in manager.trackables)
        {
            var visualizer = pointCloud.GetComponent<ARAllPointCloudPointsParticleVisualizer>();
            if (visualizer)
            {
                visualizer.mode = mode;
            }
        }
    }
}
