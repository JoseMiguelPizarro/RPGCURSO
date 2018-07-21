using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrasCinematicas : MonoBehaviour {

    private RectTransform topBar, BottomBar;
    public float targetSize = 300;
    public float tiempo;
    private float changeSizeAmount;

    private void Awake()
    {
        GameObject gameObject = new GameObject("topBar", typeof(Image));
        gameObject.transform.SetParent(transform, false);
        gameObject.GetComponent<Image>().color = Color.black;
        topBar = gameObject.GetComponent<RectTransform>();
        topBar.anchorMin = new Vector2(0, 1);
        topBar.anchorMax = new Vector2(1, 1);
        topBar.sizeDelta = new Vector2(0, 0);


        gameObject = new GameObject("bottomBar", typeof(Image));
        gameObject.transform.SetParent(transform, false);
        gameObject.GetComponent<Image>().color = Color.black;
        BottomBar = gameObject.GetComponent<RectTransform>();
        BottomBar.anchorMin = new Vector2(0, 0);
        BottomBar.anchorMax = new Vector2(1, 0);
        BottomBar.sizeDelta = new Vector2(0, 0);
    }

    private void Update()
    {
        //Vector2 sizeDelta = topBar.sizeDelta;
        //sizeDelta.y += changeSizeAmount * Time.deltaTime;
        //if (changeSizeAmount>0)
        //{
        //    if (sizeDelta.y>=targetSize)
        //    {
        //        sizeDelta.y = targetSize;
        //    }
        //}
        //topBar.sizeDelta = sizeDelta;
        //BottomBar.sizeDelta = sizeDelta;
    }

    public void Show()
    {
        targetSize = 300;
        changeSizeAmount = (targetSize - topBar.sizeDelta.y)/tiempo;
        StartCoroutine(Mostrar());
    }

    public void Hide()
    {
        targetSize = 0;
        changeSizeAmount= (targetSize - topBar.sizeDelta.y) / tiempo;
        StartCoroutine(Mostrar());
    }

    IEnumerator Mostrar()
    {
            Vector2 sizeDelta = topBar.sizeDelta;

        while (Mathf.Abs( sizeDelta.y-targetSize)> Mathf.Abs(changeSizeAmount)*Time.deltaTime)
        {
            sizeDelta.y += changeSizeAmount * Time.deltaTime;
           // sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y);
            topBar.sizeDelta = sizeDelta;
            BottomBar.sizeDelta = sizeDelta;
            yield return new WaitForEndOfFrame();
        }
        topBar.sizeDelta =new Vector2(topBar.sizeDelta.x,  targetSize);
        BottomBar.sizeDelta =new Vector2(BottomBar.sizeDelta.x, targetSize);
    }
}
