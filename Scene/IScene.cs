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


        // TODO maybe turn this into a base class and have these methods be inheritted? 
        bool HandIsTouchingAnyPhobicStimulus();
        bool PatientIsLookingAtAnyPhobicStimulus();
        List<IObjective> GetObjectives();
        bool AllObjectivesComplete();
        float GetMinDistance();
    }
}
