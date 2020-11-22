using HarryPotter.Enums;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;

namespace HarryPotter.Systems
{
    public class DiscardSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<DiscardAction>(), OnPerformDiscard);
        }

        private void OnPerformDiscard(object sender, object args)
        {
            var action = (DiscardAction) args;

            var playerSystem = Container.GetSystem<PlayerSystem>();
            
            // Discarded Cards should already be set by Ability Loader's target selector.
            foreach (var card in action.DiscardedCards)
            {
                playerSystem.ChangeZone(card, Zones.Discard);

                foreach (var attribute in card.Data.Attributes)
                {
                    attribute.ResetAttribute();
                }
            }
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<DiscardAction>(), OnPerformDiscard);            
        }
    }
}