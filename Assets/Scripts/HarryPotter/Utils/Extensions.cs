﻿using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Views;
using UnityEngine;

namespace HarryPotter.Utils
{
    public static class Extensions
    {
        public static Color WithAlpha(this Color c, float alpha) => new Color(c.r, c.g, c.b, Mathf.Clamp01(alpha));

        public static Sequence Move(this CardView cardView, Vector3 position, Vector3 rotation, float duration = 0.5f,
            float startDelay = 0f, float endDelay = 0f, bool doLocal = false)
            => DOTween.Sequence()
                .AppendInterval(startDelay)
                .Append(cardView.transform.DOMove(position, duration))
                .Join(cardView.transform.DORotate(rotation, duration))
                .AppendInterval(endDelay)
                ;

        public static bool IsInPlay(this Zones z) 
            => new[]{
            Zones.Items,
            Zones.Creatures,
            Zones.Location,
            Zones.Match,
            Zones.Characters,
            Zones.Adventure,
            Zones.Lessons
        }.Contains(z);

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> src) => new HashSet<T>(src);

        public static TAttribute GetAttribute<TAttribute>(this Card card)
            where TAttribute : CardAttribute
        {
            var modifiedAttribute = card.Attributes.OfType<TAttribute>().SingleOrDefault();
            return modifiedAttribute;
        }
        
        public static List<TAttribute> GetAttributes<TAttribute>(this Card card)
            where TAttribute : CardAttribute
        {
            var modifiedAttributes = card.Attributes.OfType<TAttribute>().ToList();
            return modifiedAttributes;
        }

        public static LessonType GetLessonType(this Card card)
        {
            var cost = card.GetAttribute<LessonCost>();
            if (cost != null)
            {
                return cost.Type;
            }

            var provider = card.GetAttribute<LessonProvider>();
            if (provider != null)
            {
                return provider.Type;
            }

            return LessonType.None;
        }
        
        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            var size = rectTransform.rect.size;
            var deltaPivot = rectTransform.pivot - pivot;
            var deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }
    }
}