using UnityEngine;
using DG.Tweening;
using System.Collections;

public class SquishOnCollision : MonoBehaviour
{
    public GameObject hitVFXPrefab;
    public float squishAmount = 0.8f;
    public float squishDuration = 0.2f;
    public float cooldownDuration = 1f;

    private Vector3 originalScale;
    private bool isOnCooldown = false;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isOnCooldown) return;
        if (hitVFXPrefab != null)
        {
            Instantiate(hitVFXPrefab, collision.contacts[0].point, Quaternion.identity);
        }

        SquishEffect();
        StartCoroutine(CooldownCoroutine());
    }

    private void SquishEffect()
    {
        transform.DOScale(new Vector3(originalScale.x * squishAmount, originalScale.y * 1.2f, originalScale.z * squishAmount), squishDuration)
            .OnComplete(() =>
            {
                transform.DOScale(originalScale, squishDuration);
            });
    }

    private IEnumerator CooldownCoroutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
    }
}
