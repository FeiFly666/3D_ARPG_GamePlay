using Assets.Model;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.UI
{
    public class UICharacterItem : MonoBehaviour
    {

        [SerializeField] private Image itemIcon;
        [SerializeField] private Image process;

        private PlayerController player;
        void Start()
        {
            player = Manager.Character.player;
            if(player != null)
            {
                player.ItemCtrl.OnItemProgressChanged += ChangeItemProgress;

                player.ItemCtrl.OnItemChanged += ChangeItemUI;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(player == null)
            {
                player = Manager.Character.player;
                if (player != null)
                {
                    player.ItemCtrl.OnItemProgressChanged += ChangeItemProgress;

                    player.ItemCtrl.OnItemChanged += ChangeItemUI;
                }

            }
        }
        private void ChangeItemUI(ItemData item)
        {
            if(item.icon != null)
            {
                itemIcon.sprite = item.icon;
            }
        }
        private void ChangeItemProgress(float max, float start)
        {
            float process = 1 - Mathf.Min(1, (Time.time - start) / max);

            this.process.fillAmount = process;

            /*if(process >= 1 )
            {
                this.process.gameObject.SetActive(false);
            }
            else
            {
                this.process.gameObject.SetActive(true);
            }*/
        }

        private void OnDestroy()
        {
            if (player != null)
            {
                player.ItemCtrl.OnItemProgressChanged -= ChangeItemProgress;

                player.ItemCtrl.OnItemChanged -= ChangeItemUI;
            }
        }
    }
}