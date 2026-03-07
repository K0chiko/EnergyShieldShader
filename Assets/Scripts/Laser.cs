using System;
using UnityEngine;

namespace SideViewShooter
{
    // Backward-compatibility wrapper so existing prefabs with LaserController won't break.
    [Obsolete("Use Weapon component instead. This class inherits from Weapon for compatibility.")]
    public class LaserController : Weapon { }
}