using UnityEngine;
using System.Collections;
public abstract class Damageable : MonoBehaviour
{
    public abstract int MaxHealth { get; set; }
    public abstract int CurrentHealth { get; set; }
    public bool IsAlive => CurrentHealth > 0;
    public abstract bool IsStunned { get; set; }
    public abstract float StunHealth { get; set; }
    public abstract bool IsStaggered { get; set; }
    public abstract float StaggerHealth { get; set; }
    public abstract bool IsSlowed { get; set; }
    public abstract float SlowHealth { get; set; }
    public abstract bool IsBleed { get; set; }
    public abstract float BleedHealth { get; set; }

    public abstract bool IsInvincible { get; set; }
    public abstract float InvincibilityDuration { get; set; }
    
    public abstract void OnDamage(DamageData damageData);
    public abstract void Die();
    protected IEnumerator InvincibilityFrames()
    {
        IsInvincible = true;
        yield return new WaitForSeconds(InvincibilityDuration);
        IsInvincible = false;
    }
}