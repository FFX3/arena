using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    //Inspectior values
    public float speed;
    public float airSpeed;
    public float jumpForce;

    //camera
    public float mouseSensitivityX;
    public float mouseSensitivityY;
    public float upperCameraLockOut;
    public float lowerCameraLockOut;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        UpdatePlayerCameraPitch();
    }

    void MovePlayer(){
        gameObject.GetComponent<Rigidbody>().AddRelativeForce(ClaculateTotalForce());
        UpdatePlayerStance();
        UpdatePlayersRotation();
    }

    //movement scripts
    Vector3 ClaculateTotalForce(){
        Vector3 totalForce = CalculateJumpForce() + CalculatePlayerMovementForce() + CalculateCurrentGravity();
        return totalForce;
    }

    Vector3 CalculatePlayerDirection(){
        Vector3 horizontal = Vector3.right * Input.GetAxisRaw("Horizontal");
        Vector3 vertical = Vector3.forward * Input.GetAxisRaw("Vertical");

        Vector3 direction = Vector3.Normalize(horizontal + vertical);

        return direction;
    }

    Vector3 CalculatePlayerMovementForce(){
        float _speed;
        if(gameObject.GetComponent<State>().isGrounded){
            _speed = speed;
        }else{
            _speed = airSpeed;
        }
        Vector3 PlayerMovementForce = CalculatePlayerDirection() * _speed;
        return PlayerMovementForce;
    }

    Vector3 CalculateJumpForce(){
        if(!gameObject.GetComponent<State>().isGrounded){
            return Vector3.zero;
        }
        if(Input.GetAxisRaw("Jump") > 0f && !gameObject.GetComponent<State>().isJumping){
            gameObject.GetComponent<State>().isJumping = true;
            gameObject.GetComponent<State>().isGrounded = false;
            currentGravity = airborneGravity;
            return Vector3.up * jumpForce;
        }
        return Vector3.zero;
    }
    public float airborneGravity;
    public float groundedGravity;
    float currentGravity = 70;

    Vector3 CalculateCurrentGravity(){
            return Vector3.down * currentGravity;
    }

    //I need to manipulate the timing of the gravity change to allow for consitent Jumps
    public void SetCurrentGravity(string caseString){
        switch(caseString){
            case "Airborne":
                currentGravity = airborneGravity;
            break;
            case "Grounded":
            default:
                currentGravity = groundedGravity;
            break;
        }
    }

    //stance scripts
    void UpdatePlayerStance(){
        if(Input.GetAxisRaw("StanceModifier") > 0f){
            gameObject.GetComponent<State>().ChangeStance("Crouching");
            return;
        }
        gameObject.GetComponent<State>().ChangeStance("Standing");
    }

    //player rotation scripts
    void UpdatePlayersRotation(){
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        Vector3 rotation = new Vector3(0f, Input.GetAxisRaw("Mouse X") * mouseSensitivityX, 0f);

        transform.eulerAngles += rotation;
    }
    void UpdatePlayerCameraPitch(){
        Vector3 currentEulerAngles = transform.Find("Camera").gameObject.transform.rotation.eulerAngles;
        Vector3 rotation = new Vector3(Input.GetAxisRaw("Mouse Y") * -mouseSensitivityY, 0f, 0f);

        //clamp the rotation to stop the player from breaking their back by bending backwards and looking between their legs
        Vector3 newEulerAngles = currentEulerAngles + rotation;

        if(newEulerAngles.x <= 280f && newEulerAngles.x > 80f){
            if(newEulerAngles.x - 180f > 0){
                newEulerAngles.x = 280f;
            }else{
                newEulerAngles.x = 80f;
            }
        }

        transform.Find("Camera").gameObject.transform.eulerAngles = newEulerAngles;
    }

}
