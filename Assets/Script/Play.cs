using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class Play : MonoBehaviour
{
    public static int Score = 0;

    public GameObject Snake;//蛇头
    public GameObject Food;//食物预制物体
    public GameObject BigFood;//积分食物预制物体
    public GameObject Body;//身体预制物体
    public GameObject Wall;//墙壁预制物体
    public GameObject T_Wall;//T形墙预制物体
    public GameObject AllWall;//墙壁父物体
    public const int xlimit=30;//空间的宽
    public const int ylimit=22;//空间的高
    public float speed = 1.0f;//蛇身移动时间间隔

    private List<Transform> BodyList = new List<Transform>();//蛇身列表
    private List<GameObject> WallList = new List<GameObject>();//墙壁列表
    private GameObject tempBigFood;//临时保存积分食物
    private GameObject movingwall;//当前移动的墙壁
    private Vector2 direction;//蛇身运动方向
    private Vector2 WallDirection;//墙壁移动方向
    private bool Is_Add = false;//蛇身是否可添加
    private bool Is_Destoryed = false;//积分食物是否销毁
    private bool Is_Pause = false;//是否暂停
    private float RefreshWall = 10;//墙壁刷新时间

    /// <summary>
    /// 初始化：生成五个墙壁，一秒后唤醒移动线程
    /// </summary>
	void Start ()
    {
        direction = Vector2.up;
        CreatFood();
        for (int i = 0; i < 5; i++)
            CreatWall();
        movingwall = WallList[0];
        InvokeRepeating("CreatWall", 0, RefreshWall);
        InvokeRepeating("Move", 1, speed);
    }

	/// <summary>
    /// 等待输入
    /// 是否越界
    /// </summary>
	void Update ()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            direction = Vector2.up;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            direction = Vector2.down;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            direction = Vector2.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            direction = Vector2.right;

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            CancelInvoke("Move");
            InvokeRepeating("Move", 0, 0.01f);
        }
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            CancelInvoke("Move");
            InvokeRepeating("Move", 0, speed);
        }

        #region 暂停
        if (Input.GetKey(KeyCode.Space))
        {
            if (!Is_Pause)
                Time.timeScale = 0.001f;
            else
                Time.timeScale = 1.0f;
        }
        #endregion

        #region 判断是否越界
        int x = (int)Snake.transform.position.x;
        int y = (int)Snake.transform.position.y;
        int z = (int)Snake.transform.position.z;
        if (x <= -xlimit - 1)
            Snake.transform.position = new Vector3(xlimit, y, z);
        if (x >= xlimit + 1)
            Snake.transform.position = new Vector3(-xlimit, y, z);
        if (y <= -ylimit - 1)
            Snake.transform.position = new Vector3(x, ylimit, z);
        if (y >= ylimit + 1)
            Snake.transform.position = new Vector3(x, -ylimit, z);
        #endregion

        #region 判断墙壁移动
        if (WallList.Count > 10)
        {
            if (movingwall == WallList.Last())
                movingwall = WallList[0];
            Destroy(WallList.Last());
            WallList.RemoveAt(WallList.Count - 1);
            WallDirection = new Vector2(Random.Range(0, 2), Random.Range(0, 2));
        }
        #endregion
    }

    /// <summary>
    /// 蛇头移动代码
    /// </summary>
    void Move()
    {
        Vector3 AddPoint = Snake.transform.position;
        Snake.transform.Translate(direction);
        if (Is_Add)
        {
            GameObject temp = (GameObject)Instantiate(Body, AddPoint, Quaternion.identity);
            BodyList.Insert(0, temp.transform);
            Is_Add = false;
        }
        else
        {
            if(BodyList.Count > 0)
            {
                BodyList.Last().position = AddPoint;
                BodyList.Insert(0, BodyList.Last());
                BodyList.RemoveAt(BodyList.Count - 1);
            }
        }
    }

    /// <summary>
    /// 碰撞事件处理
    /// </summary>
    /// <param name="other">碰撞器</param>
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            CreatFood();
            Score++;
            CancelInvoke("Move");
            speed = speed * 0.95f;
            InvokeRepeating("Move", 0, speed);
            CancelInvoke("CreatWall");
            RefreshWall = RefreshWall * 0.9f;
            InvokeRepeating("CreatWall", 0, RefreshWall);
            Is_Add = true;//蛇身可加长
            //生成大积分食物
            if (BodyList.Count % 5 == 0 && BodyList.Count > 0)
                CreatBigFood();
            if (BodyList.Count > 10)
                InvokeRepeating("CreatT_Wall", 0, 5);
            if(BodyList.Count>15)
                InvokeRepeating("MoveWall", 10, 0.6f);
        }
        else if(other.gameObject.CompareTag("BigFood"))
        {
            Destroy(other.gameObject);
            Is_Destoryed = true;
            Score += 5;
        }
        else
        {
            SceneManager.LoadScene(2);
        }
    }

    /// <summary>
    /// 创建食物
    /// </summary>
    void CreatFood()
    {
        int x = Random.Range(-xlimit, xlimit);
        int y = Random.Range(-ylimit, ylimit);
        Instantiate(Food, new Vector2(x, y), Quaternion.identity);
    }

    /// <summary>
    /// 如果积分奖励食物存在，则销毁食物
    /// </summary>
    void DestoryBigFood()
    {
        if(!Is_Destoryed)
            Destroy(tempBigFood);
    }

    /// <summary>
    /// 创建墙壁
    /// </summary>
    void CreatWall()
    {
        int x = Random.Range(-xlimit + 2, xlimit - 2);
        int y = Random.Range(-ylimit + 2, ylimit - 2);
        Vector2 pos = new Vector3(x, y, 0);
        while(Vector3.Distance(Snake.transform.position , pos) < 3.0f)
        {
            x = Random.Range(-xlimit + 2, xlimit - 2);
            y = Random.Range(-ylimit + 2, ylimit - 2);
            pos = new Vector3(x, y, 0);
        }
        GameObject temp = null;
        if (WallList.Count < 5)
            temp = (GameObject)Instantiate(Wall, pos, Quaternion.Euler(0, 0, (Random.Range(0, 2) * 90)), AllWall.transform);
        else if (WallList.Count > 5)
            temp = (GameObject)Instantiate(Wall, pos, Quaternion.Euler(0, 0, (Random.Range(0, 3) * 45)), AllWall.transform);
        WallList.Insert(0, temp);
    }

    /// <summary>
    /// 创建T形墙
    /// </summary>
    void CreatT_Wall()
    {
        int x = Random.Range(-xlimit + 2, xlimit - 2);
        int y = Random.Range(-ylimit + 2, ylimit - 2);
        Vector2 pos = new Vector3(x, y, 0);
        while (Vector3.Distance(Snake.transform.position, pos) < 3.0f)
        {
            x = Random.Range(-xlimit + 2, xlimit - 2);
            y = Random.Range(-ylimit + 2, ylimit - 2);
            pos = new Vector3(x, y, 0);
        }
        GameObject temp = (GameObject)Instantiate(T_Wall, pos, Quaternion.Euler(0, 0, (Random.Range(0, 3) * 45)), AllWall.transform);
        WallList.Insert(0, temp);
    }

    /// <summary>
    /// 创建积分奖励食物，并在5秒后调用销毁程序
    /// </summary>
    void CreatBigFood()
    {
        int x = Random.Range(-xlimit, xlimit);
        int y = Random.Range(-ylimit, ylimit);
        tempBigFood = (GameObject)Instantiate(BigFood, new Vector2(x, y), Quaternion.identity);
        Invoke("DestoryBigFood", 5);
        Is_Destoryed = false;
    }

    /// <summary>
    /// 移动墙壁
    /// </summary>
    void MoveWall()
    {
        movingwall.transform.Translate(WallDirection);
    }
}
