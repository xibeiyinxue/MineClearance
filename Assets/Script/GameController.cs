using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public static GameController instance;
    RectTransform m_RectTrans;
    GridLayoutGroup m_Grid;
    float m_UIWidth, m_UIHight;
    float numSizeX, numSizeY;
    Num[,] m_NumMap;
    Num p_Num;

    int row = 9; //行
    int column = 9; //列
    int mine = 9; //炸弹数
    public int haveMine;
    bool _gameStartBool;
    public bool GameStartBool { get { return _gameStartBool; }private set { _gameStartBool = value; } }
    bool _gameOverBool;
    public bool GameOverBool { get { return _gameOverBool; }private set { _gameOverBool = value; } }
    private bool _gameWinBool;
    public bool GameWinBool()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (!m_NumMap[i, j].IsMine && !m_NumMap[i, j].IsOpen)
                    return _gameWinBool = false;
            }
        }
        return _gameWinBool = true;
    }

    int _x,_y;

    private void Awake()
    {
        instance = this;
        m_RectTrans = GetComponent<RectTransform>();
        m_Grid = GetComponent<GridLayoutGroup>();
        p_Num = Resources.Load<Num>("Num");
    }

    void Start () {
        CreatNumMap();
	}
	
	void Update () {
        ReSetMapSize();
    }

    /// <summary>
    /// 生成游戏区域
    /// </summary>
    private void CreatNumMap()
    {
        haveMine = mine;
        m_NumMap = new Num[row, column];
        m_Grid.constraintCount = row;
        
        List<Num> numList = new List<Num>();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                m_NumMap[i, j] = Instantiate(p_Num, transform);
                m_NumMap[i, j].name = "Num[" + i + "," + j + "]";
                m_NumMap[i, j].X = i;
                m_NumMap[i, j].Y = j;
                numList.Add(m_NumMap[i,j]);
            }
        }
        StartCoroutine(CreatMineMap(numList));
        GameWinBool();
    }
    /// <summary>
    /// 地雷生成避免
    /// </summary>
    public void DotMine(int x, int y)
    {
        _x = x;
        _y = y;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int n = x + i;
                int m = y + j;
                if (n >= 0 && n < row && m >= 0 && m < column)
                    m_NumMap[n, m].DotMine = true;
            }
        }
        GameStartBool = true;
    }
    /// <summary>
    /// 生成地雷区域
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreatMineMap(List<Num> numList)
    {
        yield return new WaitUntil(GameStart);
        int mineIndex = 0;
        while(mineIndex < mine)
        {
            int i = Random.Range(0, numList.Count);
            if (numList[i].DotMine)
            {
                numList.Remove(numList[i]);
            }
            else
            {
                numList[i].IsMine = true;
                numList.Remove(numList[i]);
                mineIndex++;
            }
        }
        CheckAllNum();
    }
    /// <summary>
    /// 检测所有 Num
    /// </summary>
    private void CheckAllNum()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                m_NumMap[i, j].MineCount = CheckThisNumMineCount(i,j);
            }
        }
        OpenNum(_x,_y);
    }
    /// <summary>
    /// 检测该 Num 九宫格内的地雷数量
    /// </summary>
    private int CheckThisNumMineCount(int x,int y)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int n = x + i;
                int m = y + j;
                if (n >= 0 && n < row && m >= 0 && m < column)
                {
                    if (m_NumMap[n, m].IsMine)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    /// <summary>
    /// 打开格子
    /// </summary>
    public void OpenNum(int x, int y)
    {
        m_NumMap[x, y].IsOpen = true;
        if (m_NumMap[x,y].MineCount == 0)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!(i == 0 && j == 0))
                    {
                        int n = x + i;
                        int m = y + j;
                        if (n >= 0 && n < row && m >= 0 && m < column && !m_NumMap[n, m].IsOpen && !m_NumMap[n, m].DotOpen)
                        {
                            //m_NumMap[x, y].IsOpen = true;
                            OpenNum(n, m);
                        }
                    }
                }
            }
        }
    }

    public void OpenMyNumAround(int x, int y)
    {
        m_NumMap[x, y].thisNumOver = true;
        int count = 0; //该临时变量记录九宫格内的不能开的格子
        List<Num> thisNum = new List<Num>(); //临时记录该九宫格内没有打开的格子
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (!(i == 0 && j == 0))
                {
                    int n = x + i;
                    int m = j + y;
                    if (n >= 0 && n < row && m >= 0 && m < column)
                    {
                        if (m_NumMap[n, m].DotOpen && !m_NumMap[n,m].IsOpen)
                            count++;

                        else if (!m_NumMap[n,m].IsOpen)
                            thisNum.Add(m_NumMap[n, m]);
                    }
                }
            }
        }
        if (count == m_NumMap[x,y].MineCount)
        {
            foreach (Num item in thisNum)
            {
                OpenNum(item.X, item.Y);
            }
        }
    }

    private bool GameStart()
    {
        return GameStartBool;
    }

    public void GameOver()
    {
        GameOverBool = true;
    }

    public void ReGame(int r,int c,int m)
    {
        foreach (Num item in m_NumMap)
        {
            item.DestroySelf();
        }
        if (r != 0 && c != 0 && m != 0)
        {
            row = r;
            column = c;
            mine = m;
        }
        GameStartBool = false;
        GameOverBool = false;
        CreatNumMap();
    }

    private void ReSetMapSize()
    {
        m_UIWidth = m_RectTrans.rect.width;
        m_UIHight = m_RectTrans.rect.height;

        numSizeX = m_UIWidth / row;
        numSizeY = m_UIHight / column;

        bool Xsize = numSizeX <= numSizeY ? true : false;
        if (Xsize)
            m_Grid.cellSize = new Vector2(numSizeX, numSizeX);
        else
            m_Grid.cellSize = new Vector2(numSizeY, numSizeY);
    }
}
