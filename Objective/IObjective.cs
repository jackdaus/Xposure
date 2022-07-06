using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    public interface IObjective
    {
        /// <summary>
        /// A description of the objective.
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public string Description(SessionHistory history);

        /// <summary>
        /// If the objective is completed.
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public bool IsCompleted(SessionHistory history);
    }
}
