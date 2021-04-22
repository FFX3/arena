using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class State : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CalculateVelocity();
    }

    //drag and momentum logic
    public void CalculateVelocity(){
        Vector3 velocity = gameObject.GetComponent<Rigidbody>().velocity; 
        gameObject.GetComponent<Rigidbody>().velocity = velocity + CalculateXZDrag();
    }

    //is grounded & jumping logic
    public bool isGrounded = false;
    public bool isJumping = false;
    public bool didLooseFooting = false;

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "Ground"){
            isGrounded = true;
            isJumping = false;
            gameObject.GetComponent<PlayerController>().SetCurrentGravity("Grounded");
            PreventBounce();
        }
    }

    private void OnCollisionExit(Collision collision) {
        if(collision.gameObject.tag == "Ground"){
            isGrounded = false;
            isJumping = false;
            if(checkForLostFooting()){
                gameObject.GetComponent<PlayerController>().SetCurrentGravity("Airborne");
            }
        }
    }

    //stance logic
    public string stance = "Standing";
    public void ChangeStance(string newStance){
        stance = newStance;
    }

    public float lostFootingRaycastLenght = 1f;
    private bool checkForLostFooting(){
        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.down, out hit, gameObject.GetComponent<CapsuleCollider>().height/2f + lostFootingRaycastLenght)){
            return false;
        }
        return true;
    }

    //physic "fixes" clean up
    private void PreventBounce(){
        Vector3 velocity = gameObject.GetComponent<Rigidbody>().velocity;
        velocity.y = 0;
        gameObject.GetComponent<Rigidbody>().velocity = velocity;
    }

    public float groundedXZDragCoefficient;
    public float airborneXZDragCoefficient;
    public float crouchingXZDragCoefficient;
    private Vector3 CalculateXZDrag(){
        float coefficient = 0;
        if(isGrounded && stance == "Standing"){
            coefficient = groundedXZDragCoefficient;
        }
        if(isGrounded && stance == "Crouching"){
            coefficient = crouchingXZDragCoefficient;
        }
        if(!isGrounded){
            coefficient = airborneXZDragCoefficient;
        }
        Vector3 velocityInverse = gameObject.GetComponent<Rigidbody>().velocity * - 1;
        Vector3 drag = velocityInverse * coefficient;
        drag.y = 0;
        return drag;
    }
}
