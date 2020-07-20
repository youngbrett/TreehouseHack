using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;

public class myScript2 : MonoBehaviour
{
    public Button yourButton;

    public static DatabaseReference reference;
    int counter = 0;
    int voter = 0;

    void Start()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://u2020-test.firebaseio.com/");

        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        //writeNewUser("default", "block0", 4.0f, 1.0f, 3.0f);
        //WriteNewScore("myName", 4);

        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }
    private void Update()
    {
        
        counter++;
        if (counter % 10 == 0) {
            //Debug.Log(counter);
            //myScript2.writeNewUser("myName", "myID", CycleBuildings.count);
        }

        
    }

    void TaskOnClick()
    {
        writeNewUser("default", "block"+voter, 0.0f, 2.0f, 1.0f);
        voter++;
    }

    public static void writeNewUser(string userId, string name, float numX, float numY, float numZ)
    {
        User1 user = new User1(name, numX, numY, numZ);
        string json = JsonUtility.ToJson(user);

        //reference.Child("users").Child(userId).SetRawJsonValueAsync(json);
        reference.Child("blocks").Child(userId).Child(name).SetRawJsonValueAsync(json);
    }

   

    
}

public class User1
{
    public string uid;
    public float posX;
    public float posY;
    public float posZ;


    public User1()
    {
    }

    public User1(string username, float numX, float numY, float numZ)
    {
        this.uid = username;
        this.posX = numX;
        this.posY = numY;
        this.posZ = numZ;
    }
}