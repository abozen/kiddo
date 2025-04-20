using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileControls : MonoBehaviour
{
    [SerializeField] private RocketController rocketController;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button fireButton;
    
    [SerializeField] private float moveValue = 1f;
    
    private bool isPressingLeft = false;
    private bool isPressingRight = false;
    
    private void Awake()
    {
        if (rocketController == null)
        {
            rocketController = FindObjectOfType<RocketController>();
        }
        
        // Set up button event handlers
        if (leftButton != null)
        {
            EventTrigger leftTrigger = leftButton.gameObject.AddComponent<EventTrigger>();
            
            // Add pointer down event
            EventTrigger.Entry leftPointerDown = new EventTrigger.Entry();
            leftPointerDown.eventID = EventTriggerType.PointerDown;
            leftPointerDown.callback.AddListener((data) => { StartMovingLeft(); });
            leftTrigger.triggers.Add(leftPointerDown);
            
            // Add pointer up event
            EventTrigger.Entry leftPointerUp = new EventTrigger.Entry();
            leftPointerUp.eventID = EventTriggerType.PointerUp;
            leftPointerUp.callback.AddListener((data) => { StopMovingLeft(); });
            leftTrigger.triggers.Add(leftPointerUp);
        }
        
        if (rightButton != null)
        {
            EventTrigger rightTrigger = rightButton.gameObject.AddComponent<EventTrigger>();
            
            // Add pointer down event
            EventTrigger.Entry rightPointerDown = new EventTrigger.Entry();
            rightPointerDown.eventID = EventTriggerType.PointerDown;
            rightPointerDown.callback.AddListener((data) => { StartMovingRight(); });
            rightTrigger.triggers.Add(rightPointerDown);
            
            // Add pointer up event
            EventTrigger.Entry rightPointerUp = new EventTrigger.Entry();
            rightPointerUp.eventID = EventTriggerType.PointerUp;
            rightPointerUp.callback.AddListener((data) => { StopMovingRight(); });
            rightTrigger.triggers.Add(rightPointerUp);
        }
        
        if (fireButton != null)
        {
            fireButton.onClick.AddListener(Fire);
        }
    }
    
    private void Update()
    {
        // If both or neither directional buttons are pressed, stop moving
        if ((isPressingLeft && isPressingRight) || (!isPressingLeft && !isPressingRight))
        {
            rocketController.MoveHorizontal(0);
        }
        else if (isPressingLeft)
        {
            rocketController.MoveHorizontal(-moveValue);
        }
        else if (isPressingRight)
        {
            rocketController.MoveHorizontal(moveValue);
        }
    }
    
    public void StartMovingLeft()
    {
        isPressingLeft = true;
    }
    
    public void StopMovingLeft()
    {
        isPressingLeft = false;
    }
    
    public void StartMovingRight()
    {
        isPressingRight = true;
    }
    
    public void StopMovingRight()
    {
        isPressingRight = false;
    }
    
    public void Fire()
    {
        rocketController.Shoot();
    }
}