/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TestUnity2DActionGame.Presenter.Hero;

namespace TestUnity2DActionGame.Presenter.HeroSelect
{
public class HeroInfoClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        HeroInfoController heroInfo = eventData.pointerPress.GetComponent<HeroInfoController>();
        if (heroInfo != null)
        {
            HeroSelectViewController.Instance.ClickedHeroInfoCard(heroInfo);
        }
    }
}
}*/