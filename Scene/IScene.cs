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

        /// <summary>
        /// Should be called to properly clean up a scene
        /// </summary>
        void Destroy();


        // TODO maybe turn this into a base class and have these methods be inheritted? 
        bool PatientIsTouchingAnyPhobicStimulus();
        bool PatientIsLookingAtAnyPhobicStimulus();
        bool PatientIsHoldingAnyPhobicStimulus();
        List<IObjective> GetObjectives();
        bool AllObjectivesComplete();
        float GetMinDistance();
    }
}
