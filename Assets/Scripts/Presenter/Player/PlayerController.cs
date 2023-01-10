using System.Timers;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UniRx;
using MiniUnidux.Util;
using TestUnity2DActionGame.Domain.Common;
using TestUnity2DActionGame.Domain.Player;
using TestUnity2DActionGame.Presenter.Common;

namespace TestUnity2DActionGame.Presenter.Player
{
    public class PlayerController : MonoBehaviour
    {
        UnityEngine.Vector3 mousePos, worldPos;

        private Camera subCamera;
        private Rigidbody2D rigid2D;
        private Animator animator;

        UnityEngine.Vector3 pos0, pos1, pos;
        public float sensitivity = 1f;  // 感度調節用
        [SerializeField] float flightTime = 0.5f;  // 対空時間
        [SerializeField] float speedRate = 1f;   // 移動速度倍率（対空時間基準）

        private float xDirection;
        private float gravity = -9.8f;
        float loadWidth = 6f;
        float loadHeight= 10f;

        private MoveState moveState;
        IEnumerator move;

        public ReactiveProperty<PlayerState> playerState { get; set; }
        /* FixMe ニンジン入手でスコアUP */
        public ReactiveProperty<int> tresureCount { get; set; } // 入手したニンジンの数

        // LayerMask targetLayerMask = 1 << 9 | 1 << 10;
        LayerMask blockLayer = 1 << 9;
        LayerMask targetLayerMask = 1 << 10;

        // sound
        private SoundManager soundManager;

        void Awake()
        {
            subCamera = GameObject.FindGameObjectWithTag("SubCamera").GetComponent<Camera>();
            rigid2D = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            playerState = new ReactiveProperty<PlayerState>(PlayerState.Alive);
            tresureCount = new ReactiveProperty<int>(0);
        }
        // Start is called before the first frame update
        void Start()
        {
            // CommonからSoundManagerを取得する
            soundManager = new CommonObjectGetUtil().GetCommonObject("SoundManager").GetComponent<SoundManager>();
            
            // 何もしなければ延々と動き続ける
            moveState = MoveState.Moving;
            move = Move();
            xDirection = 1.0f;

            UnityEngine.Vector3 initPos = subCamera.ScreenToWorldPoint(UnityEngine.Vector3.zero);
            // transform.position = new UnityEngine.Vector3(initPos.x + 0.5f, initPos.y + 10.0f, 1.0f);
            transform.position = new UnityEngine.Vector3(initPos.x + 20.5f, initPos.y + 10.0f, 1.0f);

        }

        // Update is called once per frame
        void Update()
        { 
            // 歩くアニメーションの変更
            UpdateMoveState();

            // カメラの位置調整
            UpdateCameraPosition();

            if (Input.GetMouseButtonDown(0)) pos0 = Input.mousePosition;
            if (Input.GetMouseButtonUp(0)) pos1 = Input.mousePosition;
            if (Input.GetMouseButton(0)) {
                pos = Input.mousePosition;
                UpdatePositionFromSwipe();
            }
 
             // 数秒待ったらまた歩き出す
            if (moveState == MoveState.Idling) {
                StopMoving();   // 念の為
                StartCoroutine(WaitAndMoveStart(1));
            } else if (moveState == MoveState.Moving) {
                move = Move();
                StartCoroutine(move);
            } else if (moveState == MoveState.Stopping) {
                if (!IsGround()) rigid2D.isKinematic = true;
            } else {
                StopMoving();   // 念の為
            }

            // 目の前に敵が来たら逃げる
            if (moveState == MoveState.Idling || moveState == MoveState.Moving) {
                UnityEngine.Vector3 tmpPos = transform.position;
                tmpPos.x += 2f * xDirection;
                UnityEngine.Vector3 obstaclePos = SearchObstacle(tmpPos);
                if (obstaclePos != UnityEngine.Vector3.zero) {

                    soundManager.PlaySE(SE.OK);

                    moveState = MoveState.Idling;
                    StopMoving();   // 念の為
                    xDirection *= -1.0f; // 逆向きになる
                    ChangeXScale();
                    animator.SetBool("isHurt", true);
                    StartCoroutine(WaitAndMoveStart(1));
                } 
            }
        }

