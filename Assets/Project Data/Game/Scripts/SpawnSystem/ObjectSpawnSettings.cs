using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectSpawnSettings : SpecialSpawnItem
{
    public Vector3 additionalPosition;
    public ObjectType objectType;

    public enum ObjectType
    {
        //Coin, 
        PowerUp,
        WarningSign,
        PortalIn,
        PortalOut,
        PowerUpDisabler,
        CanyonPassedTrigger,
        RelaxLocationEnteranceParticle,
        RelaxLocationBottomCollider,
        LevelBorder,
    }

    public ObjectSpawnSettings(ObjectType type, Vector3 additionalPosition) : base (Type.Object)
    {
        objectType = type;
        this.additionalPosition = additionalPosition;
    }
}