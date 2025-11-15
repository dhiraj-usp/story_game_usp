
using System;
using UnityEngine;

namespace USP.Minigame.DF_Game
{
    public class Snack : MonoBehaviour, IClickable
    {
        public Action<Snack> OnuserClick;
        public void OnClicked()
        {
            OnuserClick?.Invoke(this);
        }
    }
}
