using UnityEngine;

public interface IDamageable
{
    void Hit(float damage);

    void Heal(float amount);

    void Death();

}
