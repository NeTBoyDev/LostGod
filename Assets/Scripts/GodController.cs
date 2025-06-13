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
    private bool EyeOpened;

    [SerializeField] private Material GlitchScreenMaterial;

    private AudioSource _source;
    private AudioSource _musicsource;
    [SerializeField] private AudioClip[] _prayClip;
    [SerializeField] private AudioClip _prayMusic;

    private void Start()
    {
        Camera = FindObjectOfType<ExampleCharacterCamera>().GetComponent<Camera>();
        Player = FindObjectOfType<ExampleCharacterController>().transform;
        EyeSizeDefaultValue = EyeMaterial.GetFloat("_PupilSize");
        EyeSizeValue = EyeSizeDefaultValue;
        EyeOpenValue = 0.99f;
        _source = gameObject.AddComponent<AudioSource>();
        _musicsource = gameObject.AddComponent<AudioSource>();
        _source.pitch = 0.7f;

        //StartCoroutine(BlinkCycle());
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
            yield return new WaitForSeconds(Random.Range(45, 60));
            GlitchScreenMaterial.DOFloat(60, "_NoiseAmount", 5f).SetEase(Ease.Linear);
            // Анимация _GlitchStrength
            GlitchScreenMaterial.DOFloat(2, "_GlitchStrength", 5f).SetEase(Ease.Linear);
            OpenEye();
           

            var entitys = FindObjectsOfType<Entity>();
            foreach (var entity in entitys)
            {
                entity.Pray(); 
            }
            
            _musicsource.PlayOneShot(_prayMusic);
            yield return new WaitForSeconds(_prayMusic.length / 3);
            
            var clip = _prayClip[Random.Range(0, _prayClip.Length)];
            _source.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
            
            GlitchScreenMaterial.DOFloat(0, "_NoiseAmount", 5f).SetEase(Ease.Linear);
            // Анимация _GlitchStrength
            GlitchScreenMaterial.DOFloat(0, "_GlitchStrength", 5f).SetEase(Ease.Linear);
            CloseEye();
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
        EyeOpened = true;
        DOTween.To(() => EyeOpenValue, x =>
        {
            EyeOpenValue = x;
            EyeMaterial.SetFloat("_Open", EyeOpenValue);
        }, 0.99f, 2);
    }

    private void CloseEye()
    {
        EyeOpened = false;
        DOTween.To(() => EyeOpenValue, x =>
        {
            EyeOpenValue = x;
            EyeMaterial.SetFloat("_Open", EyeOpenValue);
        }, 0, 2);
    }

    private void OnDestroy()
    {
        GlitchScreenMaterial.SetFloat("_NoiseAmount",0);
        GlitchScreenMaterial.SetFloat("_GlitchStrength",0);
    }
}
