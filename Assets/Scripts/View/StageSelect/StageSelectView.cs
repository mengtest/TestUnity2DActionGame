using System.Net;
using System.Security.AccessControl;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UniRx;
using MiniUnidux;
using MiniUnidux.SceneTransition;
using MiniUnidux.Util;

namespace TestUnity2DActionGame.View.StageSelect
{
    public class StageSelectView : MonoBehaviour
    {
        public Button battleStartButton;  // button of battle start

        public Transform canvasTransform;  // canvas transform

        //[SerializeField] StageInfoDialog stageInfoDialogPrefab; // Stage Infomantion Dialog Prefab  
        //[System.NonSerialized] public StageInfoDialog stageInfoDialog = null;

        //[SerializeField] AlertDialog alertDialogPrefab; // Alert Dialog Prefab  
        //[System.NonSerialized] public AlertDialog alertDialog = null;

        // Stage Prefab
        //[SerializeField] StageInfoController stageInfoPrefab;

        [SerializeField] TextMeshProUGUI untilEndOfSelectText;

        private void Awake()
        {
            // ステージ情報ダイアログを生成する
            /*if (stageInfoDialog == null) {
                stageInfoDialog = Instantiate(stageInfoDialogPrefab, canvasTransform, false);
            }*/

            // 警告ダイアログを生成する
            /*if (alertDialog == null) {
                alertDialog = Instantiate(alertDialogPrefab, canvasTransform, false);
            }
            */
        }

        private void Start()
        {
           
        }

        private void OnDestroy()
        {
            CleanUp();
        }

        // いらないオブジェクトの破棄
        void CleanUp()
        {
            /*if (alertDialog != null) {
                Destroy(alertDialog.gameObject);
            }*/

            /*if (stageInfoDialog != null) {
                Destroy(stageInfoDialog.gameObject);
            }*/
        }

        public void ShowStageInfoDialog(int stageID, Sprite icon, string name, string info)
        {
            //stageInfoDialog.Show(stageID, player, icon, name, info);
        }

        public void SetUntilEndOfSelectText(string untilEndOfSelectString)
        {
            untilEndOfSelectText.text = untilEndOfSelectString;
        }

       /* public StageInfoController GetStageInfoPrefab()
        {
            return stageInfoPrefab;
        }
        */
    }
}