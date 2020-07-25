using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

[RequireComponent(typeof(ARPointCloudManager))]
public class SwitchPointCloudVisualizationMode : MonoBehaviour
{
    public int pubInt = 0;
    public Button btn; // not used - was save file
    public Button btn2; // done scanning
    public Button btn3; // trees done, view treehouse
    public Button btn4; // trees done, view indesigner
    //public GameObject btn3;
    string path = "";
    string folderName = "xyz";
    string locations = "";
    int numPoints = 0;
    string readStr = "";
    List<Vector3> mPoints = new List<Vector3>(); // empty now
    public static List<Vector3> newPoints = new List<Vector3>(); // empty now
    public GameObject go, go2;
    List<Vector3> result = new List<Vector3>();
    float minDist = 0.35f;

    public GameObject[] treeimgs;

    public Camera cam;

    public GameObject TreeHouse;
    int currentTree = 0;

    bool logShow = false;

    public List<GameObject> TreeCyls = new List<GameObject>(3);

    float Mx_c = 0f; //Circle's center coordinate (X Axis).
    float My_c = 0f; //Circle's center coordinate (Y Axis).
    float Mr = 0f; //Circle's radius.

    bool runOnce = true;

    //public GameObject center;
    public static Transform[] treeTransform1;

    List<Vector3> tree1Rad = new List<Vector3>();
    Vector3[] tree1RadArray;

    int screenWidth = 0;
    int currentPos = 0;
    RectTransform rt;
    public GameObject movingPanel;
    public GameObject parentPanelScan;
    public GameObject parentPanelTree;

    //public Button scanningState;


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
        //scanningState.onClick.AddListener(ChangeTextAndDoSomething);

        screenWidth = (int)Screen.width;

