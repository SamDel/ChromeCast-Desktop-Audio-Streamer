using Unity;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    /// <summary>
    /// Unity dependency factory.
    /// </summary>
    public class DependencyFactory
    {
        public static IUnityContainer Container { get; set; }

        static DependencyFactory()
        {
            Container = new UnityContainer();
        }
    }
}
