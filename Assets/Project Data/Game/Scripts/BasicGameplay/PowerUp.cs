using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp
{
    public static int powerUpsCount = System.Enum.GetValues(typeof(PowerUpType)).Length;

    public static Vector2 energyFieldForce = new Vector2(9f, 8f);

    public static float shieldDuration = 6f;
    public static float boosterDuration = 4f;
    public static float coinsMagnetDuration = 4f;

}
