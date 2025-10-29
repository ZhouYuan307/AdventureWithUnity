using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSkillController : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    public float maxSize;
    public float growSpeed;
    public float shrinkSpeed;
    public bool canGrow;
    public bool canShrink;

    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;
    public int amountAttacks = 4;
    public float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKey = new List<GameObject>();

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            DestroyHotKeys();
            cloneAttackReleased = true;
            canCreateHotKeys =false;
        }

        if(cloneAttackTimer < 0 && cloneAttackReleased)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);
            float xOffset;

            xOffset = (Random.Range(0, 100) > 50) ? 2 : -2;
            SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0, 0));

            amountAttacks--;

            if (amountAttacks <= 0)
            {
                canShrink = true;
                cloneAttackReleased = false;
            }
        }


        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize,maxSize), growSpeed*Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);
            if(transform.localScale.x  < 0)
            {
                Destroy(gameObject);
                canShrink = false;
            }
        }
    }

    private void DestroyHotKeys()
    {
        if (createdHotKey.Count <= 0)
        {
            return;
        }
        for(int i = 0; i < createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);
        }
        createdHotKey.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);
        }
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("Not enough hot keys in the list");
            return;
        }

        if (!canCreateHotKeys)
        {
            return;
        }

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdHotKey.Add(newHotKey);
        //choose one hotkey to be assigned to the prefab
        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        BlackHoleHotKeyController newHotKeyScript = newHotKey.GetComponent<BlackHoleHotKeyController>();

        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
