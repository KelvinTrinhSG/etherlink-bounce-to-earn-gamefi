using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBorderBehaviour : MonoBehaviour
{
    private ColorsPreset preferedColorsPreset;

    public ColorsPreset PreferedColorsPreset
    {
        get { return preferedColorsPreset; }
        set { preferedColorsPreset = value; }
    }
}