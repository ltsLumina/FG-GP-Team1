#region
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
#endregion

namespace UnityEngine.Custom.Attributes
{
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
public sealed class AuthorAttribute : Attribute
{
    enum Authors
    {
        Alex,
        Korben,
        Oscar,
        Mateusz,
        Turner,
        Tibet,
    }

    /// <summary>
    ///     A dictionary of authors and their emails.
    /// </summary>
    [UsedImplicitly]
    readonly static Dictionary<Authors, string> authors = new ()
    { { Authors.Alex, "alexander.andrejeff@edu.futuregames.se" },
      { Authors.Korben, "korben.lyall-wilson@edu.futuregames.se" },
      { Authors.Oscar, "oscar.anderlind@edu.futuregames.se" },
      { Authors.Mateusz, "unknown" },
      { Authors.Turner, "unknown" },
      { Authors.Tibet, "unknown" } };

    public string Name { get; }
    public string Email { get; set; }

    /// <summary>
    ///     <para>Author: Alex | alexander.andrejeff@edu.futuregames.se</para>
    ///     <para>Author: Korben | korben.lyall-wilson@edu.futuregames.se</para>
    ///     <para>Author: Oscar | oscar.anderlind@edu.futuregames.se</para>
    ///     <para>Author: Mateusz | unknown</para>
    ///     <para>Author: Turner | unknown</para>
    ///     <para>Author: Tibet | unknown</para>
    /// </summary>
    public AuthorAttribute(string name, string email = "")
    {
        Name = name;
        Email = email;
    }
}
}
