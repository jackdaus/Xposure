using StereoKit;

namespace StereoKitApp
{
    /// <summary>
    /// A store of all the loaded models to be used by the application. 
    /// Call Copy() on a model to get a unique copy so that different animations can be played across multiple models!
    /// </summary>
    public sealed class Asset
    {
        // Simple, thread-safe Singleton pattern adapted from https://csharpindepth.com/Articles/Singleton
        private static Asset instance = null;
        private static readonly object padlock = new object();
        
        public Model SpiderModelA { get; }
        public Model SpiderModelB { get; }
        public Model SpiderModelC { get; }

        Asset()
        {
            SpiderModelA = Model.FromFile("spiderA.glb");
            SpiderModelB = Model.FromFile("spiderB.glb");
            SpiderModelC = Model.FromFile("spiderC/scene.gltf");
        }

        public static Asset Instance
        {
            get
            {
                lock (padlock)
                {
                    if(instance == null)
                    {
                        instance = new Asset();
                    }
                    return instance;
                }
            }
        }
    }
}
