﻿using StereoKit;

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

        public Sprite IconClose { get; }
        public Sprite IconDown { get; }
        public Sprite IconUp { get; }

        Asset()
        {
            SpiderModelA = Model.FromFile("spiderA.glb");
            SpiderModelB = Model.FromFile("spiderB.glb");
            SpiderModelC = Model.FromFile("spiderC.glb");
            SpiderModelD = Model.FromFile("spiderD.glb");
            SpiderModelE = Model.FromFile("spiderE.glb");
            SpiderModelF = Model.FromFile("spiderF.glb");
            SpiderModelG = Model.FromFile("spiderG/scene.gltf");

            IconClose   = Sprite.FromFile("Icons/outline_close_white_24dp.png");
            IconDown    = Sprite.FromFile("Icons/outline_arrow_downward_white_24dp.png");
            IconUp      = Sprite.FromFile("Icons/outline_arrow_upward_white_24dp.png");
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
