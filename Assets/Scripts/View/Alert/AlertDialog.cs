using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TestUnity2DActionGame.Domain.Common;
using TestUnity2DActionGame.Presenter.Common;

namespace TestUnity2DActionGame.View.Alert{
    public class AlertDialog : MonoBehaviour
    {
        public SoundManager soundManager;

        [SerializeField] GameObject alertDialog;
        [SerializeField] TextMeshProUGUI alertText;

        private void Awake()
        {
            alertDialog.SetActive(false);
        }

        public void ShowAlertDialog(string message)
        {
            soundManager.PlaySE(SE.DialogOpen); // ダイアログが出たよという意味で
            alertText.text = message;
            alertDialog.SetActive(true);
        }

        public void HideAlertDialog()
        {
            alertDialog.SetActive(false);
        }
    }
}
