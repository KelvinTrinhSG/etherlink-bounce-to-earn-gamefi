using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawnSettings : ObjectSpawnSettings
{
    public PowerUpType powerUpType;

    public PowerUpSpawnSettings(PowerUpType type, Vector3 additionaPosition) : base(ObjectType.PowerUp, additionaPosition)
    {
        powerUpType = type;
    }
}
