using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins.Core;
using System;
using System.Reflection;
using UnityEngine;

namespace Holoville.HOTween.Plugins
{
    /// <summary>
    /// Plugin for the tweening of Vector3 objects along a Vector3 path.
    /// </summary>
    public class PlugVector3Path : ABSTweenPlugin
    {
        private const int SUBDIVISIONS_MULTIPLIER = 16;
        private const float EPSILON = 0.001f;
        private const float MIN_LOOKAHEAD = 0.0001f;
        private const float MAX_LOOKAHED = 0.9999f;

        internal static Type[] validPropTypes = new Type[1]
        {
            typeof(Vector3)
        };

        internal static Type[] validValueTypes = new Type[1]
        {
            typeof(Vector3[])
        };

        internal Path path;
        internal float pathPerc;
        internal bool hasAdditionalStartingP;
        private Vector3 typedStartVal;
        private Vector3[] points;
        private Vector3 diffChangeVal;
        internal bool isClosedPath;
        private OrientType orientType;
        private float lookAheadVal = 0.0001f;
        private Axis lockPositionAxis;
        private Axis lockRotationAxis;
        private bool isPartialPath;
        private bool usesLocalPosition;
        private float startPerc;
        private float changePerc = 1f;
        private Vector3 lookPos;
        private Transform lookTrans;
        private Transform orientTrans;

        internal PathType pathType { get; private set; }

        /// <summary>
        /// Gets the untyped start value,
        /// sets both the untyped and the typed start value.
        /// </summary>
        protected override object startVal
        {
            get => _startVal;
            set
            {
                if (tweenObj.isFrom)
                {
                    _endVal = value;
                    var vector3Array = (Vector3[])value;
                    points = new Vector3[vector3Array.Length];
                    Array.Copy(vector3Array, points, vector3Array.Length);
                    Array.Reverse(points);
                }
                else
                    _startVal = typedStartVal = (Vector3)value;
            }
        }