        // 画面の表示範囲を超えない限りカメラがプレイヤーを追跡する
        void UpdateCameraPosition()
        {
            Vector3 subCameraPos = subCamera.transform.position;
            subCameraPos.x = transform.position.x - 0.5f;
            /*if (transform.position.x <= -loadWidth) {
                subCameraPos.x = loadWidth - 0.5f;
            }
            if (transform.position.x <= loadWidth) {
                subCameraPos.x = loadWidth - 0.5f;
            }
            if (transform.position.y <= loadHeight) {
                subCameraPos.y = 0;
            }
            if (transform.position.y >= -loadHeight) {
                subCameraPos.y = 0;
            }*/
            subCamera.transform.position = subCameraPos;
        }

        void StopMoving()
        {
            if(move != null) {
                StopCoroutine(move);
                move = null;
            }
        }

        IEnumerator WaitAndMoveStart(int seconds)
        {
            float count = seconds;
            var initPos = transform.position;
            while(count > 0) {
                // 念の為座標を固定する
                transform.position = initPos;

                yield return new WaitForSeconds(1.0f);
                count--;
            }
            yield return null;
            moveState = MoveState.Moving;
        }

        void UpdatePositionFromSwipe()
        { 
            moveState = MoveState.Idling;
            StopMoving();

            pos1 = Input.mousePosition;
            float diffXDistance = (pos.x - pos0.x) / Screen.width * loadWidth;
            diffXDistance *= sensitivity;

            if (diffXDistance > 0) {
                xDirection = 1;
                ChangeXScale();
            } else if (diffXDistance < 0) {
                xDirection = -1;
                ChangeXScale();
            }

            // 上スワイプでジャンプ、下スワイプでしゃがみ
            float diffYDistance = (pos.y - pos0.y) / Screen.height * loadHeight;
            diffYDistance *= sensitivity;
            if (diffYDistance > 0.5f && IsGround()) {
                moveState = MoveState.Jumping;
                StartCoroutine(Jump(xDirection));

            } else if (diffYDistance < -0.5f && IsGround()) {
                moveState = MoveState.Crouching;
                StartCoroutine(Crouch());
            }
        }

        void ChangeXScale()
        {
            UnityEngine.Vector3 scale = transform.localScale;
            scale.x = xDirection;
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
            while(moveState == MoveState.Moving)
            {
                animator.SetBool("isHurt", false);  // 念の為

                //単位方向に動く
                var movePos = transform.position;
                movePos.x += 1.0e-5f * xDirection;
                transform.position = movePos;
                
                // 画面を更新するため
                yield return null;
            }
        }

        IEnumerator Climb(Vector3 destPos)
        {
            // 進行方向に動き続ける
            float yDirection = 1f;
            if (destPos.y < transform.position.y) yDirection = -1f;
            while (moveState == MoveState.Climbing && Mathf.Abs(destPos.y - transform.position.y) > 1.0e-5f)
            {
                // rigidBodyにかかる重力調整
                rigid2D.isKinematic = true;

                //単位方向に動く
                var movePos = transform.position;
                movePos.y += 1.0e-4f * yDirection;
                transform.position = movePos;
                // UnityEngine.Debug.Log("isClimbing: " + animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerClimb"));

                // 画面を更新するため
                yield return null;
            }
            if (moveState != MoveState.Stopping) {
                moveState = MoveState.Idling;
            }

            yield return null;

        }

        void UpdateMoveState()
        {
            // 地面に接しているか
            if (IsGround()) {
                // rigidBodyにかかる重力調整
                rigid2D.isKinematic = false;
                animator.SetBool("isFalling", false);
                if (moveState == MoveState.Moving) {
                    animator.SetFloat("speed", speedRate);
                } else {
                    animator.SetFloat("speed", 0f);
                }
             } else if (moveState == MoveState.Idling || moveState == MoveState.Climbing || moveState == MoveState.Stopping) {
                rigid2D.isKinematic = true;
                animator.SetBool("isFalling", false);
                // UnityEngine.Debug.Log("isFalling: " + animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerFall"));
           } else {
                // 重力調整
                rigid2D.isKinematic = false;
                animator.SetBool("isFalling", true);
           }
        }

