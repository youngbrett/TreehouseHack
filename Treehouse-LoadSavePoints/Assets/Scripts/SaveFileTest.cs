using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Unity.Jobs;
using UnityEngine.XR;


//namespace UnityEngine.XR.ARFoundation

public class SaveFileTest : MonoBehaviour
{
    // Start is called before the first frame update
    public Button btn;
    string path = "";
    string folderName = "xyz";
    //string AndroidText = "androidtxt";

    void Start()
    {
        btn.onClick.AddListener(SaveFile);

        path = Application.persistentDataPath;
        if (!Directory.Exists(path + folderName)) {
            Directory.CreateDirectory(path + folderName);
        } SaveFile(); 

       

        // You can find the created folder in your phone's memory
    }
    void SaveFile() {

        string fileName = "androidtxt";
            //ParticleSystem.Particle[] particles = new ParticleSystem.Particle[m_PointCloudData.Length];
            //int index = 0;
            string str = "";
        var trackableCollection = "hello";

        foreach (var pointCloud in trackableCollection)
            {
            // Collect the points in the point cloud
            //if (!pointCloud.positions.HasValue)
            //    continue;

            //var points = pointCloud.positions.Value;
            //str += points.x + "," + points.y + "," + points.z + "|";

            }
            Debug.Log(str);
        
        System.IO.File.WriteAllText(path + folderName + "/" + fileName + ".txt", "Congrats! It's saved");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
