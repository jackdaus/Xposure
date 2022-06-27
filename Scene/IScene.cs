using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal interface IScene
    {
        void Init(int startingLevel);

        void Step();

        void SetCurrentLevel(int level);

        int GetMaxLevel();
    }
}
