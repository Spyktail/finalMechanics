using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollisions : MonoBehaviour
{
    [Header("Wall Stuff")]
        public bool isOnWall = false;
        public float walkSpeed;
        public float wallSpeed;
        public float wallGravity;
    
    [Header("Win Condition")]
    public float setWinTimer = 5.0f;
    float winTimer;
    public bool isLevelCompleted;



    public Jumping _jumping;
    public RedbeardController _controller;

    void Start()
    {
        isLevelCompleted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLevelCompleted)
        {
            winTimer -= Time.deltaTime;
            if (winTimer <= 0)
            {
                SceneManager.LoadSceneAsync("WinScene");
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    	{
       		if (hit.gameObject.tag == ("Wall"))
        	{
            	_jumping.gravity = -10f;
           		_controller.moveSpeed = wallSpeed;
            //addd camera tilt here
        	}
        	else if (hit.gameObject.tag != ("Wall"))
        	{
            	_jumping.gravity = -30f;
            	_controller.moveSpeed = walkSpeed;
            //remove camera tilt here
        	}

            if (hit.gameObject.tag == ("Win"))
            {
                isLevelCompleted = true;
                winTimer = setWinTimer;
                
            }

        	if (hit.gameObject.tag == ("Reset"))
        	{
            	SceneManager.LoadScene("MainScene");
        	}
    	}
}
