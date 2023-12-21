using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/GameData/PhoneNumber", menuName = "GameMode2D/PhoneNumber", order = 1)]
public class PhoneNumberSO : ScriptableObject
{
    public string UserName;
    public string PhoneNumber;
}