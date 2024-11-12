#region
using System.Collections;
using DG.Tweening;
using UnityEngine;
#endregion

public class SquishOnCollision : MonoBehaviour
{
    [SerializeField] GameObject hitVFXPrefab;
    [SerializeField] float squishAmount = 0.8f;
    [SerializeField] float squishDuration = 0.2f;
    [SerializeField] float cooldownDuration = 1f;

    Vector3 originalScale;
    bool isOnCooldown;

    void Start() => originalScale = transform.localScale;

    void OnCollisionEnter(Collision collision)
    {
        if (isOnCooldown) return;
        if (hitVFXPrefab != null) Instantiate(hitVFXPrefab, collision.contacts[0].point, Quaternion.identity);
        
        this.DoForEachPlayer(p => p.Animator.SetTrigger("Collide"));
        SquishEffect();
        StartCoroutine(CooldownCoroutine());
    }

    void SquishEffect() => transform.DOScale(new Vector3(originalScale.x * squishAmount, originalScale.y * 1.2f, originalScale.z * squishAmount), squishDuration)
                                    .OnComplete(() => { transform.DOScale(originalScale, squishDuration); });

    IEnumerator CooldownCoroutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
    }
}
