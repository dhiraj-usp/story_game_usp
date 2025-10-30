using System;
using UnityEngine;


namespace USP.Minigame.DF_Game{
public class ClikcableObject : MonoBehaviour,IClickable
{
    public Action OnClick;
    public void OnClicked()
    {
        OnClick?.Invoke();
    }
}
}