using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Score_SceneManager : MonoBehaviour
{
    public GameObject canves;
    public GameObject warnTip;
    public GameObject correctTip;
    public GameObject scorePanel;
    public RectTransform scorePanelInPos;
    public RectTransform scorePanelOutPos;

    public GameObject loginWindow;
    public RectTransform loginWindowInPos;
    public RectTransform loginWindowOutPos;

    public InputField userIdInputField;
    public InputField userPwdInputField;
    public Text userIdText;
    public Text userPwdText;

    public float animStartDelay;
    public float animRepeatRate;
    public float animLength;

    public GameObject title;
    public GameObject btnReturnToMenu;
    public GameObject btnExit;
    public GameObject btnUpload;
    public GameObject[] scoreItems;
    private Text[] scoreTexts;
    private int[] scores;
    private int index;
    private AudioSource audioSourse;
    private AudioClip audioClipSou;
    private bool loadFinished = false;
    //public Vector3 testPos0;
    //public Vector3 testPos1;

    void Start()
    {
        scores = new int[9];
        loginWindow.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        audioSourse = Camera.main.GetComponent<AudioSource>();
        audioClipSou = audioSourse.clip;
        scoreTexts = new Text[scoreItems.Length];
        for (int i = 0; i < scoreItems.Length; i++)
        {
            scoreItems[i].SetActive(false);
            GameObject child = scoreItems[i].transform.Find("scoreText").gameObject;
            scoreTexts[i] = child.GetComponent<Text>();
        }
        ReadAndSetScore();
        btnReturnToMenu.SetActive(false);
        btnExit.SetActive(false);
        title.SetActive(false);
        btnUpload.SetActive(false);
        index = -1;
        InvokeRepeating("ScoreItemMoveInAnim", animStartDelay, animRepeatRate);
    }

    int ReadAndSetScore()
    {
        float sum = 0;
        string preStr = "Lab_";
        string[] str = { "", "/15", "/5", "/15", "/10", "/10", "/10", "/15", "/20" };
        for (int i = 1; i <= 8; i++)
        {
            string key = preStr + i.ToString();
            int score = ScoreManager.ReadScore(key);
            score = score >= 0 ? score : 0;
            scores[i] = score;
            sum += score;
            scoreTexts[i].text = score.ToString() + str[i];
        }
        int finalScore = (int)(Mathf.Round(sum));
        scoreTexts[0].text = finalScore.ToString() + "/100";
        return finalScore;
    }

    void ScoreItemMoveInAnim()
    {
        index++;
        if (index == 0)
        {
            PlayAudio();
            title.SetActive(true);
            iTween.MoveFrom(title, title.transform.position + new Vector3(0, 250, 0), animLength);
        }
        else if (index == 9)                                     //综合评分
        {
            PlayAudio();
            scoreItems[0].SetActive(true);
            iTween.MoveFrom(scoreItems[0], scoreItems[0].transform.position + new Vector3(0, -250, 0), animLength);
        }
        else if (index == 10)                               //返回主菜单按钮和退出按钮
        {
            PlayAudio();
            btnExit.SetActive(true);
            iTween.MoveFrom(btnExit, btnExit.transform.position + new Vector3(0, -250, 0), animLength);
            btnReturnToMenu.SetActive(true);
            iTween.MoveFrom(btnReturnToMenu, btnReturnToMenu.transform.position + new Vector3(0, -250, 0), animLength);
            btnUpload.SetActive(true);
            iTween.MoveFrom(btnUpload, btnUpload.transform.position + new Vector3(0, -250, 0), animLength);
            Invoke("loadSceneFinished", animLength + 0.1f);
        }
        else if (index >= 1 && index <= 4)                  //左边4个
        {
            PlayAudio();
            scoreItems[index].SetActive(true);
            iTween.MoveFrom(scoreItems[index], scoreItems[index].transform.position + new Vector3(-640, 0, 0), animLength);
        }
        else if (index >= 5 && index <= 8)                  //右边4个
        {
            PlayAudio();
            scoreItems[index].SetActive(true);
            iTween.MoveFrom(scoreItems[index], scoreItems[index].transform.position + new Vector3(640, 0, 0), animLength);
        }
    }

    void loadSceneFinished()
    {
        loadFinished = true;
        CancelInvoke("ScoreItemMoveInAnim");
    }

    void PlayAudio()
    {
        audioSourse.clip = audioClipSou;
        audioSourse.Play();
    }

    void Update()
    {
        //testPos1 = scoreItems[1].transform.position;
        //testPos0 = scoreItems[0].transform.position;
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            OnBtnPreScene();
        }

    }

    public void OnBtnReturnToMenu()
    {
        ScoreManager.ClearAll();
        SceneManager.LoadScene("MainMenu");
    }

    public void OnBtnExit()
    {
#if UNITY_EDITOR
        ScoreManager.ClearAll();
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    public void OnBtnPreScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void OnBtnUpload()
    {
        if (!loadFinished)
            return;
        userIdInputField.text = "";
        userPwdInputField.text = "";
        PlayAudio();
        loginWindow.SetActive(true);
        loginWindow.transform.position = loginWindowOutPos.position;
        iTween.MoveTo(loginWindow, loginWindowInPos.position, animLength);
        iTween.MoveTo(scorePanel, scorePanelOutPos.position, animLength);
    }

    public void OnBtnCancelLogin()
    {
        userIdInputField.text = "";
        userPwdInputField.text = "";
        iTween.MoveTo(loginWindow, loginWindowOutPos.position, animLength);
        iTween.MoveTo(scorePanel, scorePanelInPos.position, animLength);
    }

    public void OnBtnDoUpload()
    {
        //iTween.MoveTo(loginWindow, loginWindowOutPos.position, animLength);
        //iTween.MoveTo(scorePanel, scorePanelInPos.position, animLength);
        string userId = userIdText.text;
        string userPwd = userPwdText.text;
        if (userId.Equals("") && userPwd.Equals(""))
        {
            ShowWarnTip("账号和密码不能为空");
        }
        else if (userId.Equals(""))
        {
            ShowWarnTip("账号不能为空");
        }
        else if (userPwd.Equals(""))
        {
            ShowWarnTip("密码不能为空");
        }

        bool hasLogined = false;
        MySQLConnector mySQLConnector = new MySQLConnector();
        if (mySQLConnector.OpenSqlConnection())
        {
            switch (mySQLConnector.Check_Id_Pwd(userId, userPwd))
            {
                case -1:
                    ShowWarnTip("账号不存在");
                    break;
                case -2:
                    ShowWarnTip("密码错误");
                    break;
                case 0:
                    //ShowWarnTip("正确", Color.green);
                    hasLogined = true;
                    break;
            }
            mySQLConnector.CloseConnection();
        }
        else
        {
            ShowWarnTip("连接服务器出错，请检查网络连接");
        }

        if(hasLogined)
        {
            if (mySQLConnector.OpenSqlConnection())
            {
                string sqlStr;
                sqlStr = string.Format("delete from experimentscore where stuId='{0}';", userId);
                mySQLConnector.Exec(sqlStr);
                sqlStr = string.Format("insert into experimentscore(stuId,Lab_1,lab_2,lab_3,lab_4,lab_5,lab_6,lab_7,lab_8) values('{0}',{1},{2},{3},{4},{5},{6},{7},{8})",
                                                userId,
                                                scores[1], scores[2], scores[3], scores[4], scores[5], scores[6], scores[7],scores[8]);
                mySQLConnector.Exec(sqlStr);
                mySQLConnector.CloseConnection();
                ShowCorrectTip("上传成功");
                Invoke("OnBtnCancelLogin", 2f);
            }
            else
            {
                ShowWarnTip("连接服务器出错，请检查网络连接");
            }
        }
        


    }

    void ShowWarnTip(string tipText)
    {
        GameObject obj = Instantiate(warnTip, canves.transform) as GameObject;
        Text tx = obj.GetComponent<Text>();
        tx.text = tipText;
    }

    void ShowCorrectTip(string tipText)
    {
        GameObject obj = Instantiate(correctTip, canves.transform) as GameObject;
        Text tx = obj.GetComponent<Text>();
        tx.text = tipText;
    }
}
