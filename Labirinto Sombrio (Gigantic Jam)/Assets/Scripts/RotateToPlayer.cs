using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToPlayer : MonoBehaviour
{
    private Transform player => GameState.PlayerTransform;
    [SerializeField] private bool flatYRotantion = true;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private int distanceToRotate = 30;
    private Vector3 curWorldPos;
    void Update()
    {
        if(Vector3.Distance(this.transform.position, player.position) < distanceToRotate)
        {
            var playerFlatPos = player.position;
            if(flatYRotantion) playerFlatPos.y = transform.position.y;
            var directionPlayer = playerFlatPos - transform.position;
            var speed = lerpSpeed * Time.deltaTime;
            var newDirection = Vector3.RotateTowards(transform.forward, directionPlayer, speed, 0);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }
}
