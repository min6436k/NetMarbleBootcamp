using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Food", menuName = "item/Food", order = 1)]
public class Fooddata : ScriptableObject
{
    public Sprite icon;
    public Fooditem Data;
    public AudioClip toush_sound;
    public AudioClip eat_sound;
}