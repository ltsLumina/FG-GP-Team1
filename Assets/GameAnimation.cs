using UnityEngine;

public class GameAnimation : MonoBehaviour
{
    [SerializeField]
    GameObject ploraTitle;

    public void DestroyPloraTitle()
    {
        Destroy(ploraTitle);
    }
}
