#region
using UnityEngine;
using UnityEngine.Custom.Attributes;
#endregion

[Author("Alex"), DisallowMultipleComponent]
public class GameManager : SingletonPersistent<GameManager>
{
}
