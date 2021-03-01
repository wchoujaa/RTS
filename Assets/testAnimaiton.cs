using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAnimaiton : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("speed", 1.2f);
    }
}
