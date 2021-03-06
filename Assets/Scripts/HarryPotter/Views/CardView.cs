using System;
using System.Text;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Input.InputStates;
using HarryPotter.Systems;
using HarryPotter.Utils;
using HarryPotter.Views.UI;
using HarryPotter.Views.UI.Tooltips;
using UnityEngine;

namespace HarryPotter.Views
{
    public class CardView : MonoBehaviour, ITooltipContent
    {
        public SpriteRenderer CardFaceRenderer;

        public ParticleSystem HighlightParticles;
        public ParticleSystem PlayableParticles;
        
        private Card _card;
        private GameViewSystem _gameView;
        private MatchData _match;
        private CardSystem _cardSystem;
        private Lazy<string> _toolTipDescription;

        public Card Card
        {
            get => _card;
            set
            {
                _card = value;
                InitView(_card);
            }
        }

        private void Awake()
        {
            _gameView = GetComponentInParent<GameViewSystem>();
            _match = _gameView.Match;
            _cardSystem = _gameView.Container.GetSystem<CardSystem>();
            PlayableParticles = GetComponentInChildren<ParticleSystem>();
            
            _toolTipDescription =  new Lazy<string>(GetToolTipDescription);
            
            PlayableParticles.Stop();
            HighlightParticles.Stop();
        }

        private void InitView(Card c)
        {
            CardFaceRenderer.sprite = c.Data.Image;
        }

        private bool IsInTargetingZone()
        {
            if (_gameView.Input.StateMachine.CurrentState is TargetingState targetState)
            {
                return targetState.IsCandidateZone(_card);
            }

            return false;
        }
        
        private void OnMouseOver()
        {
            var playerOwnsCard = Card.Owner.Index == _gameView.Match.LocalPlayer.Index;
            var cardInHand = Card.Zone == Zones.Hand;
            var isPreview = _gameView.Input.StateMachine.CurrentState is PreviewState;
            var isTargeting = _gameView.Input.StateMachine.CurrentState is TargetingState;
            
            if((playerOwnsCard && cardInHand) || Card.Zone.IsInBoard() || IsInTargetingZone()) // TODO: Need to check zone's alliance or we may end up showing tooltips for face down cards.
            {
                _gameView.Tooltip.Show(this);
            }

            if (_cardSystem.IsPlayable(Card) && _match.CurrentPlayerIndex == _match.LocalPlayer.Index)
            {
                _gameView.Cursor.SetActionCursor();
            }

            if (playerOwnsCard && cardInHand && !isPreview && !isTargeting)
            {
                var color = _cardSystem.IsPlayable(Card) ? Colors.Playable : Colors.Unplayable;
                PlayableParticles.SetParticleColor(color);
                PlayableParticles.Play();
            }
            else
            {
                PlayableParticles.Stop();
            }
        }

        private void OnMouseExit()
        {
            _gameView.Tooltip.Hide();
            _gameView.Cursor.ResetCursor();
            
            PlayableParticles.Stop();
        }

        public string GetDescriptionText()
        {
            var tooltipText = new StringBuilder();

            var lessonCost = _card.GetAttribute<LessonCost>();
            if (lessonCost != null)
            {
                tooltipText.AppendLine($@"<align=""right"">{lessonCost.Amount} {TextIcons.FromLesson(lessonCost.Type)}</align>");
            }
            tooltipText.AppendLine($"<b>{_card.Data.CardName}</b>");
            tooltipText.AppendLine($"<i>{_card.Data.Type}</i>");

            var creature = _card.GetAttribute<Creature>();
            if (creature != null)
            {
                //TODO: Show current health in separate color if it does not == MaxHealth 
                tooltipText.AppendLine($"{TextIcons.ICON_ATTACK} {creature.Attack}");
                tooltipText.AppendLine($"{TextIcons.ICON_HEALTH} {creature.Health} / {creature.MaxHealth}");
            }
            
            if (!string.IsNullOrWhiteSpace(_card.Data.CardDescription))
            {
                
                tooltipText.AppendLine(_toolTipDescription.Value);                
            }

            return tooltipText.ToString();
        }

        public string GetActionText(MonoBehaviour context)
        {
            if (_gameView.IsIdle &&
                _gameView.Input.StateMachine.CurrentState is ITooltipContent tc)
            {
                return tc.GetActionText(this);
            }

            return string.Empty;
        }

        public void Highlight(Color color)
        {
            if (color == Color.clear)
            {
                HighlightParticles.Stop();
            }
            else
            {
                HighlightParticles.SetParticleColor(color);
                HighlightParticles.Play();
            }
        }
        
        private string GetToolTipDescription()
        {
            const int wordsPerLine = 12;

            var words = _card.Data.CardDescription.Split(' ');
            var splitText = new StringBuilder();

            int wordCount = 0;

            foreach (string word in words)
            {
                splitText.Append($"{word} ");
                wordCount++;

                if (wordCount > wordsPerLine)
                {
                    // TODO: Fix trailing space at the end of each line
                    splitText.AppendLine();
                    wordCount = 0;
                }
            }

            return splitText.ToString().TrimEnd(' ', '\n');
        }
    }
}