using System.Numerics;
using System.Xml.Serialization;

public interface IEnemy
{
    // TODO: Post Alpha -> Consider adding Jump
    // Movement
    bool GoTo(Vector3 target, float speed);
    bool ReachedTarget();
    void Stop();

    // Death
    void Die();
    void SpawnPickUp();

    // Attack
    bool IsInsight();
    bool ResetTimeOfLastAttack();
    void GainAgro(); // Alert State
    void LoseAgro(); //
    bool IsAttackCooldown();
    void AttackTarget();
}