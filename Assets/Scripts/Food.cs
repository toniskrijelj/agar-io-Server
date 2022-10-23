using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public static List<Food> foods = new List<Food>();

    public static void AddFood(Food food)
    {
        if(food != null)
        {
            foods.Add(food);
        }
    }

    public static void RemoveFood(Food food)
    {
        foods.Remove(food);
    }

    private void OnEnable()
    {
        AddFood(this);
        ServerSend.SpawnFood(transform.position);
    }

    private void OnDisable()
    {
        RemoveFood(this);
    }
}
