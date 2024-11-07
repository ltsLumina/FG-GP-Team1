#region
using System.Collections;
using System.Linq;
using Lumina.Essentials.Modules;
using UnityEngine;
using UnityEngine.Custom.Attributes;
#endregion

/// <summary>
/// Used on any object that has a lifetime and should be destroyed after a certain amount of time.
/// </summary>
internal interface IDestructible
{
    /// <summary>
    /// The lifetime of the object in seconds.
    /// </summary>
    float Lifetime => 60f;

    /// <summary>
    /// Bypass the destruction of this object.
    /// </summary>
    bool Bypass => false;
}

public class GameManagerOLD : MonoBehaviour
{
    IEnumerator Start()
    {
        // Do initial destruction of all ILifetime objects
        var initialDestructibles = Helpers.FindMultiple<MonoBehaviour>().OfType<IDestructible>();
        foreach (var destructible in initialDestructibles)
        {
            if (destructible is MonoBehaviour monoBehaviour)
            {
                if (destructible.Bypass) continue;
                
                Debug.Log($"Destroying {monoBehaviour.gameObject.name} in {destructible.Lifetime} seconds.", monoBehaviour.gameObject);
                Destroy(monoBehaviour.gameObject, destructible.Lifetime);
            }
        }

        // Destroy all ILifetime objects every 60 seconds
        while (true)
        {
            yield return new WaitForSeconds(60);
            var destructibles = Helpers.FindMultiple<MonoBehaviour>().OfType<IDestructible>();

            foreach (var destructible in destructibles)
            {
                if (destructible is MonoBehaviour monoBehaviour)
                {
                    if (destructible.Bypass) continue;
                    
                    Destroy(monoBehaviour.gameObject, destructible.Lifetime);
                    Debug.Log($"Destroying {monoBehaviour.gameObject.name} in {destructible.Lifetime} seconds. as part of global cleanup of lifetime objects.", monoBehaviour.gameObject);
                }
            }
        }
    }

    //[Obsolete("Don't use this. It is only meant for the Alpha build.")]
    public void LoadGame()
    {
        SceneManagerExtended.LoadScene(0);
    }
    
    //[Obsolete("Don't use this. It is only meant for the Alpha build.")]
    public void LoadMainMenu()
    {
        SceneManagerExtended.LoadScene(1);
    }
}