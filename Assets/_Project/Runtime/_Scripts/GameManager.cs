#region
using System;
using UnityEngine;
using UnityEngine.Custom.Attributes;
#endregion

[Author("Alex"), DisallowMultipleComponent]
public class GameManager : SingletonPersistent<GameManager>
{
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
