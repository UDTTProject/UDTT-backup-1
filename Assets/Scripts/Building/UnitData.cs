using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "RTS/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Unit Info")]
    public string UnitName;
    public GameObject UnitPrefab;
    public Sprite Icon;
    public int Cost = 100;
    public int Health = 100;
    public int AttackDamage = 10;
}
