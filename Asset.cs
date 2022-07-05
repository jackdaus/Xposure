using StereoKit;

namespace StereoKitApp
{
    /// <summary>
    /// A store of all the loaded models to be used by the application. 
    /// Call Copy() on Models to get a unique copy so that different animations can be played across multiple instances!
    /// </summary>
    public sealed class Asset
    {
        // Simple, thread-safe Singleton pattern adapted from https://csharpindepth.com/Articles/Singleton
        private static Asset instance = null;
        private static readonly object padlock = new object();

        public Model SpiderModelA { get; }
        public Model SpiderModelB { get; }
        public Model SpiderModelC { get; }
        public Model SpiderModelD { get; }
        public Model SpiderModelE { get; }
        public Model SpiderModelF { get; }
        public Model SpiderModelG { get; }

        public Model Bee { get; }

        public Sprite IconClose { get; }
        public Sprite IconDown { get; }
        public Sprite IconEye { get; }
        public Sprite IconPower { get; }
        public Sprite IconUp { get; }

        public Sound BeeBuzz { get; }

        Asset()
        {
            SpiderModelA = Model.FromFile("models/spiderA.glb");
            SpiderModelB = Model.FromFile("models/spiderB.glb");
            SpiderModelC = Model.FromFile("models/spiderC.glb");
            SpiderModelD = Model.FromFile("models/spiderD.glb");
            SpiderModelE = Model.FromFile("models/spiderE.glb");
            SpiderModelF = Model.FromFile("models/spiderF.glb");
            SpiderModelG = Model.FromFile("models/spiderG/scene.gltf");

            Bee = Model.FromFile("models/bee.glb");

            IconClose   = Sprite.FromFile("icons/outline_close_white_24dp.png");
            IconDown    = Sprite.FromFile("icons/outline_arrow_downward_white_24dp.png");
            IconEye     = Sprite.FromFile("icons/outline_visibility_white_24dp.png");
            IconPower   = Sprite.FromFile("icons/outline_power_settings_new_white_24dp.png");
            IconUp      = Sprite.FromFile("icons/outline_arrow_upward_white_24dp.png");

            BeeBuzz    = Sound.FromFile("sounds/bee.mp3");
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
