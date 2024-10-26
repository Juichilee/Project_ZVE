using UnityEngine;

[CreateAssetMenu(fileName = "DamageData", menuName = "ScriptableObjects/DamageData", order = 1)]
public class DamageData : ScriptableObject
{
    public int BaseDamage;
    public float StaggerDamage;
    public float StunDamage;
    public float SlowDamage;
    public float BleedDamage;
}