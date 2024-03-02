using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "item/Weapon", order = 1)]
public class Weapondata : ScriptableObject
{
    public Sprite icon;
    public Weaponitem Data;
    public AudioClip touch;
    public AnimationClip motion;
    public float cooltime;
}
