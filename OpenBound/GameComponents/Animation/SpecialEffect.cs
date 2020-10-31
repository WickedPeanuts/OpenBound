using Microsoft.Xna.Framework;
using System;

namespace OpenBound.GameComponents.Animation
{
    public enum CeaseCondition
    {
        Cicles,
        Timespan,
        CustomFunction,
    }

    public class SpecialEffect
    {
        public Flipbook Flipbook;
        private int cicles;
        private float timespan;
        private CeaseCondition ceaseCondition;
        
        public Action<SpecialEffect, GameTime> UpdateAction;

        public SpecialEffect(Flipbook flipbook, int cicles)
        {
            Flipbook = flipbook;
            this.cicles = cicles;
            ceaseCondition = CeaseCondition.Cicles;
        }

        public SpecialEffect(Flipbook flipbook, float timespan)
        {
            Flipbook = flipbook;
            this.timespan = timespan;
            ceaseCondition = CeaseCondition.Timespan;
        }

        public SpecialEffect(Flipbook flipbook)
        {
            Flipbook = flipbook;
            ceaseCondition = CeaseCondition.CustomFunction;
        }

        public virtual void Update(GameTime gameTime)
        {
            UpdateAction?.Invoke(this, gameTime);

            switch (ceaseCondition)
            {
                case CeaseCondition.Cicles:
                    UpdateCicles();
                    break;
                case CeaseCondition.Timespan:
                    UpdateTimespan(gameTime);
                    break;
                case CeaseCondition.CustomFunction:
                    UpdateConditional();
                    break;
            }
        }

        private void UpdateCicles()
        {
            if (Flipbook.FlipbookAnimationList[0].IsLastFrame && cicles > 0)
            {
                cicles--;

                if (cicles == 0)
                    SpecialEffectHandler.Remove(this);

            }
        }

        private void UpdateTimespan(GameTime gameTime)
        {
            timespan -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timespan < 0)
                SpecialEffectHandler.Remove(this);
        }

        public virtual bool CeaseFunction()
        {
            return true;
        }

        private void UpdateConditional()
        {
            if (CeaseFunction())
                SpecialEffectHandler.Remove(this);
        }
    }
}
