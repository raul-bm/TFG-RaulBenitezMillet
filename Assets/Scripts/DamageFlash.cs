using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float flashTime = 0.25f;

    private SpriteRenderer spriteRenderer;
    private Material material;

    private Coroutine _damageFlashCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        Init();
    }

    private void Init()
    {
        material = spriteRenderer.material;
    }

    public void CallDamageFlash()
    {
        _damageFlashCoroutine = StartCoroutine(DamageFlasher());
    }

    private IEnumerator DamageFlasher()
    {
        // set the color
        material.SetColor("_FlashColor", _flashColor);

        // lerp the flash amount
        float currentFlashAmout = 0f;
        float elapsedTime = 0f;
        while(elapsedTime < flashTime)
        {
            // iterate elapsedTime
            elapsedTime += Time.deltaTime;

            // lerp the flash amount
            currentFlashAmout = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));
            material.SetFloat("_FlashAmount", currentFlashAmout);

            yield return null;
        }
    }
}
