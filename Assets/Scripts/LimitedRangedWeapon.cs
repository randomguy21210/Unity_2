using UnityEngine;

public class LimitedRangedWeapon: RangedWeapon
{
    public override void Shoot()
    {
        if (Navigation.Map.ContainsKey(Name))
        {
            GameObject p = Navigation.Map[Name];
            Destroy(p);
        }
        base.Shoot();

    }
}