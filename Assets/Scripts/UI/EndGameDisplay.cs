using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using AntWars;
using UnityEngine.SceneManagement;

public class EndGameDisplay : MonoBehaviour
{

    public Image winnerQueen;
    public Text winnerText;

    void Start()
    {
        if (GameManagerExtension.winner != null)
        {
            winnerText.color = GameManagerExtension.winner.behaviour.playerColor;
            winnerQueen.color = GameManagerExtension.winner.behaviour.playerColor;
        }

        transform.DOScale(0, 0.75f).From().SetEase(Ease.OutBack);
        transform.DORotate(Vector3.forward * 180, 0.75f).From().SetEase(Ease.OutBack);

        Invoke("RestartGame", 5);
    }

    void RestartGame()
    {
        GameManagerExtension.winner = null;
        SceneManager.LoadScene("Main Menu");
    }

}
