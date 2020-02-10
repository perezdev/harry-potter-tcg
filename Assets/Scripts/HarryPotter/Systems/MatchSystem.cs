using HarryPotter.Data;
using HarryPotter.GameActions;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class MatchSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        }

        public void ChangeTurn()
        {
            var action = new ChangeTurnAction(1 - Container.GameState.CurrentPlayerIndex);
            Container.Perform(action);
        }

        private void OnPerformChangeTurn(object sender, object args)
        {
            var action = (ChangeTurnAction) args;
            Container.GameState.CurrentPlayerIndex = action.NextPlayerIndex;
            Container.GameState.CurrentPlayer.ActionsAvailable += 2;
        }
        
        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Perform<ChangeTurnAction>(), OnPerformChangeTurn);
        }
    }
}