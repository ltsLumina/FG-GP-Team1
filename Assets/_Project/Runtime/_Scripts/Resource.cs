using Lumina.Essentials.Attributes;
using UnityEngine;

public interface IGrabbable
{
    public enum Grabbables
    {
        Kelp,
        Battery
    }
}

public class Resource : MonoBehaviour, IGrabbable
{
    [SerializeField] IGrabbable.Grabbables grabbable;
    [SerializeField, ReadOnly] bool grabbed;

    public bool Grabbed => grabbed;

    public void Grab()
    {
        grabbed = true;
    }

}
