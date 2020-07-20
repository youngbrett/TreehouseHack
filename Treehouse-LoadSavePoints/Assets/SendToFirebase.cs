using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class SendToFirebase : MonoBehaviour
{
    public Button yourButton;
    //GameObject m_GO;
    MeshRenderer m_MR;

    GameObject[] b_GO;
    //MeshRenderer b_MR;

    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);

    }

    void TaskOnClick()
    {
        //myScript2.writeNewUser("myName", "myID", 0.0f, 0.0f,5.0f);
    }
}
