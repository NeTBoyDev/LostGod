using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class AudioList : MonoBehaviour
{
   [SerializeField] private List<AudioClip> clips;
   private AudioSource _source;

   private void Awake()
   {
      _source = GetComponent<AudioSource>();
      StartCoroutine(SoundLoop());
   }

   private IEnumerator SoundLoop()
   {
      while (true)
      {
         var clip = clips[Random.Range(0, clips.Count)];
         _source.PlayOneShot(clip);
         yield return new WaitForSeconds(clip.length);
      }
   }
}
