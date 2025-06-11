using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IIenteractable
{
    [SerializeField] [TextArea(4, 8)] public string CharacterDescription;

    public void Interact()
    {
        FindObjectOfType<Dialogue>().CharacterInfo = CharacterDescription;
    }
}
