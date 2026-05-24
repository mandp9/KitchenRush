using UnityEngine;

public enum DrinkType
{
    SodaBottle,     // 0
    SodaCan,   // 1
    WaterBottle,  // 2
    Yougurt,    // 3
    Cocacola
}

public class Drink : MonoBehaviour
{
    public DrinkType type;

    [HideInInspector]
    public bool isPlaced = false;
}