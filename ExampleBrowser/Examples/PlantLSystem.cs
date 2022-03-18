using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Generative;

namespace ExampleBrowser
{
    struct LState
    {
        public static float MoveMult = 0.9f;
        public static float AngleMult = 0.9f;

        public SKPoint Position;
        public float Rotation;
    }

    public class PlantLSystem : BoundsPainter
    {
        Dictionary<char, List<KeyValuePair<float, string>>> rules = new Dictionary<char, List<KeyValuePair<float, string>>>();
        SKPaint paint;

        public PlantLSystem()
        {
            paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round
            };
        }

        void AddRule(char lhs, string rhs)
        {
            AddRule(lhs, rhs, 1);
        }

        void AddRule(char lhs, string rhs, float weight)
        {
            if (!rules.ContainsKey(lhs))
            {
                rules[lhs] = new List<KeyValuePair<float, string>>();
            }

            rules[lhs].Add(new KeyValuePair<float, string>(weight, rhs));
        }

        string PickRule(char lhs)
        {
            var ruleList = rules[lhs];

            float totWeight = 0;

            foreach (KeyValuePair<float, string> pair in ruleList)
                totWeight += pair.Key;

            float rand = totWeight * (float)Random.NextDouble();

            float weight = 0;
            
            foreach (KeyValuePair<float, string> pair in ruleList)
            {
                weight += pair.Key;

                if (weight > rand)
                    return pair.Value;
            }

            throw new InvalidOperationException("Shouldn't get here");
        }

        public static SKPoint GetAngleVector(float angle, float length)
        {
            return new SKPoint((float)Math.Cos(angle) * length, -(float)Math.Sin(angle) * length);
        }

        public override IEnumerable<bool> ProgressivePaint(SKRect bounds)
        {
            AddRule('F', "FF", 0.5f);
            AddRule('F', "FFF", 0.2f);
            AddRule('F', "F[-F]");
            AddRule('F', "F[+F]");
            AddRule('F', "F[-F]{+F]");

            Draw('F', new LState { Position = new SKPoint(bounds.MidX, bounds.Height * 0.95f), Rotation = (float)Math.PI * 0.5f }, 8, 50, (float)Math.PI / 6.0f);

            yield return true;
        }

        LState Draw(char lhs, LState state, int iterations, float moveDistance, float turnAngle)
        {
            iterations--;

            string rhs = PickRule('F');

            LState store = new LState();

            foreach (char c in rhs)
            {
                switch (c)
                {
                    case 'F':
                        if (iterations == 0)
                        {
                            SKPoint start = state.Position;
                            state.Position += GetAngleVector(state.Rotation, moveDistance);

                            Canvas.DrawLine(start, state.Position, paint);
                        }
                        else
                        {
                            state = Draw('F', new LState { Position = state.Position, Rotation = state.Rotation }, iterations, moveDistance * LState.MoveMult, turnAngle * LState.AngleMult);
                        }

                        break;

                    case '[':
                        store = state;
                        break;

                    case ']':
                        state = store;
                        break;

                    case '-':
                        state.Rotation += turnAngle;
                        break;

                    case '+':
                        state.Rotation -= turnAngle;
                        break;
                }
            }

            return state;
        }
    }
}
