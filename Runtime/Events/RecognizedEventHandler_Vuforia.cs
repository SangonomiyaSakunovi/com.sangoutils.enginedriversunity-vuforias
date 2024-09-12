using SangoUtils.Engines_Unity.Utilities;
using UnityEngine;
using UnityEngine.Events;
using Vuforia;

namespace SangoUtils.EngineDrivers_Unity.Vuforias.Recognizables
{
    public class RecognizedEventHandler_Vuforia : MonoBehaviour
    {
        [SerializeField]
        public string method0;
        [SerializeField]
        public MethodParameterType methodParameterType0;
        [SerializeField]
        public int Int0;
        [SerializeField]
        public string String0;
        [SerializeField]
        public float Float0;
        [SerializeField]
        public UnityEngine.Object Object0;

        [SerializeField]
        public string method1; 
        [SerializeField]
        public MethodParameterType methodParameterType1;
        [SerializeField]
        public int Int1;
        [SerializeField]
        public string String1;
        [SerializeField]
        public float Float1;
        [SerializeField]
        public UnityEngine.Object Object1;

        public enum TrackingStatusFilter
        {
            Tracked,
            Tracked_ExtendedTracked,
            Tracked_ExtendedTracked_Limited
        }

        public TrackingStatusFilter StatusFilter = TrackingStatusFilter.Tracked_ExtendedTracked_Limited;

        public bool UsePoseSmoothing = false;
        public AnimationCurve AnimationCurve = AnimationCurve.Linear(0, 0, LERP_DURATION, 1);

        public UnityEvent OnTargetFound = new();
        public UnityEvent OnTargetLost = new();


        protected ObserverBehaviour mObserverBehaviour;
        protected TargetStatus mPreviousTargetStatus = TargetStatus.NotObserved;
        protected bool mCallbackReceivedOnce;

        const float LERP_DURATION = 0.3f;

        PoseSmoother mPoseSmoother;

        protected virtual void Start()
        {
            mObserverBehaviour = GetComponent<ObserverBehaviour>();

            if (mObserverBehaviour)
            {
                mObserverBehaviour.OnTargetStatusChanged += OnObserverStatusChanged;
                mObserverBehaviour.OnBehaviourDestroyed += OnObserverDestroyed;

                OnObserverStatusChanged(mObserverBehaviour, mObserverBehaviour.TargetStatus);
                SetupPoseSmoothing();
            }
        }

        protected virtual void OnDestroy()
        {
            if (VuforiaBehaviour.Instance != null)
                VuforiaBehaviour.Instance.World.OnStateUpdated -= OnStateUpdated;

            if (mObserverBehaviour)
                OnObserverDestroyed(mObserverBehaviour);

            mPoseSmoother?.Dispose();
        }

        private void OnObserverDestroyed(ObserverBehaviour observer)
        {
            mObserverBehaviour.OnTargetStatusChanged -= OnObserverStatusChanged;
            mObserverBehaviour.OnBehaviourDestroyed -= OnObserverDestroyed;
            mObserverBehaviour = null;
        }

        private void OnObserverStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
        {
            var name = mObserverBehaviour.TargetName;
            if (mObserverBehaviour is VuMarkBehaviour vuMarkBehaviour && vuMarkBehaviour.InstanceId != null)
            {
                name += " (" + vuMarkBehaviour.InstanceId + ")";
            }

            Debug.Log($"Target status: {name} {targetStatus.Status} -- {targetStatus.StatusInfo}");

            HandleTargetStatusChanged(mPreviousTargetStatus.Status, targetStatus.Status);
            HandleTargetStatusInfoChanged(targetStatus.StatusInfo);

            mPreviousTargetStatus = targetStatus;
        }

        protected virtual void HandleTargetStatusChanged(Status previousStatus, Status newStatus)
        {
            var shouldBeRendererBefore = ShouldBeRendered(previousStatus);
            var shouldBeRendererNow = ShouldBeRendered(newStatus);
            if (shouldBeRendererBefore != shouldBeRendererNow)
            {
                if (shouldBeRendererNow)
                {
                    OnTrackingFound();
                }
                else
                {
                    OnTrackingLost();
                }
            }
            else
            {
                if (!mCallbackReceivedOnce && !shouldBeRendererNow)
                {
                    // This is the first time we are receiving this callback, and the target is not visible yet.
                    // --> Hide the augmentation.
                    OnTrackingLost();
                }
            }

            mCallbackReceivedOnce = true;
        }

        protected virtual void HandleTargetStatusInfoChanged(StatusInfo newStatusInfo)
        {
            if (newStatusInfo == StatusInfo.WRONG_SCALE)
            {
                Debug.LogErrorFormat("The target {0} appears to be scaled incorrectly. " +
                                     "This might result in tracking issues. " +
                                     "Please make sure that the target size corresponds to the size of the " +
                                     "physical object in meters and regenerate the target or set the correct " +
                                     "size in the target's inspector.", mObserverBehaviour.TargetName);
            }
        }

