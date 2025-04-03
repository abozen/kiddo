using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public CharacterMovement characterMovement;

    //public CarController carController;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCarState(CarController carController)
    {
        characterMovement.enabled = false;
        carController.enabled = true;
    }
    public void SetWalkState(CarController carController)
    {
        characterMovement.enabled = false;
        carController.enabled = true;
    }
}
