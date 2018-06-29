using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Num : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    #region 给游戏管理者调用的属性
    private bool _isMine;
    public bool IsMine { get { return _isMine; } set { _isMine = value; } }
    private bool _isOpen;
    public bool IsOpen { get { return _isOpen; } set { _isOpen = value; } }
    private bool _dotMine;
    public bool DotMine { get { return _dotMine; } set { _dotMine = value; } }
    private bool _dotOpen;
    public bool DotOpen { get { return _dotOpen; } set { _dotOpen = value; } }

    private int _mineCount;
    public int MineCount { get { return _mineCount; } set { _mineCount = value; } }

    private int _x;
    public int X { get { return _x; } set { _x = value; } }
    private int _y;
    public int Y { get { return _y; } set { _y = value; } }
    #endregion

    [SerializeField]
    private Sprite[] _sprites;
    private Image m_Image;
    private int m_SpriteIndex = 0;

    private int currentSpriteCount = 10;

    private bool clickButtonBool = false;
    private float clickButtonTimer = 0;
    private int clickCount = 0;

    public bool thisNumOver = false;

    private void Awake()
    {
        m_Image = GetComponent<Image>();
        SpriteInGame();
    }

    void Start()
    {

    }

    void Update()
    {
        if (!thisNumOver)
        {
            if (IsOpen && !DotOpen)
                GameState();

            if (clickButtonBool)
            {
                clickButtonTimer += Time.deltaTime;
            }

            if (clickCount >= 2 && clickButtonTimer <= 1f)
            {
                GameController.instance.OpenMyNumAround(X, Y);
            }
            else if (clickCount <= 2 && clickButtonTimer >= 1f)
            {
                clickButtonBool = false;
                clickCount = 0;
                clickButtonTimer = 0;
            }
        }
    }

    /// <summary>
    /// 为了方便知道要索取的图片{
    /// [0 : 未点击的新格子] [1 : 0雷区] ∨ [9 : 8雷区] [10 : 旗子] [11 : 问号] [12 : 赢时成功排的雷] [13 : 未成功排的雷] [14 : 点击到地雷]}
    /// </summary>
    private void SpriteInGame()
    {
        switch (m_SpriteIndex)
        {
            case 0:
                m_Image.sprite = _sprites[0];
                break;
            case 1:
                m_Image.sprite = _sprites[1];
                break;
            case 2:
                m_Image.sprite = _sprites[2];
                break;
            case 3:
                m_Image.sprite = _sprites[3];
                break;
            case 4:
                m_Image.sprite = _sprites[4];
                break;
            case 5:
                m_Image.sprite = _sprites[5];
                break;
            case 6:
                m_Image.sprite = _sprites[6];
                break;
            case 7:
                m_Image.sprite = _sprites[7];
                break;
            case 8:
                m_Image.sprite = _sprites[8];
                break;
            case 9:
                m_Image.sprite = _sprites[9];
                break;
            case 10:
                m_Image.sprite = _sprites[10];
                break;
            case 11:
                m_Image.sprite = _sprites[11];
                break;
            case 12:
                m_Image.sprite = _sprites[12];
                break;
            case 13:
                m_Image.sprite = _sprites[13];
                break;
            case 14:
                m_Image.sprite = _sprites[14];
                break;
            case 15:
                m_Image.sprite = _sprites[15];
                break;
        }
    }

    private void GameState()
    {
        if (IsMine)
        {
            m_SpriteIndex = 14;
            SpriteInGame();
            GameController.instance.GameOver();
        }
        else
        {
            m_SpriteIndex = MineCount + 1;
            SpriteInGame();
        }
        DotOpen = true;
    }



    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!DotOpen && eventData.button == PointerEventData.InputButton.Left)
        {
            if (!GameController.instance.GameStartBool)
            {
                GameController.instance.DotMine(X, Y);
            }
            else
            {
                GameController.instance.OpenNum(X, Y);
                GameController.instance.GameWinBool();
            }
        }
        else if (!IsOpen && eventData.button == PointerEventData.InputButton.Right)
        {
            m_SpriteIndex = currentSpriteCount;
            DotOpen = true;
            SpriteInGame();
            currentSpriteCount++;
            if (currentSpriteCount == 11)
            {
                GameController.instance.haveMine--;
            }
            else if (currentSpriteCount == 12)
            {
                GameController.instance.haveMine++;
                currentSpriteCount = 0;
            }
            else if (currentSpriteCount == 1)
            {
                currentSpriteCount = 10;
                DotOpen = false;
            }
        }

        if (IsOpen)
        {
            clickButtonBool = true;
            clickCount++;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsOpen && !DotOpen)
        {
            if (Input.GetMouseButton(0))
            {
                m_SpriteIndex = 1;
                SpriteInGame();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsOpen && !DotOpen)
        {
            m_SpriteIndex = 0;
            SpriteInGame();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsOpen && !DotOpen && eventData.button == PointerEventData.InputButton.Left)
        {
            m_SpriteIndex = 1;
            SpriteInGame();
        }
    }
}