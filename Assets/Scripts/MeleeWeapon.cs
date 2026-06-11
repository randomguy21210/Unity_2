
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeWeapon : MonoBehaviour, IWeapon
{
    public float attackRange = 0.5f;
    public int attackDamage = 20;
    public float cooldown = 0.5f;
    public float rotationOffset = 0;
    public bool isPlayerWeapon = false;
    public string animationName;
    private Animator anim;
    float timer = 0f;
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
        anim = GetComponent<Animator>();
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
        Swing();
    }
    private void Swing()
    {
        if (timer < cooldown) return;
        
        AudioManager.instance.Play("Hurt");
        if (anim != null) anim.Play(animationName, 0, 0f);
        timer = 0;
        if (isPlayerWeapon)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach(Collider2D enemyCollider in hitEnemies)
            {
                IEnemy enemy = enemyCollider.GetComponent<IEnemy>();
                if (enemy == null) continue;
                enemy.dealDamage(attackDamage);
            }
        } 
        else
        {
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach(Collider2D playerCollider in hitPlayer)
            {
                PlayerController player = playerCollider.GetComponent<PlayerController>();
                if (player == null) continue;
                player.dealDamage(attackDamage);
            }
        }
    }
    public bool canSwing()
    {
        return timer >= cooldown;
    }
    public float getAttackRange()
    {
        return attackRange;
    }
}