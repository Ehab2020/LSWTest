using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSW.Player;
using LSW.UI;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField]
    private Vector3[] waypoints;
    
    private Animator animator;
    private int waypointIndex = 1;

    private bool isTalkingToPlayer = false;

    public GameObject player;

    public delegate void OnOpenShop();
    public static OnOpenShop onOpenShop;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();


        PlayerController.onNPCInteract += InteractWithPlayer;
        ShopHandler.onClose += StopInteraction;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTalkingToPlayer)
        {
            if (Vector3.Distance(transform.position, waypoints[waypointIndex]) != 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointIndex], 0.5f * Time.deltaTime);
            }
            else
            {
                ChangeDestination();
            }
        }
        else
        {
            if (transform.position.x > player.transform.position.x)
            {
                    animator.SetFloat("Vert", 0f);
                    animator.SetFloat("LastHoriz", -0.5f);   
            }
            else
            {
                    animator.SetFloat("Vert", 0f);
                    animator.SetFloat("LastHoriz", 0.5f);
            }

        }
    }

    void ChangeDestination()
    {
        if (waypointIndex >= waypoints.Length - 1)
        {
            waypointIndex = 0;
            animator.SetFloat("Vert", 0.5f);
        }
        else
        {
            waypointIndex += 1;
            animator.SetFloat("Vert", -0.5f);
        }
    }

    void InteractWithPlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) >= 1.8f) return;
        isTalkingToPlayer = true;
        onOpenShop();
    }

    void StopInteraction()
    {
        isTalkingToPlayer = false;
        animator.SetFloat("LastHoriz", 0f);
        ChangeDestination();
    }
}
