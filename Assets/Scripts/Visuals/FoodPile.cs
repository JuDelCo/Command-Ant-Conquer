using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPile : MonoBehaviour
{

    public GameObject foodPrefab;

    int checkFoodAmount = 100;
    public int currentFoodAmount = 100;
    public int maxFoodAmount = 100;

    List<GameObject> foodObjects;

    public float pileRadius = 0.5f;
    public float pileHeight = 0.4f;
    public float deltaAngle = 2 * Mathf.PI / 18;

    public float foodTimer = 0;

    public float delayBetweenFoodSpawns = 10;

    void Start()
    {

        foodObjects = new List<GameObject>();

        for (float i = 0; i < maxFoodAmount; i++)
        {
            float alpha = i / maxFoodAmount;
            float radius = (1 - alpha) * pileRadius;
            float angle = deltaAngle * i;
            float x = Mathf.Sin(angle) * radius;
            float y = alpha * pileHeight;
            float z = Mathf.Cos(angle) * radius;
            Vector3 position = transform.position + new Vector3(x, y, z);
            var newFoodObject = Instantiate(foodPrefab, position, Quaternion.identity);
            newFoodObject.transform.SetParent(transform);
            foodObjects.Add(newFoodObject);
        }
    }

    void Update()
    {
        foodTimer += Time.deltaTime;
        if (foodTimer > delayBetweenFoodSpawns && currentFoodAmount < maxFoodAmount)
        {
			foodTimer = 0f;
            currentFoodAmount += 1;
        }

        if (checkFoodAmount != currentFoodAmount)
        {
            checkFoodAmount = currentFoodAmount;
            UpdateFoodAmount();
        }
    }

    void UpdateFoodAmount()
    {
        for (int i = 0; i < maxFoodAmount; i++)
        {
            foodObjects[i].SetActive(i < currentFoodAmount);
        }
    }
}
