using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AntWars;
using DG.Tweening;

public class PlayerFoodScoreDisplay : MonoBehaviour
{

    public LairBehaviour lair;
    public Text text;

    int currentAmount = 0;

    Vector3 startScale;
    public float punch = 1;
    public float duration = 0.5f;

    void Start()
    {
        startScale = transform.GetChild(0).localScale;
        Punch();
    }

    public void SetFoodAmount(int amount)
    {
        text.text = amount.ToString() + "/" + (int)GameConstants.TARGET_FOOD_POINTS;
    }

    void Update()
    {
        if (currentAmount != lair.lairData.foodCount)
        {
            currentAmount = lair.lairData.foodCount;
            SetFoodAmount(currentAmount);
            Punch();
        }
    }

    void Punch()
    {
        transform.GetChild(0).DOPunchScale(Vector3.one * punch, duration, 1).OnComplete(() => transform.GetChild(0).localScale = startScale);
    }

}
