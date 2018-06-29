using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class MainUI : MonoBehaviour {
    #region SettingのInputField
    [SerializeField]
    private GameObject m_GameSettingOBJ;
    [SerializeField]
    private InputField row = null;
    [SerializeField]
    private InputField column = null;
    [SerializeField]
    private InputField mine = null;

    private int rowInt32 = 9;
    private int columnInt32 = 9;
    private int mineInt32 = 10;
    #endregion
    #region GameのDownTitle
    private float thisTimer;
    [SerializeField]
    private Text m_Timer;
    [SerializeField]
    private Text m_MineCount;
    #endregion
    #region OnGameOver
    [SerializeField]
    private GameObject m_GameWinObj;
    [SerializeField]
    private Text m_MinTimer;
    [SerializeField]
    private Text m_CurrentTimer;
    [SerializeField]
    private GameObject m_GameOverObj;
    #endregion

    private void Start()
    {
        m_GameWinObj.SetActive(false);
        m_GameOverObj.SetActive(false);
    }

    void Update () {
        if (GameController.instance.GameWinBool())
        {
            OnGameWin();
        }
        if (GameController.instance.GameOverBool)
        {
            OnGameOver();
        }
    }
    /// <summary>
    /// 点击设置的 Button
    /// </summary>
    public void ClickGameSettingButton()
    {
        m_GameSettingOBJ.SetActive(!m_GameSettingOBJ.activeSelf);
    }
    /// <summary>
    /// 点击新游戏的 Button
    /// </summary>
    public void ClickNewGameButton()
    {
        GameController.instance.ReGame(rowInt32,columnInt32,mineInt32);
        m_GameSettingOBJ.SetActive(false);
    }
    /// <summary>
    /// 点击返回游戏的 Button
    /// </summary>
    public void ClickBackGameButton(GameObject obj)
    {
        obj.SetActive(false);
    }
    /// <summary>
    /// 点击再来一局的 Button
    /// </summary>
    public void ClickReGameButton(GameObject obj)
    {
        GameController.instance.ReGame(0, 0, 0);
        m_Timer.text = "";
        obj.SetActive(false);
    }
    /// <summary>
    /// 设置文本的区域限定
    /// </summary>
    private void SetGameController() {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0))
        {
            if (row.text != "")
            {
                row.text = Mathf.Clamp((Convert.ToInt32(row.text)), 9, 25).ToString();
                rowInt32 = Convert.ToInt32(row.text);
            }
            if (column.text != "")
            {
                column.text = Mathf.Clamp((Convert.ToInt32(column.text)), 9, 25).ToString();
                columnInt32 = Convert.ToInt32(column.text);
            }
            if (mine.text != "")
            {
                if (row.text == "" || column.text == "")
                {
                    mine.text = "10";
                    mineInt32 = 10;
                }
                else
                {
                    mine.text = Mathf.Clamp((Convert.ToInt32(mine.text)), 10, (Int32)((Convert.ToSingle(row.text) * (Convert.ToSingle(column.text) * 0.8f)))).ToString();
                    mineInt32 = Convert.ToInt32(mine.text);
                }
            }
        }
    }
    /// <summary>
    /// 游戏胜利时弹出窗口并时间对比
    /// </summary>
    private void OnGameWin()
    {
        m_GameWinObj.SetActive(true);

        m_CurrentTimer.text = m_Timer.text;

        if (PlayerPrefs.GetInt("keepTime") == 0)
        {
            int i = Convert.ToInt32(m_CurrentTimer.text);
            PlayerPrefs.SetInt("keepTime",i);
        }
        else if ((Convert.ToInt32(m_CurrentTimer.text)) < PlayerPrefs.GetInt("keepTime"))
        {
            int i = Convert.ToInt32(m_CurrentTimer.text);
            PlayerPrefs.SetInt("keepTime", i);
        }
        m_MinTimer.text = (PlayerPrefs.GetInt("keepTime")).ToString();
    }
    private void OnGameOver()
    {
        m_GameOverObj.SetActive(true);
        m_MineCount.text = "";
    }

    private void OnGUI()
    {
        row.text = Regex.Replace(row.text, "[^0-9]","");
        column.text = Regex.Replace(column.text, "[^0-9]","");
        mine.text = Regex.Replace(mine.text, "[^0-9]","");

        SetGameController();

        if (!m_GameSettingOBJ.activeSelf)
        {
            row.text = "";
            column.text = "";
            mine.text = "";
        }

        if (!GameController.instance.GameStartBool)
        {
            thisTimer = Time.realtimeSinceStartup;
        }
        else if(GameController.instance.GameStartBool && !GameController.instance.GameWinBool() && !GameController.instance.GameOverBool)
        {
            m_Timer.text = ((int)(Time.realtimeSinceStartup - thisTimer)).ToString();
            m_MineCount.text = GameController.instance.haveMine.ToString();
        }
    }

}
