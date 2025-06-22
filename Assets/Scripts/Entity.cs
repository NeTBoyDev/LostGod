
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class Entity : MonoBehaviour, IIenteractable,IInvokable<InvokeType>
{
    [SerializeField] [TextArea(4, 8)] public string CharacterDescription;
    [SerializeField] [TextArea(4, 8)] public string StartText;
    [SerializeField] private bool isFemale;
    [SerializeField] private string Name;

    [SerializeField] private bool mayMove;
    [SerializeField] private float moveDelay;

    private Animator Animator;
    private NavMeshAgent Agent;

    private Coroutine walkCoroutine;
    private Coroutine agressiveCoroutine;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();

        if (mayMove)
            walkCoroutine = StartCoroutine(WalkCoroutine());
    }

    public void Pray()
    {
        StartCoroutine(PrayCoroutine());
    }

    private IEnumerator PrayCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.5f));
        Animator.SetBool("Walk",false);
        mayMove = false;
        if(walkCoroutine!= null)
            StopCoroutine(walkCoroutine);
        Animator.SetTrigger("Pray");
        yield return new WaitForSeconds(15f);
        if (mayMove)
            walkCoroutine = StartCoroutine(WalkCoroutine());
    }

    public void Interact()
    {
        var dialogue = FindObjectOfType<Dialogue>();
        dialogue.CharacterInfo = CharacterDescription;
        dialogue.isFemale = isFemale;
        dialogue.CharacterName.text = Name;
        dialogue.Character = this;
    }

    public void Invoke(InvokeType parameter)
    {
        print(parameter.ToString());
    }

    private IEnumerator WalkCoroutine()
    {
        if(agressiveCoroutine != null)
            StopCoroutine(agressiveCoroutine);
        while (mayMove)
        {
            Animator.SetBool("Walk",true);
            var point = transform.position +
                        new Vector3(Random.Range(-15, 15), transform.position.y, Random.Range(-15, 15));
            Agent.destination = point;
            yield return new WaitUntil(() => Agent.remainingDistance < .01);
            Animator.SetBool("Walk",false);
            yield return new WaitForSeconds(moveDelay + Random.Range(-2,5));
        }
    }

    public void SetAgressiveState()
    {
        // Stop the walking coroutine if it's running
        if (walkCoroutine != null)
            StopCoroutine(walkCoroutine);

        // Start the aggressive coroutine
        agressiveCoroutine = StartCoroutine(AggressiveCoroutine());
    }

    private IEnumerator AggressiveCoroutine()
    {
        // Find the player's transform (assuming the player has a tag "Player")
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        while (true)
        {
            Agent.destination = player.position;

            if (Vector3.Distance(transform.position, player.position) < 1f)
            {
                Debug.Log($"{Name} is close to the player!");
            }

            yield return null;
        }
    }
}
