using StereoKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StereoKitApp
{
    internal abstract class PhobicStimulus
    {
        protected List<Model> models = new List<Model>();
        private Solid _solid;
        private readonly Random _random = new Random();
        private int _modelIntensity = 0;
        private Model _activeModel;
        private int _maxModelLevel { get => models.Count - 1; }
        private bool _isHeld;
        private bool _animationEnabled;

        // used for Roaming.Walk
        private DateTime _lastWalkingChange = DateTime.Now;
        private bool _isWalking = true;

        // used for Roaming.Fly
        private float _flightTheta = 0;

        // debug
        private Pose _debugWindowPose = new Pose(0.4f, 0, -0.4f, Quat.LookDir(0, 0, 1));
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Enable or disable the model's physics simulation.
        /// </summary>
        public bool PhysicsEnabled { get; set; }

        /// <summary>
        /// Enable or disable the model's animation.
        /// </summary>
        public bool AnimationEnabled 
        { 
            get => _animationEnabled; 
            set
            {
                _animationEnabled = value;
                syncAnimation();
            }
        }

        /// <summary>
        /// Size of the model
        /// </summary>
        public float Scale { get; set; } = 3;

        /// <summary>
        /// How the model is moving on its own
        /// </summary>
        public Roaming RoamingMode { get; set; }

        /// <summary>
        /// Level of realism of the model
        /// </summary>
        public int ModelIntensity
        {
            get { return _modelIntensity; }
            set
            {
                if (_modelIntensity < 0)
                    throw new ArgumentOutOfRangeException(nameof(ModelIntensity), "The ModelIntensity must be greater than or equal to 0");

                if (_modelIntensity + 1 > models.Count)
                    throw new IndexOutOfRangeException($"The ModelIntensity {_modelIntensity} does not exist. The maximum ModelIntensity is {models.Count - 1}");

                _modelIntensity = value;
                _activeModel = models[_modelIntensity];

                // sync model animation to the current animation state
                syncAnimation();

                // TODO when the model changes, also change the solid's physical shape
                // Maybe this would be best accomplished by encapsulating each model in another object
                // that has a corresponding Solid. In that case, we can init each solid
                // during the InitModels call.
            }
        }

        public PhobicStimulus()
        {
        }

        /// <summary>
        /// Load all the models for the stimulus
        /// </summary>
        protected abstract void InitModels();

        /// <summary>
        /// Call after creating the stimulus to initialize! Only call DURING/AFTER the StereoKit app initialization.
        /// </summary>
        public void Init()
        {
            InitModels();
            ModelIntensity = 1;

            // The initial position of the object
            _solid = new Solid(V.XYZ(0, 0, -1), Quat.Identity);

            // Use the last model's bounds since it looks the best. TODO each model should have it's own bounding box, ideally
            _solid.AddBox(models.Last().Bounds.dimensions * Scale, 1);

            // Set physics off initially to prevent the physics system stepping before
            // the client gets a chance to turn off physics!
            _solid.Enabled = false;
            PhysicsEnabled = false;
        }

        /// <summary>
        /// Step the phobic stimulus.
        /// </summary>
        public virtual void Step()
        {
            roam();

            // Allow user to pick up / move the solid
            Pose sPose = _solid.GetPose();
            if (UI.Handle($"PSTIM_{_id}", ref sPose, _activeModel.Bounds * Scale))
            {
                // We must disable physics when UI handle is in use
                _solid.Enabled = false;
                _isHeld = true;
            }
            else
            {
                // Renable the physics (only if the PhobicStimulus has physics enabled!)
                _solid.Enabled = PhysicsEnabled;
                _isHeld = false;
            }

            // We need the teleport here... not sure why... but it makes it work!
            _solid.Teleport(sPose.position, sPose.orientation);
            _activeModel.Draw(sPose.ToMatrix(Scale));

            if (App.DebugToolsStepper.Enabled)
            {
                debugExtras();
            }

        }

        /// <summary>
        /// Move the object around according to its RoamingMode
        /// </summary>
        private void roam()
        {
            // Roaming movement
            if (RoamingMode == Roaming.Walk)
            {
                if (_isWalking)
                {
                    Pose solidPose = _solid.GetPose();
                    Vec3 forwardPose = solidPose.Forward;

                    float speed = 0.0015f * Scale;
                    Vec3 newPosition = new Vec3();
                    newPosition.x = solidPose.position.x + (forwardPose.x * speed);
                    newPosition.y = solidPose.position.y;
                    newPosition.z = solidPose.position.z + (forwardPose.z * speed);

                    // Rotate to walk in a circle
                    Quat rotateAroundY = (Matrix.R(0, 0.5f, 0) * solidPose.ToMatrix()).Rotation;

                    // Remove wobble
                    rotateAroundY.z = 0;
                    rotateAroundY.x = 0;

                    _solid.Teleport(newPosition, rotateAroundY);
                }

                // Randomly change ambulating status about every 1/300 steps, with a throttle of 3 seconds
                var timeSinceLastChange = DateTime.Now - _lastWalkingChange;
                if (_random.Next(300) == 1 && timeSinceLastChange.TotalSeconds > 3)
                {
                    _isWalking = !_isWalking;
                    syncAnimation();
                    _lastWalkingChange = DateTime.Now;
                }
            }
            else if (RoamingMode == Roaming.Fly)
            {
                Pose solidPose = _solid.GetPose();
                Vec3 forwardPose = solidPose.Forward;

                float speed = 0.003f * Scale;
                _flightTheta += 0.02f;
                Vec3 newPosition = new Vec3();
                newPosition.x = solidPose.position.x + (forwardPose.x * speed);
                newPosition.y = solidPose.position.y + (float) Math.Cos(_flightTheta) * 0.002f * Scale;
                newPosition.z = solidPose.position.z + (forwardPose.z * speed);

                // Rotate to walk in a circle
                Quat rotateAroundY = (Matrix.R(0, 0.7f, 0) * solidPose.ToMatrix()).Rotation;

                // Remove wobble
                rotateAroundY.z = 0;
                rotateAroundY.x = 0;

                _solid.Teleport(newPosition, rotateAroundY);
            }

        }

        /// <summary>
        /// Get the model's position
        /// </summary>
        /// <returns></returns>
        public Vec3 GetPosition()
        {
            return _solid.GetPose().position;
        }

        /// <summary>
        /// Set the model's position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void SetPosition(float x, float y, float z)
        {
            var currentRotation = _solid.GetPose().orientation;
            _solid.Teleport(V.XYZ(x, y, z), currentRotation);

            _debugWindowPose.position.x = x;
            _debugWindowPose.position.y = y;
            _debugWindowPose.position.z = z;
        }

        /// <summary>
        /// Set the model's position
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vec3 position)
        {
            SetPosition(position.x, position.y, position.z);
        }

        /// <summary>
        /// Is the hand touching the phobic stimulus this frame?
        /// </summary>
        /// <returns></returns>
        public bool PatientIsTouching()
        {
            Bounds modelBounds = new Bounds(_solid.GetPose().position, _activeModel.Bounds.dimensions * Scale);

            // Loop over both hands
            for (int h = 0; h < (int)Handed.Max; h++)
            {
                Hand hand = Input.Hand((Handed)h);
                
                // Only consider tracked hands.
                // Untracked hands can still have a position, even if not visible!
                if (hand.IsTracked)
                {
                    // Loop over all 5 fingers
                    for (int f = 0; f <= (int)FingerId.Little; f++)
                    {
                        Pose fingertip = hand[(FingerId)f, JointId.Tip].Pose;
                        if (modelBounds.Contains(fingertip.position))
                            return true;
                    }
                }
            }

            return false;
        }

        public bool PatientIsLooking()
        {
            // Get the 'angle' between the stimulus and patient's forward direction.
            // If patient is generally facing the stimulus, then return true.
            // TODO would be nice to have eye tracking if supported by the device!
            Vec3 patientToStimulusVec = _solid.GetPose().position - Input.Head.position;
            float cosTheta = Vec3.Dot(patientToStimulusVec.Normalized, Input.Head.Forward.Normalized);
            return cosTheta > 0.55f;
        }

        public bool PatientIsHolding()
        {
            return _isHeld;
        }

        /// <summary>
        /// Synchronize the animation with the current activity of the model
        /// </summary>
        private void syncAnimation()
        {
            // last model intensity has animations
            if (_modelIntensity == _maxModelLevel)
            {
                // TODO this probably isn't a good use of reflection! 
                // It would be better to encapsulate the animation within each sub-class
                Type thisType = GetType();
                if (thisType == typeof(Spider))
                {
                    if (AnimationEnabled)
                    {
                        if (_isWalking)
                            _activeModel.PlayAnim("walk_ani_vor", AnimMode.Loop);
                        else
                            _activeModel.PlayAnim("warte_pose", AnimMode.Loop);
                    }
                    else
                    {
                        // I think using AnimMode.Manual will stop the animation?
                        _activeModel.PlayAnim("warte_pose", AnimMode.Manual);
                    }
                }
                else if (thisType == typeof(Bee))
                {
                    if (AnimationEnabled)
                    {
                        _activeModel.PlayAnim("_bee_hover", AnimMode.Loop);
                    }
                    else
                    {
                        // I think using AnimMode.Manual will stop the animation?
                        _activeModel.PlayAnim("_bee_hover", AnimMode.Manual);
                    }
                }
            }
        }

        private void debugExtras()
        {
            // Draw a UI box to visualize the solid
            Pose sPose = _solid.GetPose();
            Mesh box = Mesh.GenerateCube(_activeModel.Bounds.dimensions);
            box.Draw(Material.UIBox, sPose.ToMatrix(Scale));

            // Window for debug controls
            UI.WindowBegin($"PSTIM_DEBUG_{_id}", ref _debugWindowPose);
            UI.Label($"Level: {ModelIntensity}");
            UI.SameLine();
            if (UI.ButtonRound($"Down_{_id}", Asset.Instance.IconDown) && _modelIntensity > 0)
                ModelIntensity--;
            UI.SameLine();
            if (UI.ButtonRound($"Up_{_id}", Asset.Instance.IconUp) && _modelIntensity < _maxModelLevel)
                ModelIntensity++;

            UI.Label($"Type: {GetType()}");
            UI.Label($"Roaming: {RoamingMode}");
            UI.Label($"Position: {GetPosition()}");
            UI.Label($"Distance: {Vec3.Distance(GetPosition(), Input.Head.position)}");
            float scaleVal = Scale;
            UI.Label($"Scale: {scaleVal}");
            UI.HSlider($"Scale_{_id}", ref scaleVal, 0, 10, 0.01f);
            Scale = scaleVal;

            UI.WindowEnd();
        }

    }
}
