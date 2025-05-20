using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeSwordPart
{
    Pommel,
    Grip,
    Crossguard,
    Blade
}

public enum SetOfSword
{
    Ice,
    Blood,
}

public class SwordPartPickup : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public SwordPartScripteable swordPartScripteableObj;

    private bool isPlayerNearby = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(isPlayerNearby && Input.GetKeyDown(KeyCode.E)) {
            if(UI.Instance.AddObject(swordPartScripteableObj)) Destroy(gameObject);
        }
    }

    public void SetSwordPart(SwordPartScripteable swordPartScripteableObj)
    {
        spriteRenderer.sprite = swordPartScripteableObj.partImageGameObject;
        this.swordPartScripteableObj = swordPartScripteableObj;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = false;
    }
}