        rt = movingPanel.GetComponent<RectTransform>();
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, currentPos);

        //mPoints.Add(new Vector3(1f, 1f, 1f)); // adding an example Vector3
        //foreach (var point in mPoints)
        //{
        //    float myX = point.x;
        //}

        btn.onClick.AddListener(SaveFile);
        btn2.onClick.AddListener(ReadFile);
        btn3.onClick.AddListener(showTreehouse);
        btn4.onClick.AddListener(goToDesigner);

        path = Application.persistentDataPath;
        if (!Directory.Exists(path + folderName))
        {
            Directory.CreateDirectory(path + folderName);
        }
        SaveFile();

        // You can find the created folder in your phone's memory
    }

    void goToDesigner() {
        myScript2.canGoForwards = true;
        //myScript2.nScale = 0;
        myScript2.mScale = 1;
    }

    void showTreehouse() {
        if (currentTree >= 3 && runOnce)
        {
            Debug.Log("party time");
            TreeHouse.GetComponent<Treehouse>().enabled = true;
            //Treehouse.NeedRefresh = true;
            runOnce = false;
            gameObject.GetComponent<ARPointCloudManager>().enabled = false;
            btn3.gameObject.SetActive(false);
            btn4.gameObject.SetActive(true);
        }

    }
    void SaveFile()
    {
        locations = SerializeVector3Array(mPoints);

        string fileName = "androidtxt";
        System.IO.File.WriteAllText(path + folderName + "/" + fileName + ".txt", locations);
    }

    void ReadFile()
    {
        //currentStateOfScan++;
        //Color btnclr = btn2.GetComponent<Image>().color;
        //string mText = btn3.GetComponent<Text>().text;
        //if (currentStateOfScan == 1)
        //{
            //btnclr = Color.white;
            //mText = "CLICK ME";
            btn2.gameObject.SetActive(false);
        btn3.gameObject.SetActive(true);
            parentPanelScan.SetActive(false);
            parentPanelTree.SetActive(true);

        //}

        logShow = true;
        newPoints = new List<Vector3>();

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            
            foreach (var pt in mPoints) {
                newPoints.Add(pt);
            }
            
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
            if (Mathf.Abs(newPoints[i].y) < 0.25f && i % 5 == 0)
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
        //List<Vector3> result = new List<Vector3>();
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

        if (logShow == false)
        {
            //log.text = m_StringBuilder.ToString();
            log.text = "points scanned: " + numPoints.ToString() + " / 10,000";
            if (numPoints >= 10000)
            {
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 900);
            }
            else
            {
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, (float)numPoints/100.0f * 9.0f);
            }

            //log.text = pointCloud.positions.Value
        }
        else
        {
            //log.text = "trees selected " + currentTree.ToString();
            log.text = "";
        }

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0 && Input.touchCount < 2)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                Touch touch = Input.GetTouch(0);
                if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
                {
                    //Debug.Log("I actually hit something");
                    //Debug.Log(hit.point);
                    //Debug.Log(newPoints.Count);

                    //tree1RadArray = new Vector3[newPoints.Count];
                    //int stupidCounter = 0;

                    //for (int i = 0; i < newPoints.Count; i++)
                    //{
                    //    if (Mathf.Abs(newPoints[i].x - hit.point.x) < minDist && Mathf.Abs(newPoints[i].y - hit.point.y) < 0.1f && Mathf.Abs(newPoints[i].z - hit.point.z) < minDist)
                    //    {
                    //        stupidCounter++;
                    //    }

                    //}
                    //treeTransform1 = new Transform[stupidCounter];
                    //stupidCounter = 0;

                    //for (int i = 0; i < newPoints.Count; i++)
                    //{
                    //    if (Mathf.Abs(newPoints[i].x - hit.point.x) < minDist && Mathf.Abs(newPoints[i].y - hit.point.y) < 0.1f && Mathf.Abs(newPoints[i].z - hit.point.z) < minDist)
                    //    {

                    //        //Debug.Log("go Added");
                    //        tree1Rad.Add(new Vector3(newPoints[i].x, hit.point.y, newPoints[i].z));
                    //        tree1RadArray[i] = new Vector3(newPoints[i].x, hit.point.y, newPoints[i].z);
                    //        GameObject mNewGO = GameObject.Instantiate(go2, new Vector3(newPoints[i].x, hit.point.y, newPoints[i].z), Quaternion.identity);
                    //        treeTransform1[stupidCounter] = (mNewGO.transform);
                    //        GameObject.Destroy(mNewGO);
                    //        stupidCounter++;

                    //    }

                    //}




                    //Debug.Log(treeTransform1[4].position.x.ToString());
                    //FitCircle.FitCircleToCoordinates(ref Mx_c, ref My_c, ref Mr, treeTransform1);
                    //GameObject myCyl = GameObject.Instantiate(center, new Vector3(Mx_c, treeTransform1[0].position.y, My_c), Quaternion.identity);

                    //TreeCyls[currentTree].transform.position = new Vector3(Mx_c, treeTransform1[0].position.y, My_c);
                    //if (Mr > 0.2f) {
                    //    Mr = 0.2f;
                    //}
                    //TreeCyls[currentTree].transform.localScale = new Vector3(Mr * 2, 0.01f, Mr * 2);
                    //+ (cam.transform.forward.normalized * 0.02f)


                    TreeCyls[currentTree].transform.position = hit.collider.transform.position + (cam.transform.forward.normalized * 0.17f);
                    treeimgs[currentTree].SetActive(true);
                    currentTree++;
                    

                    //GameObject.Instantiate(go2, hit.point, Quaternion.identity);

                }
            }
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
            {
                //Debug.Log("I actually hit something");
                //Debug.Log(hit.point);

                //Debug.Log(newPoints.Count);
                tree1RadArray = new Vector3[newPoints.Count];
                int stupidCounter = 0;

                for (int i = 0; i < newPoints.Count; i++)
                {
                    if (Mathf.Abs(newPoints[i].x - hit.point.x) < minDist && Mathf.Abs(newPoints[i].y - hit.point.y) < 0.1f && Mathf.Abs(newPoints[i].z - hit.point.z) < minDist)
                    {
                        stupidCounter++;
                    }

                }
                treeTransform1 = new Transform[stupidCounter];
                stupidCounter = 0;

                for (int i = 0; i < newPoints.Count; i++)
                {
                    if (Mathf.Abs(newPoints[i].x - hit.point.x) < minDist && Mathf.Abs(newPoints[i].y - hit.point.y) < 0.1f && Mathf.Abs(newPoints[i].z - hit.point.z) < minDist)
                    {
                        
                        //Debug.Log("go Added");
                        tree1Rad.Add(new Vector3(newPoints[i].x, hit.point.y, newPoints[i].z));
                        tree1RadArray[i] = new Vector3(newPoints[i].x, hit.point.y, newPoints[i].z);
                        GameObject mNewGO = GameObject.Instantiate(go2, new Vector3(newPoints[i].x, hit.point.y, newPoints[i].z), Quaternion.identity);
                        treeTransform1[stupidCounter] = (mNewGO.transform);
                        GameObject.Destroy(mNewGO);
                        stupidCounter++;
                    }
                }
                //Debug.Log(treeTransform1[4].position.x.ToString());
                FitCircle.FitCircleToCoordinates(ref Mx_c, ref My_c, ref Mr, treeTransform1);
               
                    TreeCyls[currentTree].transform.position = new Vector3(Mx_c, treeTransform1[0].position.y, My_c);
                treeimgs[currentTree].SetActive(true);
                //TreeCyls[currentTree].transform.localScale = new Vector3(Mr * 2, 0.01f, Mr * 2);


                currentTree++;

                //TreeCyls.Add(myCyl);
                //GameObject.Instantiate(go2, hit.point, Quaternion.identity);
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
                    int maxLen = 50;
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
                        int maxLen = 50;
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
