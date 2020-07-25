using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


public class MoveSideBar : MonoBehaviour
{
    public GameObject homePanel;
    public GameObject loginPanel;
    public Button THLogo;
    public GameObject SideBar;
    int screenWidth = 0;
    bool sideBarOpen = false;
    RectTransform rt;
    int currentPos = 0;
    public Button[] clickables;
    public GameObject svg;

    public Button[] instructions;
    public Button movePlatform;
    public Button hideRailing;

    public Button instructionBtn;
    public GameObject instructionsPanel;

    // Start is called before the first frame update
    void Start()
    {
        movePlatform.onClick.AddListener(movePlatformUp);
        hideRailing.onClick.AddListener(hideRailings);

        instructions[0].onClick.AddListener(showNext0);
        instructions[1].onClick.AddListener(showNext1);
        instructions[2].onClick.AddListener(showNext2);
        instructions[3].onClick.AddListener(showNext3);
        instructions[4].onClick.AddListener(showNext4);

        instructionBtn.onClick.AddListener(showInstructions);

        clickables[0].onClick.AddListener(myAccount);
        clickables[1].onClick.AddListener(resetScene1);
        clickables[2].onClick.AddListener(resetScene2);

        screenWidth = (int)Screen.width;
        Button btn11 = THLogo.GetComponent<Button>();
        btn11.onClick.AddListener(moveSideBar);
        rt = SideBar.GetComponent<RectTransform>();
        currentPos = screenWidth;
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, currentPos * -0.35f, screenWidth);
    }

    GameObject[] FindGameObjectsInLayer(int layer)
    {
        var goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        var goList = new System.Collections.Generic.List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == layer)
            {
                goList.Add(goArray[i]);
            }
        }
        if (goList.Count == 0)
        {
            return null;
        }
        return goList.ToArray();
    }

    void hideRailings() {
        var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Rail(Clone)");
        var objects2 = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Picket(Clone)");

        foreach (var obj in objects) {
            obj.SetActive(false);
        }
        foreach (var obj in objects2)
        {
            obj.SetActive(false);
        }
    }

    void movePlatformUp() {
        GameObject[] th = FindGameObjectsInLayer(8);
        for (int k = 0; k < th.Length; k++) {
            th[k].transform.position = new Vector3(th[k].transform.position.x, th[k].transform.position.y + 0.1f, th[k].transform.position.z);
        }
    }


    void showNext0() {
        instructions[1].gameObject.SetActive(true);
    }
    void showNext1()
    {
        instructions[2].gameObject.SetActive(true);
    }
    void showNext2()
    {
        instructions[3].gameObject.SetActive(true);
    }
    void showNext3()
    {
        instructions[4].gameObject.SetActive(true);
    }
    void showNext4()
    {
        instructions[1].gameObject.SetActive(false);
        instructions[2].gameObject.SetActive(false);
        instructions[3].gameObject.SetActive(false);
        instructions[4].gameObject.SetActive(false);
        instructionsPanel.SetActive(false);

    }

    void showInstructions() {
        instructionsPanel.SetActive(true);
        moveSideBar();
    }

    void myAccount() {
        Debug.Log("dosomething");
    }


    void resetScene1() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void resetScene2()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //loginPanel.SetActive(false);
        svg.transform.localScale = new Vector3(40, 40, 0.0f);
        moveSideBar();
        homePanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (sideBarOpen && currentPos > 0)
        {
            currentPos-= 50;
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, currentPos*-0.35f, screenWidth);
        }
        else if (!sideBarOpen && currentPos < screenWidth)
        {
            currentPos+= 50;
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, currentPos * -0.35f, screenWidth);
        }
    }

    void moveSideBar() {
        if (sideBarOpen)
        {
            sideBarOpen = false;
            //SideBar.SetActive(false);
        }
        else {
            sideBarOpen = true;
            SideBar.SetActive(true);
        }
    }
}
