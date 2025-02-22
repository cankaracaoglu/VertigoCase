using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class RotateWheel : MonoBehaviour
{
    [SerializeField] private double speed = 10;
    [SerializeField] private SpriteAtlas atlas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        this.transform.Rotate(Vector3.forward, (float)speed * Time.deltaTime);
    }
}
