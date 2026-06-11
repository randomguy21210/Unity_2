
using UnityEngine;
using UnityEngine.InputSystem;

public class RangedWeapon : MonoBehaviour, IWeapon
{
    public GameObject projectile;
    public float rotationOffset = 0;
    public float cooldown = 0.5f;
    public bool isPlayerWeapon = false;
    public float timer = 0f;
    private Camera mainCamera;
    
    public string desc;
    public string getDesc(){return desc;}
    public string Name;
    public string getName(){return Name;}
    public int price;
    public int getPrice(){return price;}
    void Start()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        transform.position = transform.parent.position;
        timer += Time.deltaTime;
        if (isPlayerWeapon)
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, Mathf.Abs(mainCamera.transform.position.z - transform.parent.position.z)));
            mousePos.z = transform.parent.position.z;
            Vector2 dir = mousePos - transform.parent.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.Translate(Vector3.right * Navigation.weaponDistance);
            transform.Rotate(0,0,rotationOffset);
        }
        else
        {
            Vector3 playerPos = Navigation.player.transform.position;
            playerPos.z = transform.parent.position.z;
            Vector2 dir = playerPos - transform.parent.position;
            float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,angle);
            transform.Translate(Vector3.right * Navigation.weaponDistance);
            transform.Rotate(0,0,rotationOffset);
        }
    }
    public void Attack()
    {
        Shoot();
    }
    public virtual void Shoot()
    {
        if (timer < cooldown) return;
        timer = 0;
        Projectile p = Instantiate(projectile, transform.position, Quaternion.Euler(0,0,transform.rotation.eulerAngles.z-rotationOffset)).GetComponent<Projectile>();
        p.isPlayerProjectile = isPlayerWeapon;

    }
    public bool canSwing()
    {
        return timer >= cooldown;
    }
    public float getAttackRange()
    {
        return float.MaxValue;
    }
}