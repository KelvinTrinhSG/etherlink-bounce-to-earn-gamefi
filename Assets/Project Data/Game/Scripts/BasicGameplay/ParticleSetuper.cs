using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

// Setups automaticaly colors for particles
public class ParticleSetuper : MonoBehaviour
{
    public bool autoColorSetup = true;
    public bool autoComboColorSetup = true;
    public ParticleSystem[] particlesWithChangableColor;
    public ParticleSystem[] particlesWithChangableEmission;

    public ObjectType type;

    //private bool isInitialized = false;
    private bool isFirstEnable = true;

    public enum ObjectType
    {
        PowerUp = 0,
        Portal = 1,
        RelaxZonePortal = 2,
        PlayerTrail = 3,
    }

    public void OnEnable()
    {
        if (autoColorSetup && (!isFirstEnable)) // time since level loaded == 0 means that object is created by pool and cached - dont need to update values
        {
            //isInitialized = true;

            if (particlesWithChangableColor.IsNullOrEmpty())
                return;

            Color colorToSet = GetColorByType();

            SetColor(colorToSet);
        }

        isFirstEnable = false;
    }

    public void SetColor(Color color)
    {
        for (int i = 0; i < particlesWithChangableColor.Length; i++)
        {
            ParticleSystem.MainModule mainModule = particlesWithChangableColor[i].main;
            mainModule.startColor = color;

            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particlesWithChangableColor[i].colorOverLifetime;

            if (colorOverLifetime.enabled)
            {
                for (int j = 0; j < colorOverLifetime.color.gradient.colorKeys.Length; j++)
                {
                    colorOverLifetime.color.gradient.colorKeys[j].color = color;
                }

                Gradient gradient = new Gradient();
                List<GradientColorKey> colorKeys = new List<GradientColorKey>();
                List<GradientAlphaKey> alphaKeys = new List<GradientAlphaKey>();

                for (int j = 0; j < colorOverLifetime.color.gradient.colorKeys.Length; j++)
                {
                    colorKeys.Add(new GradientColorKey(color, colorOverLifetime.color.gradientMax.colorKeys[j].time));
                }

                for (int j = 0; j < colorOverLifetime.color.gradient.alphaKeys.Length; j++)
                {
                    alphaKeys.Add(new GradientAlphaKey(colorOverLifetime.color.gradientMax.alphaKeys[j].alpha, colorOverLifetime.color.gradientMax.alphaKeys[j].time));
                }

                gradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
                colorOverLifetime.color = gradient;
            }
        }
    }

    public void SetEmissionRate(float emissionRate)
    {
        for (int i = 0; i < particlesWithChangableEmission.Length; i++)
        {
            ParticleSystem.EmissionModule emission = particlesWithChangableEmission[i].emission;
            emission.rateOverTime = emissionRate;
        }
    }

    private Color GetColorByType()
    {
        if (type == ObjectType.PowerUp)
        {
            if (autoComboColorSetup && PlayerController.ComboState != ComboState.None)
            {
                return ComboVisualizer.GetStateColor(PlayerController.ComboState);
            }
            else
            {
                return ColorsController.CurrentPreset.powerUpsMainColor;
            }
        }
        else if (type == ObjectType.Portal)
        {
            return ColorsController.CurrentPreset.portalColor;
        }
        else // player trail
        {
            return GameController.instance.CurentCharacter.hitEffectColor;
        }
    }
}