        IEnumerator Jump(float xDirection)
        {
            UpdateMoveState();
            animator.SetBool("isJumping", true);
            var initPos = transform.position;
            var destPos = new UnityEngine.Vector3(initPos.x + xDirection * 4f, initPos.y, initPos.z);
            ChangeXScale();
            var diffY = (destPos - initPos).y;
            var vf = (diffY - 0.5f * gravity * flightTime * flightTime) / flightTime;   // y方向初速度

            var time = 0f;

            var isColideFloor = false;
            var isColideCeil = false;
            for (time = 0.0f; time < flightTime; time += Time.deltaTime * speedRate) {
                
                // 重力調整
                rigid2D.isKinematic = true;

                var tmpPos = UnityEngine.Vector3.Lerp(initPos, destPos, time / flightTime);
                tmpPos.y = initPos.y + vf * time + 0.5f * gravity * time * time;
                transform.position = tmpPos;

                if (time > 0.5 * flightTime) {
                    animator.SetBool("isJumping", false);
                    animator.SetBool("isFalling", true);
                }

                // 1フレーム待つ（画面を更新するため）
                yield return null;

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerFall") && IsGround()) {
                    isColideFloor = true;
                    break;
                } else if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerJump") && IsCeil()) {
                    isColideCeil = true;
                    break;
                }
            }

            // 前と現在の位置を更新する
            if (time >= flightTime) {
                transform.position = destPos;
            }

            if (isColideFloor) {
                animator.SetBool("isFalling", false);
                moveState = MoveState.Idling;
            } else if (isColideCeil) {
                animator.SetBool("isJumping", false);
            } else {
                // 重力調整
                rigid2D.isKinematic = false;
            }

            yield return null;
        }

        IEnumerator Crouch()
        {
            animator.SetBool("isCrouching", true);
            var initPos = transform.position;
            for (var time = 0.0f; time < flightTime; time += Time.deltaTime * speedRate) {
                // 念の為座標を固定する
                transform.position = initPos;
                // 1フレーム待つ（画面を更新するため）
                yield return null;
            }
            animator.SetBool("isCrouching", false);
            moveState = MoveState.Idling;
            yield return null;

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

        bool IsCeil() {
            UnityEngine.Vector3 leftStartPoint = transform.position - UnityEngine.Vector3.right * 0.2f;
            UnityEngine.Vector3 rightStartPoint = transform.position + UnityEngine.Vector3.right * 0.2f;
            UnityEngine.Vector3 endPoint = transform.position + UnityEngine.Vector3.up * 1.0f;

            // Debug.DrawLine(leftStartPoint, endPoint, Color.red);
            // Debug.DrawLine(rightStartPoint, endPoint, Color.red);
 
            return Physics2D.Linecast(leftStartPoint, endPoint, blockLayer)
                || Physics2D.Linecast(rightStartPoint, endPoint, blockLayer);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Trap") {
                playerState.Value = PlayerState.Dead;
            } else if (collision.gameObject.tag == "Trap") {
                tresureCount.Value ++;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Trap") {
                playerState.Value = PlayerState.Dead;
            } else if (collision.gameObject.tag == "Ladder") {
                StopMoving();   // 念の為
                moveState = MoveState.Climbing;
                animator.SetBool("isClimbing", true);
                var destPos = transform.position;
                destPos.y += 3f;
                StartCoroutine(Climb(destPos));
            } else if (moveState == MoveState.Climbing && collision.gameObject.tag == "Goal") {
                rigid2D.isKinematic = true;
                StopMoving();   // 念の為
                moveState = MoveState.Stopping;
                playerState.Value = PlayerState.StageClear;
            }
        }
    }
}