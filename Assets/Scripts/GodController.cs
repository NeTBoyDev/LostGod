using System;
using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController.Examples;
using UnityEngine;

public class GodController : MonoBehaviour
{
    [SerializeField] private Transform Head;
    [SerializeField] private Material EyeMaterial;
    private Transform Player;
    [SerializeField] private LayerMask GodLayer;
    private Camera Camera;

    private float EyeSizeDefaultValue;
    private float EyeSizeValue;

    private void Start()
    {
        Camera = FindObjectOfType<ExampleCharacterCamera>().GetComponent<Camera>();
        Player = FindObjectOfType<ExampleCharacterController>().transform;
        EyeSizeDefaultValue = EyeMaterial.GetFloat("_PupilSize");
        EyeSizeValue = EyeSizeDefaultValue;
    }

    private void Update()
    {
        Head.transform.LookAt(Player);

        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward,1000000,GodLayer))
        {
            AddPupilSize();
        }
        else
        {
            RemovePupilSize();
        }
    }

    private void AddPupilSize()
    {
        if (EyeSizeValue < 1)
        {
            EyeSizeValue += Time.deltaTime;
            EyeSizeValue = Mathf.Clamp01(EyeSizeValue);
            EyeMaterial.SetFloat("_PupilSize",EyeSizeValue);
        }
    }
    private void RemovePupilSize()
    {
        if (EyeSizeValue >= EyeSizeDefaultValue)
        {
            EyeSizeValue -= Time.deltaTime;
            EyeSizeValue = Mathf.Clamp01(EyeSizeValue);
            EyeMaterial.SetFloat("_PupilSize",EyeSizeValue);
        }
    }

    
}
