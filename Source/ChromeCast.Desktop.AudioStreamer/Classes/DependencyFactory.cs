using Microsoft.Practices.Unity;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public class DependencyFactory
    {
        public static IUnityContainer Container { get; set; }

        static DependencyFactory()
        {
            Container = new UnityContainer();
        }
    }
}
