using StereoKit;
using System.Timers;

namespace StereoKitApp
{
    internal class Bee : PhobicStimulus
    {
        private Sound _buzzSound;
        private SoundInst _currentSound;
        private bool _soundEnabled;
        private Timer _loopTimer;

        public Bee()
        {
        }

        protected override void InitModels()
        {
            // Initialize level 0 (empty model)
            Model beeModel = new Model();
            models.Add(beeModel);

            // Initialize level 1
            beeModel = Model.FromMesh(Mesh.GenerateSphere(0.0115f), Default.MaterialUI);
            models.Add(beeModel);

            // Initialize level 2
            beeModel = Model.FromMesh(Mesh.GenerateSphere(0.0115f), Default.MaterialUI);
            beeModel.RootNode.AddChild("abs", Matrix.T(0, 0, -0.0115f), Mesh.GenerateSphere(0.0184f), Default.MaterialUI);
            beeModel.RootNode.ModelTransform *= Matrix.R(0, 180, 0);
            models.Add(beeModel);

            // Initialize level 3
            beeModel = Asset.Instance.SpiderModelA.Copy();
            beeModel.RootNode.ModelTransform *= Matrix.S(0.045f);
            beeModel.RootNode.ModelTransform *= Matrix.R(0, 180, 0);
            models.Add(beeModel);

            // Initialize level 4
            //beeModel = Asset.Instance.Bee.Copy();
            //beeModel.RootNode.ModelTransform *= Matrix.S(0.05f);
            //models.Add(beeModel);

            // Initialize level 4 animated! but texture doesn't load :(
            beeModel = Asset.Instance.Bee.Copy();
            beeModel.RootNode.ModelTransform *= Matrix.S(0.001f);
            models.Add(beeModel);

            // Sound
            // TODO This doesn't works with multiple instances of bees...
            // ... the sound will get stomped by the other instances
            _buzzSound = Asset.Instance.BeeBuzz;
            _loopTimer = new Timer(Asset.Instance.BeeBuzz.Duration * 1000);
            _loopTimer.Elapsed += OnLoopEvent;
            _loopTimer.AutoReset = true;
        }

        public override void Step()
        {
            base.Step();
            if (_soundEnabled)
                _currentSound.Position = GetPosition();
        }


        public bool SoundEnabled
        {
            get => _soundEnabled;
            set
            {
                if (value)
                {
                    _loopTimer.Enabled  = true;
                    _soundEnabled       = true;
                    _currentSound       = _buzzSound.Play(GetPosition());
                }
                else
                {
                    _loopTimer.Enabled  = false;
                    _soundEnabled       = false;

                    // Stop playing the current sound if there is one playing
                    _currentSound.Stop();
                }
            }
        }

        /// <summary>
        /// Should be called before removing a bee!
        /// </summary>
        public void Destroy()
        {
            _loopTimer.Dispose();
        }

        private void OnLoopEvent(object source, ElapsedEventArgs e)
        {
            _currentSound = _buzzSound.Play(GetPosition());
        }
    }
}
