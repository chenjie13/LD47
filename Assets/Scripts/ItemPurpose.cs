
using UnityEngine;

public class ItemPurpose : MonoBehaviour
{
    public GameObject LeftObj;
    public GameObject RightObj;
    public GameObject UpObj;
    public GameObject DownObj;
    public GameObject WaitObj;
    
    public Color SuccessColor;
    public Color FailedColor;

    public void ShowWaitPurpose()
    {
        HandlePurpose(WaitObj);
    }
    
    public void ShowMovePurpose(int x, int y)
    {
        var obj = GetMoveObj(x, y);
        var render = obj.GetComponent<SpriteRenderer>();
        render.color = SuccessColor;
        HandlePurpose(obj);
    }
    
    public void ShowCantMovePurpose(int x, int y)
    {
        var obj = GetMoveObj(x, y);
        var render = obj.GetComponent<SpriteRenderer>();
        render.color = FailedColor;
        HandlePurpose(obj);
    }

    private GameObject GetMoveObj(int x, int y)
    {
        if (x == 0)
        {
            if (y == 1) return UpObj;
            else return DownObj;
        }
        else
        {
            if (x == 1) return RightObj;
            else return LeftObj;
        }
    }

    private void HandlePurpose(GameObject obj)
    {
        HideAll();
        obj.SetActive(true);
        Invoke(nameof(HideAll), 0.3f);
    }

    private void HideAll()
    {
        CancelInvoke();
        LeftObj.SetActive(false);
        RightObj.SetActive(false);
        UpObj.SetActive(false);
        DownObj.SetActive(false);
        WaitObj.SetActive(false);
    }
}
