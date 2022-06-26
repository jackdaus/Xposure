using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal interface Scene
    {
        void Init(int startingLevel);

        void Step();

        void SetCurrentLevel(int level);
    }
}
