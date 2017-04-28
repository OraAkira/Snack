using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlGame : MonoBehaviour
{

    void Start()
    {
        if(Application.loadedLevel == 2)
        {
            GameObject Note = GameObject.Find("Canvas/Text");
            Text text = Note.GetComponent<Text>();
            text.text = text.text + "\n你最终的得分为：\n" + Play.Score.ToString();
            Play.Score = 0;
        }
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Return))
        {
            SceneManager.LoadScene(1);
        }
        if(Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void GameStart()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

	public void QuitGame()
    {
        Application.Quit();
    }
}
