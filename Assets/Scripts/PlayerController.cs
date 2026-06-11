using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    InputAction moveAction;
    public float moveSpeed = 8;
    float timer = 0f;
    float timer2 = 0f;
    public Tilemap tilemap;
    private int [,] visited = new int[Navigation.mapx,Navigation.mapy];
    public int maxhealth;
    private int health = 100;
    private GameObject activeWeaponGameObject;
    private IWeapon activeWeapon;
    public GameObject[] weaponPrefabs;
    private int curHotbarSlot = 0;
    private int[] hotbarWeapons = new int[3];
    private int weaponCount = 1;
    public int[] prevWeapon;
    public bool[] hasWeapon;
    public int coins;

    public InputAction AttackAction;
    public InputAction[] Hotbar;
    public Slider healthBar;
    public Image[] weaponSprites;
    public TextMeshProUGUI[] weaponDesc;
    public TextMeshProUGUI[] weaponNames;
    public GameObject UI;
    public GameObject Shop;
    public int waves = 0;
    private void OnEnable()
    {
        AttackAction.Enable();
        AttackAction.performed += OnAttackPressed;
        Hotbar[0].Enable();
        Hotbar[1].Enable();
        Hotbar[2].Enable();
        Hotbar[0].performed += SwitchHotbar0;
        Hotbar[1].performed += SwitchHotbar1;
        Hotbar[2].performed += SwitchHotbar2;
    }
    private void OnDisable()
    {
        AttackAction.performed -= OnAttackPressed;
        AttackAction.Disable();
        Hotbar[0].Disable();
        Hotbar[1].Disable();
        Hotbar[2].Disable();
        Hotbar[0].performed -= SwitchHotbar0;
        Hotbar[1].performed -= SwitchHotbar1;
        Hotbar[2].performed -= SwitchHotbar2;
    }
    private void OnAttackPressed(InputAction.CallbackContext context)
    {
        activeWeapon.Attack();
    }

    void Start()
    {
        UI.SetActive(true);
        Shop.SetActive(false);
        health = maxhealth;
        healthBar.maxValue = maxhealth;
        healthBar.value = maxhealth;
        hotbarWeapons[0] = 0;
        hasWeapon[0] = true;
        activeWeaponGameObject = Instantiate(weaponPrefabs[0], gameObject.GetComponent<Transform>());
        activeWeapon = activeWeaponGameObject.GetComponent<IWeapon>();
        Navigation.player = this;
        moveAction = InputSystem.actions.FindAction("Move");
        BoundsInt bounds = tilemap.cellBounds;
        foreach(Vector3Int cellPosition in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(cellPosition))
            {
                int rX = cellPosition.x*2;
                int rY = cellPosition.y*2;
                visited[rX,rY]=1;
                visited[rX+1,rY]=1;
                visited[rX,rY+1]=1;
                visited[rX+1,rY+1]=1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 moveValue = moveAction.ReadValue<Vector2>().normalized;
        transform.Translate(moveValue * moveSpeed * Time.deltaTime);
        timer += Time.deltaTime;
        timer2 += Time.deltaTime;
        if (timer > 0.1f)
        {
            timer -= 0.1f;
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            int [,] directions = new int[Navigation.mapx,Navigation.mapy];
            int [,] cloneVisited = (int[,])visited.Clone();
            directions[(int)Math.Floor(transform.position.x/Navigation.tilesize),(int)Math.Floor(transform.position.y/Navigation.tilesize)] = 0;
            cloneVisited[(int)Math.Floor(transform.position.x/Navigation.tilesize),(int)Math.Floor(transform.position.y/Navigation.tilesize)] = 1;
            queue.Enqueue(new Vector2Int((int)Math.Floor(transform.position.x/Navigation.tilesize), (int)Math.Floor(transform.position.y/Navigation.tilesize)));
            while(queue.Count > 0)
            {
                Vector2Int t = queue.Peek();
                queue.Dequeue();
                for(int i = 0; i < 4; i++)
                {
                    int a = Navigation.diri[i].x;
                    int b = Navigation.diri[i].y;
                    if (!(t.x + a >= 0 && t.x + a <= Navigation.mapx-1 && t.y + b >= 0 && t.y + b <= Navigation.mapy-1)) continue;
                    if (cloneVisited[t.x+a,t.y+b] == 1) continue;
                    cloneVisited[t.x+a,t.y+b] = 1;
                    directions[t.x+a,t.y+b] = i+1;
                    queue.Enqueue(new Vector2Int(t.x+a,t.y+b));
                }

            }
            Navigation.directions = directions;
        }
        if (timer2 >= Navigation.waveLength)
        {
            timer2 -= Navigation.waveLength;
            waves++;
            openShop();
        }
    }
    public void dealDamage(int damage)
    {
        health -= damage;
        healthBar.value = health;
        if (health <= 0) SceneManager.LoadScene(1);
    }
    public void openShop()
    {
        UI.SetActive(false);
        Shop.SetActive(true);
        Time.timeScale = 0f;
        initShop();
    }
    public void closeShop()
    {
        UI.SetActive(true);
        Shop.SetActive(false);
        Time.timeScale = 1f;
    }
    private List<int> shopWeapons = new List<int>();
    public void initShop()
    {
        shopWeapons.Clear();
        List<int> validWeapons = new List<int>();
        for(int i = 0; i < hasWeapon.Length; i++)
        {
            if (!hasWeapon[i] && (prevWeapon[i] == -1 || hasWeapon[prevWeapon[i]]))
            {
                validWeapons.Add(i);
            }
        }
        for(int i = 0; i < 3; i++)
        {
            int randi = UnityEngine.Random.Range(0, validWeapons.Count);
            int w = validWeapons[randi];
            shopWeapons.Add(w);
            weaponDesc[i].text = weaponPrefabs[w].GetComponent<IWeapon>().getDesc();
            weaponSprites[i].sprite = weaponPrefabs[w].GetComponent<SpriteRenderer>().sprite;
            weaponNames[i].text = weaponPrefabs[w].GetComponent<IWeapon>().getName();
        }
    }
    public void buyWeapon(int index)
    {
        if (weaponPrefabs[shopWeapons[index]].GetComponent<IWeapon>().getPrice() > coins) return;
        if (weaponCount == 3 && prevWeapon[shopWeapons[index]] == -1) return;
        coins -= weaponPrefabs[shopWeapons[index]].GetComponent<IWeapon>().getPrice();
        if (prevWeapon[shopWeapons[index]] == -1) 
        {
            hotbarWeapons[weaponCount] = shopWeapons[index];
            weaponCount++;
            SwitchHotbar(weaponCount-1);
        }
        else
        {
            int hotbarSlot = Array.IndexOf(hotbarWeapons, prevWeapon[shopWeapons[index]]);
            hotbarWeapons[hotbarSlot] = shopWeapons[index];
            Destroy(activeWeaponGameObject);
            activeWeaponGameObject = Instantiate(weaponPrefabs[shopWeapons[index]], gameObject.GetComponent<Transform>());
            activeWeapon = activeWeaponGameObject.GetComponent<IWeapon>();
            curHotbarSlot = hotbarSlot;
        }
        hasWeapon[shopWeapons[index]] = true;
    }
    public void buyWeapon0(){buyWeapon(0);}
    public void buyWeapon1(){buyWeapon(1);}
    public void buyWeapon2(){buyWeapon(2);}
    public void SwitchHotbar(int index)
    {
        if (index == curHotbarSlot) return;
        if (index >= weaponCount) return;
        curHotbarSlot = index;
        Destroy(activeWeaponGameObject);
        activeWeaponGameObject = Instantiate(weaponPrefabs[hotbarWeapons[index]], gameObject.GetComponent<Transform>());
        activeWeapon = activeWeaponGameObject.GetComponent<IWeapon>();
    }
    public void SwitchHotbar0(InputAction.CallbackContext context){SwitchHotbar(0);}
    public void SwitchHotbar1(InputAction.CallbackContext context){SwitchHotbar(1);}
    public void SwitchHotbar2(InputAction.CallbackContext context){SwitchHotbar(2);}
}
