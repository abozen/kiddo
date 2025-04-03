using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarInteractManager : MonoBehaviour
{
    private CarInteract currentCarInteract;
    private Transform currentCarTransform;
    public GameObject interactButton;
    public Transform player;
    public float gettingInSpeed;
    private bool isGettingIn;
    private bool isInCar = false;
    private bool isGettingOut;
    private Vector3 outPosition;
    public PlayerStateManager stateManager;
    private CarController carController;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isGettingIn)
        {
            GetIn();
        }
        else if (isGettingOut)
        {
            GetOut();
        }
    }

    public void OnButtonClick()
    {
        if (!isInCar)
        {
            isGettingIn = true;
            animator.SetTrigger("enterCar");
        }
        else
        {
            isGettingOut = true;
            animator.SetTrigger("exitCar");


        }

        interactButton.SetActive(false);
    }


    private void GetIn()
    {
        player.position = Vector3.MoveTowards(player.position, currentCarTransform.position, gettingInSpeed * Time.deltaTime);
        if (player.position == currentCarTransform.position)
        {
            isGettingIn = false;
            isInCar = true;
            //interactButton.transform.Find("text").GetComponent<Text>().text = "Get Out";
            interactButton.SetActive(true);
            stateManager.SetCarState(carController);
            player.parent = currentCarTransform;
        }
    }
    private void GetOut()
    {
        player.position = Vector3.MoveTowards(player.position, outPosition, gettingInSpeed * Time.deltaTime);
        if (player.position == outPosition)
        {
            isGettingOut = false;
            isInCar = false;
            interactButton.SetActive(true);
            player.parent = null;
        }
    }

    public void TriggerCarInteract(CarInteract carInteract, Transform carTransform, Vector3 outPosition, CarController carController)
    {
        this.carController = carController;
        currentCarInteract = carInteract;
        currentCarTransform = carTransform;
        this.outPosition = outPosition;
        interactButton.SetActive(true);
    }

    public void TriggerExit()
    {
        interactButton.SetActive(false);
        currentCarInteract = null;
        currentCarTransform = null;
    }
}
