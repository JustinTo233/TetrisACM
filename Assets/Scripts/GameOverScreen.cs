using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private AudioSource GameOverMusic;

    public Text pointsText;

    public void Setup(int score)
    {
        gameObject.SetActive(true);
        pointsText.text = score.ToString() + " POINTS";
        GameOverMusic.Play();
    }

    public void PlayAgainButton(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void MainMenuButton(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
