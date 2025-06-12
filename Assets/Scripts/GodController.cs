using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KinematicCharacterController.Examples;
using UnityEngine;
using Random = UnityEngine.Random;

public class GodController : MonoBehaviour
{
    [SerializeField] private Transform Head;
    [SerializeField] private Material EyeMaterial;
    private Transform Player;
    [SerializeField] private LayerMask GodLayer;
    private Camera Camera;

    private float EyeSizeDefaultValue;
    private float EyeSizeValue;
    private float EyeOpenValue;

    private AudioSource _source;
    [SerializeField] private AudioClip _prayClip;

    private void Start()
    {
        Camera = FindObjectOfType<ExampleCharacterCamera>().GetComponent<Camera>();
        Player = FindObjectOfType<ExampleCharacterController>().transform;
        EyeSizeDefaultValue = EyeMaterial.GetFloat("_PupilSize");
        EyeSizeValue = EyeSizeDefaultValue;
        EyeOpenValue = 0.99f;
        _source = gameObject.AddComponent<AudioSource>();
        _source.pitch = 0.5f;

        StartCoroutine(BlinkCycle());
        StartCoroutine(PrayRequestLoop());
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

    private IEnumerator PrayRequestLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(15, 30));
            _source.PlayOneShot(_prayClip);
            yield return new WaitForSeconds(_prayClip.length);
        }
    }

    private IEnumerator BlinkCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(7, 10));
            CloseEye();
            yield return new WaitForSeconds(1);
            OpenEye();
        }
    }

    private void OpenEye()
    {
        DOTween.To(() => EyeOpenValue, x =>
        {
            EyeOpenValue = x;
            EyeMaterial.SetFloat("_Open", EyeOpenValue);
        }, 0.99f, 1);
    }

    private void CloseEye()
    {
        DOTween.To(() => EyeOpenValue, x =>
        {
            EyeOpenValue = x;
            EyeMaterial.SetFloat("_Open", EyeOpenValue);
        }, 0, 1);
    }
    
}
