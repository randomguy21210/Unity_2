using UnityEngine;

public class LimitedProjectile : Projectile
{
    public string Name;
    public override void Die()
    {
        transform.Rotate(new Vector3(0,0,-rotationOffset));
        if (nextProjectile != null) Navigation.Map[Name] = Instantiate(nextProjectile, transform.position, transform.rotation);
        transform.Rotate(new Vector3(0,0,rotationOffset));
        Destroy(gameObject);
    }
}