        protected bool ShouldBeRendered(Status status)
        {
            if (status == Status.TRACKED)
            {
                // always render the augmentation when status is TRACKED, regardless of filter
                return true;
            }

            if (StatusFilter == TrackingStatusFilter.Tracked_ExtendedTracked && status == Status.EXTENDED_TRACKED)
            {
                // also return true if the target is extended tracked
                return true;
            }

            if (StatusFilter == TrackingStatusFilter.Tracked_ExtendedTracked_Limited &&
                (status == Status.EXTENDED_TRACKED || status == Status.LIMITED))
            {
                // in this mode, render the augmentation even if the target's tracking status is LIMITED.
                // this is mainly recommended for Anchors.
                return true;
            }

            return false;
        }


        protected virtual void OnTrackingFound()
        {
            OnTargetFound?.Invoke();
            SendMessage(method0, ArgConvert0(this));
        }

        protected virtual void OnTrackingLost()
        {
            OnTargetLost?.Invoke();
            SendMessage(method1, ArgConvert1(this));
        }

        private static object ArgConvert0(RecognizedEventHandler_Vuforia message) => message.methodParameterType0 switch
        {
            MethodParameterType.Int => message.Int0,
            MethodParameterType.Float => message.Float0,
            MethodParameterType.String => message.String0,
            MethodParameterType.Object => message.Object0,
            _ => null
        };

        private static object ArgConvert1(RecognizedEventHandler_Vuforia message) => message.methodParameterType1 switch
        {
            MethodParameterType.Int => message.Int1,
            MethodParameterType.Float => message.Float1,
            MethodParameterType.String => message.String1,
            MethodParameterType.Object => message.Object1,
            _ => null
        };

        protected void SetupPoseSmoothing()
        {
            UsePoseSmoothing &= VuforiaBehaviour.Instance.WorldCenterMode == WorldCenterMode.DEVICE; // pose smoothing only works with the DEVICE world center mode
            mPoseSmoother = new PoseSmoother(mObserverBehaviour, AnimationCurve);

            VuforiaBehaviour.Instance.World.OnStateUpdated += OnStateUpdated;
        }

        private void OnStateUpdated()
        {
            if (enabled && UsePoseSmoothing)
                mPoseSmoother.Update();
        }


        private class PoseSmoother
        {
            const float e = 0.001f;
            const float MIN_ANGLE = 2f;

            PoseLerp mActivePoseLerp;
            Pose mPreviousPose;

            readonly ObserverBehaviour mTarget;
            readonly AnimationCurve mAnimationCurve;

            TargetStatus mPreviousStatus;

            public PoseSmoother(ObserverBehaviour target, AnimationCurve animationCurve)
            {
                mTarget = target;
                mAnimationCurve = animationCurve;
            }

            public void Update()
            {
                var currentPose = new Pose(mTarget.transform.position, mTarget.transform.rotation);
                var currentStatus = mTarget.TargetStatus;

                UpdatePoseSmoothing(currentPose, currentStatus);

                mPreviousPose = currentPose;
                mPreviousStatus = currentStatus;
            }

            void UpdatePoseSmoothing(Pose currentPose, TargetStatus currentTargetStatus)
            {
                if (mActivePoseLerp == null && ShouldSmooth(currentPose, currentTargetStatus))
                {
                    mActivePoseLerp = new PoseLerp(mPreviousPose, currentPose, mAnimationCurve);
                }

                if (mActivePoseLerp != null)
                {
                    var pose = mActivePoseLerp.GetSmoothedPosition(Time.deltaTime);
                    mTarget.transform.SetPositionAndRotation(pose.position, pose.rotation);

                    if (mActivePoseLerp.Complete)
                    {
                        mActivePoseLerp = null;
                    }
                }
            }

            /// Smooth pose transition if the pose changed and the target is still being reported as "extended tracked" or it has just returned to
            /// "tracked" from previously being "extended tracked"
            bool ShouldSmooth(Pose currentPose, TargetStatus currentTargetStatus)
            {
                return (currentTargetStatus.Status == Status.EXTENDED_TRACKED || (currentTargetStatus.Status == Status.TRACKED && mPreviousStatus.Status == Status.EXTENDED_TRACKED)) &&
                       (Vector3.SqrMagnitude(currentPose.position - mPreviousPose.position) > e || Quaternion.Angle(currentPose.rotation, mPreviousPose.rotation) > MIN_ANGLE);
            }

            public void Dispose()
            {
                mActivePoseLerp = null;
            }
        }

        private class PoseLerp
        {
            readonly AnimationCurve mCurve;
            readonly Pose mStartPose;
            readonly Pose mEndPose;
            readonly float mEndTime;

            float mElapsedTime;

            public bool Complete { get; private set; }

            public PoseLerp(Pose startPose, Pose endPose, AnimationCurve curve)
            {
                mStartPose = startPose;
                mEndPose = endPose;
                mCurve = curve;
                mEndTime = mCurve.keys[mCurve.length - 1].time;
            }

            public Pose GetSmoothedPosition(float deltaTime)
            {
                mElapsedTime += deltaTime;

                if (mElapsedTime >= mEndTime)
                {
                    mElapsedTime = 0;
                    Complete = true;
                    return mEndPose;
                }

                var ratio = mCurve.Evaluate(mElapsedTime);
                var smoothPosition = Vector3.Lerp(mStartPose.position, mEndPose.position, ratio);
                var smoothRotation = Quaternion.Slerp(mStartPose.rotation, mEndPose.rotation, ratio);

                return new Pose(smoothPosition, smoothRotation);
            }
        }
    }
}
