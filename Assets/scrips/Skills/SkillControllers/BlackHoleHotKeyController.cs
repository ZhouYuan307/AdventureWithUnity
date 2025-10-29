using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlackHoleHotKeyController : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;

    private Transform enemyTransform;
    private BlackHoleSkillController blackHole;
    
    public void SetupHotKey(KeyCode _myNewHotKey, Transform _myEnemyTransform, BlackHoleSkillController _myBlackHole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        enemyTransform = _myEnemyTransform;
        blackHole = _myBlackHole;

        myHotKey = _myNewHotKey;
        myText.text = _myNewHotKey.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(myHotKey))
        {
            blackHole.AddEnemyToList(enemyTransform);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
