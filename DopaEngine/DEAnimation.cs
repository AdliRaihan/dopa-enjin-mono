using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaEngine
{
    public enum DEAnimationType
    {
        Scale, Position, Size, SpriteAnimation, None
    }
    public enum DEAnimationState
    {
        OnAnimation, FinishedAnimation
    }

    public enum DEAnimationLoopAction
    {
        infinite, once
    }
    public class DEAnimation
    {
        private DEBaseObject BaseObject;
        public DEAnimationLoopAction actions = DEAnimationLoopAction.once;
        public DEAnimationType type = DEAnimationType.None;
        public DEAnimationState state = DEAnimationState.FinishedAnimation;

        private float interloopValue = 0.0f;
        private float targetScale = 0.0f;
        private float sourceScale = 0.0f;

        private Vector2 sourcePosition = new();
        private Vector2 targetPosition = new();

        int CurrentIndexAnimationSprite = 0;
        int MaxIndexAnimationSprite { get => SpriteAnimationList.Count; }
        List<Vector2> SpriteAnimationList = new();

        public DEAnimation(DEBaseObject _baseObject) => BaseObject = _baseObject;

        public void SetAnimationScale(float _targetScale)
        {
            targetScale = _targetScale;
            type = DEAnimationType.Scale;
        }
        public void SetAnimationSprite(List<Vector2> SpriteData)
        {
            type = DEAnimationType.SpriteAnimation;
            SpriteAnimationList = SpriteData;
        }
        public void SetAnimationSize(Vector2 from, Vector2 to)
        {
            type = DEAnimationType.Size;
            sourcePosition = from;
            targetPosition = to;
        }

        public void StartAnimate()
        {
            CurrentIndexAnimationSprite = 0;
            interloopValue = 0.0f;
            sourceScale = 0.0f;
            state = DEAnimationState.OnAnimation;
        }

        public void OnAnimateUpdate(GameTime gameTime)
        {
            if (state == DEAnimationState.FinishedAnimation) return;
            switch (type)
            {
                case DEAnimationType.Scale:
                    UpdateScale();
                    break;
                case DEAnimationType.Position:
                    break;
                case DEAnimationType.Size:
                    UpdateSize();
                    break;
                case DEAnimationType.SpriteAnimation:
                    UpdateAnimationBySprite();
                    break;
                case DEAnimationType.None:
                    break;
            }
        }

        private void UpdateScale()
        {
            if (sourceScale >= targetScale)
            {
                state = DEAnimationState.FinishedAnimation;
                return;
            }
            if (sourceScale <= targetScale)
                sourceScale += interloopValue;
            else
                sourceScale -= interloopValue;

            interloopValue += 0.05f;

            if (interloopValue >= 0.1f)
                interloopValue = 0.1f;

            BaseObject.ObjectScalingValue = sourceScale;
        }
        private void UpdateAnimationBySprite()
        {
            if (CurrentIndexAnimationSprite > (MaxIndexAnimationSprite - 1))
            {
                if (actions == DEAnimationLoopAction.infinite)
                {
                    StartAnimate();
                    return;
                }
                state = DEAnimationState.FinishedAnimation; return;
            }
            BaseObject.SetSpriteLocationVector(SpriteAnimationList[CurrentIndexAnimationSprite]);
            CurrentIndexAnimationSprite += 1;
        }
        private void UpdateSize()
        {
            if (Vector2.Distance(sourcePosition, targetPosition) <= 1)
            {
                if (actions == DEAnimationLoopAction.infinite)
                {
                    StartAnimate();
                    return;
                }
                state = DEAnimationState.FinishedAnimation; return;
            }
            if (targetPosition.X != 0)
                if (sourcePosition.X > targetPosition.X) sourcePosition.X -= 1f;
                else sourcePosition.X += 1f;
            if (targetPosition.Y != 0)
                if (sourcePosition.Y > targetPosition.Y) sourcePosition.Y -= 1f;
                else sourcePosition.Y += 1f;
            BaseObject.SetSize(sourcePosition.ToPoint());
        }
    }
}
