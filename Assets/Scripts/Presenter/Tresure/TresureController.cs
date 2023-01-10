using System.Timers;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TestUnity2DActionGame.Presenter.Tresure
{
    public class TresureController : MonoBehaviour
    {
        private Camera subCamera;
        private Animator animator;
        //[System.NonSerializable] public UnityEngine.Vector3 initPos;

        void Awake()
        {
            subCamera = GameObject.FindGameObjectWithTag("SubCamera").GetComponent<Camera>();
            animator = GetComponent<Animator>();
        }
        // Start is called before the first frame update
        void Start()
        {
            //transform.position = new UnityEngine.Vector3(initPos.x + 0.5f, initPos.y + 5f, 1.0f);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player") {
                animator.SetBool("isCaptured", true);
                for(var t = 0f; t < 1f; t += Time.deltaTime) {}
                Destroy(this.gameObject);
            }
        }
    }
}