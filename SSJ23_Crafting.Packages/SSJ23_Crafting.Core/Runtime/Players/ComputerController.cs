using System;
using UnityEngine;

namespace SSJ23_Crafting
{
    public class ComputerController : PlayerController
    {
        private GameManager gameManager;
        private float actionDelay = 2.5f;
        private float counter = 0f;


        public override void OnEnable(Player player)
        {
            gameManager = GameManager.FindOrCreateInstance();
        }

        public override void OnDisable(Player player)
        {

        }

        public override void OnUpdate(Player player)
        {
            counter += Time.deltaTime;
            if (counter <= actionDelay)
            {
                return;
            }

            if (player.Robot is null)
            {
                if (FindAndPlayShaperCard(player))
                {
                    Debug.Log("AI: Playing Shaper Card");
                    counter = 0f;
                    return;
                }

                if (DiscardRandomCard(player))
                {
                    counter = 0f;
                }

                return;
            }

            if (FindAndPlayAttachments(player))
            {
                counter = 0f;
                return;
            }

            Debug.Log("AI: Launching Robot");
            
            var launcher = gameManager.GetLauncher(player.Id);
            if (launcher == null)
            {
                Debug.LogError("Failed to find launcher for player");
                return;
            }

            if (launcher.State != Launcher.LauncherState.Idle)
            {
                return;
            }

            player.LaunchRobot(true);
        }

        private bool FindAndPlayShaperCard(Player player)
        {
            for (var i = 0; i < player.Hand.CardCount; i++)
            {
                var card = player.Hand.GetCard(i);
                if (card.CardType != CardType.Shaper)
                {
                    continue;
                }

                if (!card.IsUsable(player))
                {
                    continue;
                }

                player.UseCard(card);
                return true;
            }

            return false;
        }

        private bool FindAndPlayAttachments(Player player)
        {
            foreach (var slot in player.Robot.Slots)
            {
                if (slot.HasAttachment)
                {
                    continue;
                }

                for (var i = 0; i < player.Hand.CardCount; i++)
                {
                    var card = player.Hand.GetCard(i);
                    if (!(card is AttachmentCard attachment))
                    {
                        continue;
                    }

                    if (!slot.IsValidAttachment(attachment))
                    {
                        continue;
                    }

                    if (!card.IsUsable(player))
                    {
                        continue;
                    }

                    player.UseCard(card);
                    return true;
                }
            }

            return false;
        }

        private bool DiscardRandomCard(Player player)
        {
            Debug.Log("AI: Discarding Card");
            return player.DiscardCard(player.Hand.GetCard(0));
        }
    }
}