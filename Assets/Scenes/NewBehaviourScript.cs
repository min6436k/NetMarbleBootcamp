using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform[] butten;
    public float A;
    public float B;
    public Ease ease;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void click(int index)
    {
        butten[index].DOPunchScale(Vector3.one * 1, 0.3f);
        for (int i = 0; i < butten.Length; i++)
        {
            if (i == index) continue;
            butten[i].DOLocalMoveX(A, B).SetEase(ease).OnComplete(() =>
            {
                SceneManager.LoadScene(4);
            });
        }
    }
}
