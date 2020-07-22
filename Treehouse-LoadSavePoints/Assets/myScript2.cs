using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;

public class myScript2 : MonoBehaviour
{
    public Button submitLoginButton;
    public Button signUp;
    public InputField Username_field;
    public GameObject loginPanel;
    public GameObject homePanel;
    int mScale = 40;
    int nScale = 0;
    bool entered = false;
    public Text loginSuccess;
    public Text errorMsgTxt;
    public Text welcome;

    public GameObject lightGreenPanel;

    public string userName = "";


    bool canGoForwards = false;
    string displayString = "loading...";


    public Button scanTrees;
    public GameObject ARSession;
    public GameObject startScanning;

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

        Button btn = submitLoginButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);

        Button btn1 = signUp.GetComponent<Button>();
        btn1.onClick.AddListener(checkLogin);

        Button btn2 = scanTrees.GetComponent<Button>();
        btn2.onClick.AddListener(startTreeScan);

        //Button btn3 = startScanning.GetComponent<Button>();
        //btn3.onClick.AddListener(startScan);
    }
    private void Update()
    {

        if (entered && mScale > 1) {
            mScale--;
            gameObject.transform.localScale = new Vector3(mScale, mScale, 0.0f);
        }

        //Debug.Log(displayString);

        if (canGoForwards) {
            if (nScale < 200)
            {
                nScale++;
            }
            else {
                canGoForwards = false;
            }
            
        }
        if (nScale > 100 && mScale < 40) {
            entered = false;
            mScale++;
            gameObject.transform.localScale = new Vector3(mScale, mScale, 0.0f);
        }
        if (nScale > 150 && nScale < 190) {

            homePanel.SetActive(true);
        }
        //Debug.Log(nScale);

    }

    //void startScan() {
    //    startScanning.SetActive(false);
    //    //ARSession.GetComponent<ARPointCloudManager>().enabled = true;
    //    ARSession.GetComponent<SwitchPointCloudVisualizationMode>().enabled = true;

    //}

    void startTreeScan() {
        entered = true;
        canGoForwards = false;
        nScale = 0;
        mScale = 40;
        //ARSession.GetComponent<SwitchPointCloudVisualizationMode>().enabled = true;
        ARSession.SetActive(true);
        homePanel.SetActive(false);
        lightGreenPanel.SetActive(false);
        //startScanning.SetActive(true);

    }

    void checkLogin() {
        string userID = Username_field.text.ToString();
        //Debug.Log(userID);

        if (userID != "")
        {
            writeNewUser("default", userID, "block", 2.0f, 1.0f);
            displayString = "you are now registered as " + userID.ToUpper();
            loginSuccess.text = displayString;
            canGoForwards = true;
            entered = true;
            loginPanel.SetActive(false);
            userName = userID;
            welcome.text = userID.ToUpper();
        }

        


        //  FirebaseDatabase.DefaultInstance
        //.GetReference("blocks/default/block0")
        //.GetValueAsync().ContinueWith(task =>
        //{
        //    if (task.IsFaulted)
        //    {
        //          // Handle the error...
        //      }
        //    else if (task.IsCompleted)
        //    {
        //        DataSnapshot snapshot = task.Result;
        //        Debug.Log(snapshot.Children);
        //        //JsonUtility.FromJson<User1>(task.Result);
        //    }
        //});

        
    }


    void TaskOnClick()
    {
        string userID = Username_field.text.ToString();
        Debug.Log(userID);
        if (userID != "")
        {
            FirebaseDatabase.DefaultInstance.RootReference.Child("user").GetValueAsync().ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    Debug.Log("FirebaseDatabaseError: IsCanceled: " + t.Exception);
                    return;
                }

                if (t.IsFaulted)
                {
                    Debug.Log("FirebaseDatabaseError: IsFaulted:" + t.Exception);
                    return;
                }

                DataSnapshot snapshot = t.Result;
                Debug.Log(snapshot.HasChild(userID));
                if (snapshot.HasChild(userID))
                {
                    displayString = "you are now loged in as " + userID.ToUpper();
                    loginSuccess.text = displayString;
                    Debug.Log(snapshot.Child(userID).GetRawJsonValue());
                    canGoForwards = true;
                    entered = true;
                    loginPanel.SetActive(false);
                    userName = userID;
                    welcome.text = userID.ToUpper();
                }
                else {
                    string errorMsg = "That username is not registered. To register this name, press SIGN UP.";
                    errorMsgTxt.text = errorMsg;
                    var hey = errorMsgTxt.GetComponent<Text>();
                    hey.enabled = true;
                }

                //User1 user = JsonUtility.FromJson<User1>(snapshot.GetRawJsonValue());
                //Debug.Log(user.ToString());
            });
            
        }
        else {
            displayString = "you are now loged in as GUEST";
            loginSuccess.text = displayString;
            canGoForwards = true;

            entered = true;
            loginPanel.SetActive(false);
            userName = userID;
            welcome.text = userID.ToUpper();
        }
        

    }

    public static void writeNewUser(string userId, string name, string numX, float numY, float numZ)
    {
        User1 user = new User1(name, numX, numY, numZ);
        string json = JsonUtility.ToJson(user);

        //reference.Child("users").Child(userId).SetRawJsonValueAsync(json);
        reference.Child("user").Child(name).SetRawJsonValueAsync(json);
    }

   

    
}

public class User1
{
    public string uid;
    public string posX;
    public float posY;
    public float posZ;


    public User1()
    {
    }

    public User1(string username, string numX, float numY, float numZ)
    {
        this.uid = username;
        this.posX = numX;
        this.posY = numY;
        this.posZ = numZ;
    }
}