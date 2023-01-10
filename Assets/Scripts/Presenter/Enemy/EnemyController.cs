using System.Runtime.InteropServices;
using System.Timers;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UniRx;
using TestUnity2DActionGame.Presenter.Common;
using TestUnity2DActionGame.Domain.Enemy;

namespace TestUnity2DActionGame.Presenter.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        UnityEngine.Vector3 mousePos, worldPos;

        private Camera subCamera;
        private Rigidbody2D rigid2D;
        private Animator animator;

        UnityEngine.Vector3 pos0, pos1, pos;
        public float sensitivity = 1f;  // 感度調節用

        public ReactiveProperty<EnemyState> enemyState { get; set; }
        private MoveKind movekind;
        IEnumerator move;

        // LayerMask targetLayerMask = 1 << 9 | 1 << 10;
        LayerMask blockLayer = 1 << 9;
        LayerMask targetLayerMask = 1 << 8;

        void Awake()
        {
            subCamera = GameObject.FindGameObjectWithTag("SubCamera").GetComponent<Camera>();
            rigid2D = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            enemyState = new ReactiveProperty<EnemyState>(EnemyState.Alive);
        }
        // Start is called before the first frame update
        void Start()
        {
            // 何もしなければ延々と動き続ける
            move = Move();

            UnityEngine.Vector3 initPos = subCamera.ScreenToWorldPoint(UnityEngine.Vector3.zero);
            // transform.position = new UnityEngine.Vector3(initPos.x + 0.5f, initPos.y + 10.0f, 1.0f);
            transform.position = new UnityEngine.Vector3(initPos.x + 25.5f, initPos.y + 10.0f, 1.0f);

        }

        void OnDestroy()
        {}


        // Update is called once per frame
        void Update()
        { 
            if (move != null) {
                move = Move();
                StartCoroutine(move);
            }
        }

        void StopMoving()
        {
            if(move != null) {
                StopCoroutine(move);
                move = null;
            }
        }

        // 方向転換
        void TurnXScale()
        {
            UnityEngine.Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
        void TurnYScale()
        {
            UnityEngine.Vector3 scale = transform.localScale;
            scale.y *= -1f;
            transform.localScale = scale;
        }

        // 進行方向にある障害物を見つける（y方向Gizmo）
        private UnityEngine.Vector3 SearchObstacle(UnityEngine.Vector3 destinationPos)
        {
            UnityEngine.Vector3 obstaclePos = UnityEngine.Vector3.zero;

            var tmpPos = (Vector2)destinationPos;
            for (float y = (destinationPos.y - 1.0f); y <= destinationPos.y + 1.0f; y += 0.5f) {
                tmpPos.y = y;
                Vector3 init2DPos = transform.position;
                obstaclePos = SearchObstacleWithRaycast(init2DPos, tmpPos);
                //obstaclePos = SearchObstacleWithRaycast((Vector2)destinationPos, tmpPos);
                float diffY = Math.Abs(obstaclePos.y - transform.position.y);
                if (obstaclePos != UnityEngine.Vector3.zero && (diffY > 0.525)) break;
            }

            return obstaclePos;
        }

        UnityEngine.Vector3 SearchObstacleWithRaycast(Vector2 init2DPos, Vector2 dest2DPos)
        {
            var heading = dest2DPos - init2DPos;
            var direction = heading / heading.magnitude;

            Ray2D ray = new Ray2D(init2DPos, direction);
            // Block, Enemy Layerにのみ衝突するようにする
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, heading.magnitude, targetLayerMask); // 位置、方向、距離
            Debug.DrawRay(ray.origin, ray.direction * heading.magnitude, Color.red, 1);
            
            if (hit && hit.collider.gameObject != this.gameObject) {
                // UnityEngine.Debug.Log("hit: " + hit.transform.gameObject.name);
                return new UnityEngine.Vector3(hit.point.x, hit.point.y, 1.0f);
            }
            return UnityEngine.Vector3.zero;
        }

        IEnumerator Move()
        {
            // 進行方向に動き続ける
            while(enemyState.Value == EnemyState.Alive)
            {
                //単位方向に動く
                var movePos = transform.position;
                movePos.x += 1.0e-6f * transform.localScale.x;
                transform.position = movePos;
                
                // 画面を更新するため
                yield return null;
            }
        }

        bool IsGround() {
            UnityEngine.Vector3 leftStartPoint = transform.position - UnityEngine.Vector3.right * 0.2f;
            UnityEngine.Vector3 rightStartPoint = transform.position + UnityEngine.Vector3.right * 0.2f;
            UnityEngine.Vector3 endPoint = transform.position - UnityEngine.Vector3.up * 1.0f;

            // Debug.DrawLine(leftStartPoint, endPoint, Color.red);
            // Debug.DrawLine(rightStartPoint, endPoint, Color.red);
 
            return Physics2D.Linecast(leftStartPoint, endPoint, blockLayer)
                || Physics2D.Linecast(rightStartPoint, endPoint, blockLayer);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Trap") {
                animator.SetBool("isDeath", true);
                for(var t = 0f; t < 1f; t += Time.deltaTime) {}
                Destroy(this.gameObject);
                }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Trap") {
                enemyState.Value = EnemyState.Dead;
            }
        }
    }
}