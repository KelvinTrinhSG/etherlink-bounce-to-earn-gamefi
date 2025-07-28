using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ComboVisualizer : MonoBehaviour
{
    public static ComboVisualizer instance;

    [Header("Particle colors")]
    public Color basicStateColor;
    public Color middleStateColor;
    public Color ultraStateColor;

    [Header("Text colors")]
    public Color normalStateTextColor;
    public Color basicStateTextColor;
    public Color middleStateTextColor;
    public Color ultraStateTextColor;
    public Color breakStateTextColor;

    [Header("References")]
    public GameObject particleObj;
    public GameObject comboChangeParticleObj;
    public ParticleSetuper particleSetuper;
    public ParticleSystem sparksParticle;

    private Transform playerTransform;
    private Transform transformRef;

    private bool isActive;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerTransform = ReferenceController.instance.playerController.transform;
        transformRef = transform;
    }

    public void OnNewState(ComboState state)
    {
        if (state == ComboState.None)
        {
            particleObj.SetActive(false);
            isActive = false;
            return;
        }

        particleObj.SetActive(true);
        comboChangeParticleObj.SetActive(true);
        isActive = true;

        if (state == ComboState.Basic)
        {
            particleSetuper.SetEmissionRate(30f);
            particleSetuper.SetColor(basicStateColor);

            ShapeModule shape = sparksParticle.shape;
            shape.radius = 0.1f;
        }
        else if (state == ComboState.Middle)
        {
            particleSetuper.SetEmissionRate(80f);
            particleSetuper.SetColor(middleStateColor);

            ShapeModule shape = sparksParticle.shape;
            shape.radius = 0.16f;
        }
        else if (state == ComboState.Ultra)
        {
            particleSetuper.SetEmissionRate(120f);
            particleSetuper.SetColor(ultraStateColor);

            ShapeModule shape = sparksParticle.shape;
            shape.radius = 0.25f;
        }
    }

    void LateUpdate()
    {
        if (isActive)
        {
            transformRef.position = playerTransform.position;
        }
    }

    public void HideGraphics()
    {
        isActive = false;
        transformRef.position = new Vector3(-20f, 0f, 0f);
    }

    public void ShowGraphics()
    {
        isActive = true;
    }

    public static Color GetStateColor(ComboState playerComboState)
    {
        if (playerComboState == ComboState.None)
        {
            return  instance.normalStateTextColor;
        }
        else if (playerComboState == ComboState.Basic)
        {
            return instance.basicStateTextColor;
        }
        else if (playerComboState == ComboState.Middle)
        {
            return instance.middleStateTextColor;
        }
        else if (playerComboState == ComboState.Ultra)
        {
            return instance.ultraStateTextColor;
        }
        else // Break state
        {
            return instance.breakStateTextColor;
        }
    }
}