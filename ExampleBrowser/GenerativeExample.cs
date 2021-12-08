using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Generative;

namespace ExampleBrowser
{
    public class GenerativeExample : Lazy<BoundsPainter>
    {
        public Type PainterType { get; private set; }

        public static IEnumerable<GenerativeExample> ExampleList { get; private set; }

        static GenerativeExample()
        {
            ExampleList = new List<GenerativeExample>
            {
                new GenerativeExample(typeof(StainedGlass)),
                new GenerativeExample(typeof(Impressionist)),
                new GenerativeExample(typeof(Ribbons)),
                new GenerativeExample(typeof(Threads)),
                new GenerativeExample(typeof(Hair)),
                new GenerativeExample(typeof(Tendrils)),
                new GenerativeExample(typeof(FlowPainter)),
                new GenerativeExample(typeof(ColorSubdivisions)),
                new GenerativeExample(typeof(Canvasify)),
                new GenerativeExample(typeof(Noodling)),
                new GenerativeExample(typeof(TieDye)),
                new GenerativeExample(typeof(StainedGlass)),
                new GenerativeExample(typeof(SolarCorruption)),
                new GenerativeExample(typeof(Clifford)),
            };
        }

        public GenerativeExample(Type painterType)
            : base(() => CreateBoundsPainter(painterType))
        {
            this.PainterType = painterType;
        }

        static BoundsPainter CreateBoundsPainter(Type painterType)
        {
            BoundsPainter painter = null;

            try
            {
                painter = Activator.CreateInstance(painterType) as BoundsPainter;
            }
            catch (Exception ex)
            {
                painter = new BoundsPainter();

                Task.Run(() =>
                {
                    MessageBox.Show((ex is TargetInvocationException) ? ex.InnerException.ToString() : ex.ToString(), "Error Creating Example");
                });
            }

            return painter;
        }
    }
}