        /// <summary>
        /// Gets the untyped end value,
        /// sets both the untyped and the typed end value.
        /// </summary>
        protected override object endVal
        {
            get => _endVal;
            set
            {
                if (tweenObj.isFrom)
                {
                    _startVal = typedStartVal = (Vector3)value;
                }
                else
                {
                    _endVal = value;
                    var vector3Array = (Vector3[])value;
                    points = new Vector3[vector3Array.Length];
                    Array.Copy(vector3Array, points, vector3Array.Length);
                }
            }
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type and an absolute path.
        /// </summary>
        /// <param name="p_path">
        /// The <see cref="T:UnityEngine.Vector3" /> path to tween through.
        /// </param>
        /// <param name="p_type">Type of path</param>
        public PlugVector3Path(Vector3[] p_path, PathType p_type = PathType.Curved)
            : base(p_path, false)
        {
            pathType = p_type;
        }

        /// <summary>
        /// Creates a new instance of this plugin using an absolute path.
        /// </summary>
        /// <param name="p_path">
        /// The <see cref="T:UnityEngine.Vector3" /> path to tween through.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_type">Type of path</param>
        public PlugVector3Path(Vector3[] p_path, EaseType p_easeType, PathType p_type = PathType.Curved)
            : base(p_path, p_easeType, false)
        {
            pathType = p_type;
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_path">
        /// The <see cref="T:UnityEngine.Vector3" /> path to tween through.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the path is considered relative to the starting value of the property, instead than absolute.
        /// Not compatible with <c>HOTween.From</c>.
        /// </param>
        /// <param name="p_type">Type of path</param>
        public PlugVector3Path(Vector3[] p_path, bool p_isRelative, PathType p_type = PathType.Curved)
            : base(p_path, p_isRelative)
        {
            pathType = p_type;
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_path">
        /// The <see cref="T:UnityEngine.Vector3" /> path to tween through.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the path is considered relative to the starting value of the property, instead than absolute.
        /// Not compatible with <c>HOTween.From</c>.
        /// </param>
        /// <param name="p_type">Type of path</param>
        public PlugVector3Path(
            Vector3[] p_path,
            EaseType p_easeType,
            bool p_isRelative,
            PathType p_type = PathType.Curved)
            : base(p_path, p_easeType, p_isRelative)
        {
            pathType = p_type;
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_path">
        /// The <see cref="T:UnityEngine.Vector3" /> path to tween through.
        /// </param>
        /// <param name="p_easeAnimCurve">
        /// The <see cref="T:UnityEngine.AnimationCurve" /> to use for easing.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        /// <param name="p_type">Type of path</param>
        public PlugVector3Path(
            Vector3[] p_path,
            AnimationCurve p_easeAnimCurve,
            bool p_isRelative,
            PathType p_type = PathType.Curved)
            : base(p_path, p_easeAnimCurve, p_isRelative)
        {
            pathType = p_type;
        }

        /// <summary>
        /// Init override.
        /// Used to check that isRelative is FALSE,
        /// and otherwise use the given parameters to send a decent warning message.
        /// </summary>
        internal override void Init(
            Tweener p_tweenObj,
            string p_propertyName,
            EaseType p_easeType,
            Type p_targetType,
            PropertyInfo p_propertyInfo,
            FieldInfo p_fieldInfo)
        {
            if (isRelative && p_tweenObj.isFrom)
            {
                isRelative = false;
                TweenWarning.Log("\"" + p_tweenObj.target + "." + p_propertyName +
                                 "\": PlugVector3Path \"isRelative\" parameter is incompatible with HOTween.From. The tween will be treated as absolute.");
            }

            usesLocalPosition = p_propertyName == "localPosition";
            base.Init(p_tweenObj, p_propertyName, p_easeType, p_targetType, p_propertyInfo, p_fieldInfo);
        }

        /// <summary>
        /// Parameter &gt; Smoothly closes the path, so that it can be used for cycling loops.
        /// </summary>
        /// <returns>
        /// A <see cref="T:Holoville.HOTween.Plugins.PlugVector3Path" />
        /// </returns>
        public PlugVector3Path ClosePath() => ClosePath(true);

        /// <summary>
        /// Parameter &gt; Choose whether to smoothly close the path, so that it can be used for cycling loops.
        /// </summary>
        /// <param name="p_close">
        /// Set to <c>true</c> to close the path.
        /// </param>
        public PlugVector3Path ClosePath(bool p_close)
        {
            isClosedPath = p_close;
            return this;
        }

        /// <summary>
        /// Parameter &gt; If the tween target is a <see cref="T:UnityEngine.Transform" />, orients the tween target to the path.
        /// </summary>
        /// <returns>
        /// A <see cref="T:Holoville.HOTween.Plugins.PlugVector3Path" />
        /// </returns>
        public PlugVector3Path OrientToPath() => OrientToPath(true);

        /// <summary>
        /// Parameter &gt; Choose whether to orient the tween target to the path (only if it's a <see cref="T:UnityEngine.Transform" />).
        /// </summary>
        /// <param name="p_orient">
        /// Set to <c>true</c> to orient the tween target to the path.
        /// </param>
        public PlugVector3Path OrientToPath(bool p_orient) => OrientToPath(p_orient, 0.0001f, Axis.None);

        /// <summary>
        /// Parameter &gt; If the tween target is a <see cref="T:UnityEngine.Transform" />, orients the tween target to the path,
        /// using the given lookAhead percentage.
        /// </summary>
        /// <param name="p_lookAhead">The look ahead percentage (0 to 1).</param>
        public PlugVector3Path OrientToPath(float p_lookAhead) => OrientToPath(true, p_lookAhead, Axis.None);

        /// <summary>
        /// Parameter &gt; If the tween target is a <see cref="T:UnityEngine.Transform" />, orients the tween target to the path,
        /// locking its rotation on the given axis.
        /// </summary>
        /// <param name="p_lockRotationAxis">
        /// Sets one or more axis to lock while rotating.
        /// To lock more than one axis, use the bitwise OR operator (ex: <c>Axis.X | Axis.Y</c>).
        /// </param>
        public PlugVector3Path OrientToPath(Axis p_lockRotationAxis) => OrientToPath(true, 0.0001f, p_lockRotationAxis);

        /// <summary>
        /// Parameter &gt; If the tween target is a <see cref="T:UnityEngine.Transform" />, orients the tween target to the path,
        /// using the given lookAhead percentage and locking its rotation on the given axis.
        /// </summary>
        /// <param name="p_lookAhead">The look ahead percentage (0 to 1)</param>
        /// <param name="p_lockRotationAxis">
        /// Sets one or more axis to lock while rotating.
        /// To lock more than one axis, use the bitwise OR operator (ex: <c>Axis.X | Axis.Y</c>).
        /// </param>
        public PlugVector3Path OrientToPath(float p_lookAhead, Axis p_lockRotationAxis) =>
            OrientToPath(true, p_lookAhead, p_lockRotationAxis);

        /// <summary>
        /// Parameter &gt; Choose whether to orient the tween target to the path (only if it's a <see cref="T:UnityEngine.Transform" />),
        /// and which lookAhead percentage ad lockRotation to use.
        /// </summary>
        /// <param name="p_orient">
        /// Set to <c>true</c> to orient the tween target to the path.
        /// </param>
        /// <param name="p_lookAhead">The look ahead percentage (0 to 1).</param>
        /// <param name="p_lockRotationAxis">
        /// Sets one or more axis to lock while rotating.
        /// To lock more than one axis, use the bitwise OR operator (ex: <c>Axis.X | Axis.Y</c>).
        /// </param>
        public PlugVector3Path OrientToPath(
            bool p_orient,
            float p_lookAhead,
            Axis p_lockRotationAxis)
        {
            if (p_orient)
                orientType = OrientType.ToPath;
            lookAheadVal = p_lookAhead;
            if (lookAheadVal < 9.99999974737875E-05)
                lookAheadVal = 0.0001f;
            else if (lookAheadVal > 0.999899983406067)
                lookAheadVal = 0.9999f;
            lockRotationAxis = p_lockRotationAxis;
            return this;
        }

        /// <summary>
        /// Parameter &gt; If the tween target is a <see cref="T:UnityEngine.Transform" />, sets the tween so that the target will always look at the given transform.
        /// </summary>
        /// <param name="p_transform">
        /// The <see cref="T:UnityEngine.Transform" /> to look at.
        /// </param>
        public PlugVector3Path LookAt(Transform p_transform)
        {
            if (p_transform != null)
            {
                orientType = OrientType.LookAtTransform;
                lookTrans = p_transform;
            }

            return this;
        }

        /// <summary>
        /// Parameter &gt; If the tween target is a <see cref="T:UnityEngine.Transform" />, sets the tween so that the target will always look at the given position.
        /// </summary>
        /// <param name="p_position">
        /// The <see cref="T:UnityEngine.Vector3" /> to look at.
        /// </param>
        public PlugVector3Path LookAt(Vector3 p_position)
        {
            orientType = OrientType.LookAtPosition;
            lookPos = p_position;
            lookTrans = null;
            return this;
        }

        /// <summary>Parameter &gt; locks the given position axis.</summary>
        /// <param name="p_lockAxis">Sets one or more axis to lock.
        /// To lock more than one axis, use the bitwise OR operator (ex: <c>Axis.X | Axis.Y</c>)</param>
        /// <returns></returns>
        public PlugVector3Path LockPosition(Axis p_lockAxis)
        {
            lockPositionAxis = p_lockAxis;
            return this;
        }

        /// <summary>
        /// Returns the speed-based duration based on the given speed x second.
        /// </summary>
        protected override float GetSpeedBasedDuration(float p_speed) => path.pathLength / p_speed;

        /// <summary>
        /// Adds the correct starting and ending point so the path can be reached from the property's actual position.
        /// </summary>
        protected override void SetChangeVal()
        {
            if (orientType != OrientType.None && orientTrans == null)
                orientTrans = tweenObj.target as Transform;
            var num1 = 1;
            var num2 = isClosedPath ? 1 : 0;
            var length1 = points.Length;
            Vector3[] vector3Array;
            if (isRelative)
            {
                hasAdditionalStartingP = false;
                var vector3 = points[0] - typedStartVal;
                vector3Array = new Vector3[length1 + 2 + num2];
                for (var index = 0; index < length1; ++index)
                    vector3Array[index + num1] = points[index] - vector3;
            }
            else
            {
                var vector3_1 = (Vector3)GetValue();
                var vector3_2 = vector3_1 - points[0];
                if (vector3_2.x < 0.0)
                    vector3_2.x = -vector3_2.x;
                if (vector3_2.y < 0.0)
                    vector3_2.y = -vector3_2.y;
                if (vector3_2.z < 0.0)
                    vector3_2.z = -vector3_2.z;
                if (vector3_2.x < 1.0 / 1000.0 && vector3_2.y < 1.0 / 1000.0 && vector3_2.z < 1.0 / 1000.0)
                {
                    hasAdditionalStartingP = false;
                    vector3Array = new Vector3[length1 + 2 + num2];
                }
                else
                {
                    hasAdditionalStartingP = true;
                    vector3Array = new Vector3[length1 + 3 + num2];
                    if (tweenObj.isFrom)
                    {
                        vector3Array[vector3Array.Length - 2] = vector3_1;
                    }
                    else
                    {
                        vector3Array[1] = vector3_1;
                        num1 = 2;
                    }
                }

                for (var index = 0; index < length1; ++index)
                    vector3Array[index + num1] = points[index];
            }

            var length2 = vector3Array.Length;
            if (isClosedPath)
                vector3Array[length2 - 2] = vector3Array[1];
            if (isClosedPath)
            {
                vector3Array[0] = vector3Array[length2 - 3];
                vector3Array[length2 - 1] = vector3Array[2];
            }
            else
            {
                vector3Array[0] = vector3Array[1];
                var vector3_1 = vector3Array[length2 - 2];
                var vector3_2 = vector3_1 - vector3Array[length2 - 3];
                vector3Array[length2 - 1] = vector3_1 + vector3_2;
            }

            if (lockPositionAxis != Axis.None)
            {
                var flag1 = (lockPositionAxis & Axis.X) == Axis.X;
                var flag2 = (lockPositionAxis & Axis.Y) == Axis.Y;
                var flag3 = (lockPositionAxis & Axis.Z) == Axis.Z;
                var typedStartVal = this.typedStartVal;
                for (var index = 0; index < length2; ++index)
                {
                    var vector3 = vector3Array[index];
                    vector3Array[index] = new Vector3(flag1 ? typedStartVal.x : vector3.x,
                        flag2 ? typedStartVal.y : vector3.y, flag3 ? typedStartVal.z : vector3.z);
                }
            }

            path = new Path(pathType, vector3Array);
            path.StoreTimeToLenTables(path.path.Length * 16);
            if (isClosedPath)
                return;
            diffChangeVal = vector3Array[length2 - 2] - vector3Array[1];
        }

        /// <summary>
        /// Sets the correct values in case of Incremental loop type.
        /// </summary>
        /// <param name="p_diffIncr">
        /// The difference from the previous loop increment.
        /// </param>
        protected override void SetIncremental(int p_diffIncr)
        {
            if (isClosedPath)
                return;
            var path = this.path.path;
            var length = path.Length;
            for (var index = 0; index < length; ++index)
                path[index] += diffChangeVal * p_diffIncr;
            this.path.changed = true;
        }

        /// <summary>Updates the tween.</summary>
        /// <param name="p_totElapsed">The total elapsed time since startup.</param>
        protected override void DoUpdate(float p_totElapsed)
        {
            pathPerc = ease(p_totElapsed, startPerc, changePerc, _duration, tweenObj.easeOvershootOrAmplitude,
                tweenObj.easePeriod);
            int out_waypointIndex;
            var constPointOnPath = GetConstPointOnPath(pathPerc, true, path, out out_waypointIndex);
            SetValue(constPointOnPath);
            if (orientType == OrientType.None || !(orientTrans != null) || orientTrans.Equals(null))
                return;
            var transform = usesLocalPosition ? orientTrans.parent : null;
            switch (orientType)
            {
                case OrientType.ToPath:
                    Vector3 vector3;
                    if (pathType == PathType.Linear && lookAheadVal <= 9.99999974737875E-05)
                    {
                        vector3 = constPointOnPath + path.path[out_waypointIndex] - path.path[out_waypointIndex - 1];
                    }
                    else
                    {
                        var t = pathPerc + lookAheadVal;
                        if (t > 1.0)
                            t = isClosedPath ? t - 1f : 1.000001f;
                        vector3 = path.GetPoint(t);
                    }

                    var worldUp = orientTrans.up;
                    if (usesLocalPosition && transform != null)
                        vector3 = transform.TransformPoint(vector3);
                    if (lockRotationAxis != Axis.None && orientTrans != null)
                    {
                        if ((lockRotationAxis & Axis.X) == Axis.X)
                        {
                            var position = orientTrans.InverseTransformPoint(vector3);
                            position.y = 0.0f;
                            vector3 = orientTrans.TransformPoint(position);
                            worldUp = !usesLocalPosition || !(transform != null) ? Vector3.up : transform.up;
                        }

                        if ((lockRotationAxis & Axis.Y) == Axis.Y)
                        {
                            var position = orientTrans.InverseTransformPoint(vector3);
                            if (position.z < 0.0)
                                position.z = -position.z;
                            position.x = 0.0f;
                            vector3 = orientTrans.TransformPoint(position);
                        }

                        if ((lockRotationAxis & Axis.Z) == Axis.Z)
                            worldUp = !usesLocalPosition || !(transform != null) ? Vector3.up : transform.up;
                    }

                    orientTrans.LookAt(vector3, worldUp);
                    break;
                case OrientType.LookAtTransform:
                    if (!(orientTrans != null) || orientTrans.Equals(null))
                        break;
                    orientTrans.LookAt(lookTrans.position, Vector3.up);
                    break;
                case OrientType.LookAtPosition:
                    orientTrans.LookAt(lookPos, Vector3.up);
                    break;
            }
        }

        internal override void Rewind()
        {
            if (isPartialPath)
                DoUpdate(0.0f);
            else
                base.Rewind();
        }

        internal override void Complete()
        {
            if (isPartialPath)
                DoUpdate(_duration);
            else
                base.Complete();
        }

        /// <summary>
        /// Returns the point at the given percentage (0 to 1),
        /// considering the path at constant speed.
        /// Used by DoUpdate and by Tweener.GetPointOnPath.
        /// </summary>
        /// <param name="t">
        /// The percentage (0 to 1) at which to get the point.
        /// </param>
        internal Vector3 GetConstPointOnPath(float t) => GetConstPointOnPath(t, false, null, out var _);

        /// <summary>
        /// Returns the point at the given percentage (0 to 1),
        /// considering the path at constant speed.
        /// Used by DoUpdate and by Tweener.GetPointOnPath.
        /// </summary>
        /// <param name="t">
        /// The percentage (0 to 1) at which to get the point.
        /// </param>
        /// <param name="p_updatePathPerc">
        /// IF <c>true</c> updates also <see cref="F:Holoville.HOTween.Plugins.PlugVector3Path.pathPerc" /> value
        /// (necessary if this method is called for an update).
        /// </param>
        /// <param name="p_path">
        /// IF not NULL uses the given path instead than the default one.
        /// </param>
        /// <param name="out_waypointIndex">
        /// Index of waypoint we're moving to (or where we are). Only used for Linear paths.
        /// </param>
        internal Vector3 GetConstPointOnPath(
            float t,
            bool p_updatePathPerc,
            Path p_path,
            out int out_waypointIndex)
        {
            if (p_updatePathPerc)
                return p_path.GetConstPoint(t, out pathPerc, out out_waypointIndex);
            out_waypointIndex = -1;
            return path.GetConstPoint(t);
        }

        /// <summary>
        /// Returns the percentage of the path length occupied by the given path waypoints interval.
        /// </summary>
        internal float GetWaypointsLengthPercentage(int p_pathWaypointId0, int p_pathWaypointId1)
        {
            if (pathType == PathType.Linear)
            {
                if (path.waypointsLength == null)
                    path.StoreWaypointsLengths(16);
                return path.timesTable[p_pathWaypointId1] - path.timesTable[p_pathWaypointId0];
            }

            if (path.waypointsLength == null)
                path.StoreWaypointsLengths(16);
            var num1 = 0.0f;
            for (var index = p_pathWaypointId0; index < p_pathWaypointId1; ++index)
                num1 += path.waypointsLength[index];
            var num2 = num1 / path.pathLength;
            if (num2 > 1.0)
                num2 = 1f;
            return num2;
        }

        private bool IsWaypoint(Vector3 position, out int waypointIndex)
        {
            var length = path.path.Length;
            for (var index = 0; index < length; ++index)
            {
                var num1 = path.path[index].x - position.x;
                var num2 = path.path[index].y - position.y;
                var num3 = path.path[index].z - position.z;
                if (num1 < 0.0)
                    num1 = 0.0f;
                if (num2 < 0.0)
                    num2 = 0.0f;
                if (num3 < 0.0)
                    num3 = 0.0f;
                if (num1 < 1.0 / 1000.0 && num2 < 1.0 / 1000.0 && num3 < 1.0 / 1000.0)
                {
                    waypointIndex = index;
                    return true;
                }
            }

            waypointIndex = -1;
            return false;
        }

        internal void SwitchToPartialPath(
            float p_duration,
            EaseType p_easeType,
            float p_partialStartPerc,
            float p_partialChangePerc)
        {
            isPartialPath = true;
            _duration = p_duration;
            SetEase(p_easeType);
            startPerc = p_partialStartPerc;
            changePerc = p_partialChangePerc;
        }

        internal void ResetToFullPath(float p_duration, EaseType p_easeType)
        {
            isPartialPath = false;
            _duration = p_duration;
            SetEase(p_easeType);
            startPerc = 0.0f;
            changePerc = 1f;
        }

        private enum OrientType
        {
            None,
            ToPath,
            LookAtTransform,
            LookAtPosition,
        }
    }
}