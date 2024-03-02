using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ALLitem", menuName = "item/ALLitem", order = 1)]
public class Allitem : ScriptableObject
{
    public List<Fooddata> ALLFood;
    public List<Weapondata> ALLWeapon;
}
