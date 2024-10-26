public interface IAttacker
{
    bool IsInAttackRange();
    bool IsInChaseRange();
    bool IsInSight();
    
    void GainAgro();
    void LoseAgro();

    bool IsAttackCooldown();
    void AttackTarget();
}