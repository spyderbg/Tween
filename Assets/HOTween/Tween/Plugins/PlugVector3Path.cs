using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins.Core;
using System;
using System.Reflection;
using UnityEngine;

namespace Holoville.HOTween.Plugins {

public class PlugVector3Path : ABSTweenPlugin
{
    private const int kSubdivisionsMultiplier = 16;
    private const float kEpsilon = 0.001f;
    private const float kMINLookahead = 0.0001f;
    private const float kMAXLookahed = 0.9999f;

    internal static Type[] ValidPropTypes = new Type[1]
    {
        typeof(Vector3)
    };

    internal static Type[] ValidValueTypes = new Type[1]
    {
        typeof(Vector3[])
    };

    internal Path Path;
    internal float PathPerc;
    internal bool HasAdditionalStartingP;
    internal bool IsClosedPath;
    
    private Vector3 _typedStartVal;
    private Vector3[] _points;
    private Vector3 _diffChangeVal;
    private OrientType _orientType;
    private float _lookAheadVal = 0.0001f;
    private Axis _lockPositionAxis;
    private Axis _lockRotationAxis;
    private bool _isPartialPath;
    private bool _usesLocalPosition;
    private float _startPerc;
    private float _changePerc = 1f;
    private Vector3 _lookPos;
    private Transform _lookTrans;
    private Transform _orientTrans;

    internal PathType pathType { get; private set; }

    protected override object startVal
    {
        get => StartVal;
        set
        {
            if (TweenObj.isFrom)
            {
                EndVal = value;
                var vector3Array = (Vector3[])value;
                _points = new Vector3[vector3Array.Length];
                Array.Copy(vector3Array, _points, vector3Array.Length);
                Array.Reverse(_points);
            }
            else
                StartVal = _typedStartVal = (Vector3)value;
        }
    }

    protected override object endVal
    {
        get => EndVal;
        set
        {
            if (TweenObj.isFrom)
            {
                StartVal = _typedStartVal = (Vector3)value;
            }
            else
            {
                EndVal = value;
                var vector3Array = (Vector3[])value;
                _points = new Vector3[vector3Array.Length];
                Array.Copy(vector3Array, _points, vector3Array.Length);
            }
        }
    }

    public PlugVector3Path(Vector3[] path, PathType type = PathType.Curved)
        : base(path, false)
    {
        pathType = type;
    }

    public PlugVector3Path(Vector3[] path, EaseType easeType, PathType type = PathType.Curved)
        : base(path, easeType, false)
    {
        pathType = type;
    }

    public PlugVector3Path(Vector3[] path, bool isRelative, PathType type = PathType.Curved)
        : base(path, isRelative)
    {
        pathType = type;
    }

    public PlugVector3Path(Vector3[] path, EaseType easeType, bool isRelative, PathType type = PathType.Curved) 
        : base(path, easeType, isRelative)
    {
        pathType = type;
    }

    public PlugVector3Path(Vector3[] path, AnimationCurve easeAnimCurve, bool isRelative, PathType type = PathType.Curved) 
        : base(path, easeAnimCurve, isRelative)
    {
        pathType = type;
    }

    internal override void Init(Tweener tweenObj, string propertyName, EaseType easeType, Type targetType, PropertyInfo propertyInfo, FieldInfo fieldInfo)
    {
        if (IsRelative && tweenObj.isFrom)
        {
            IsRelative = false;
            TweenWarning.Log("\"" + tweenObj.target + "." + propertyName +
                             "\": PlugVector3Path \"isRelative\" parameter is incompatible with HOTween.From. The tween will be treated as absolute.");
        }

        _usesLocalPosition = propertyName == "localPosition";
        base.Init(tweenObj, propertyName, easeType, targetType, propertyInfo, fieldInfo);
    }

    public PlugVector3Path ClosePath() =>
        ClosePath(true);

    public PlugVector3Path ClosePath(bool close)
    {
        IsClosedPath = close;
        return this;
    }

    public PlugVector3Path OrientToPath() =>
        OrientToPath(true);

    public PlugVector3Path OrientToPath(bool orient) =>
        OrientToPath(orient, 0.0001f, Axis.None);

    public PlugVector3Path OrientToPath(float lookAhead) =>
        OrientToPath(true, lookAhead, Axis.None);

    public PlugVector3Path OrientToPath(Axis lockRotationAxis) =>
        OrientToPath(true, 0.0001f, lockRotationAxis);

    public PlugVector3Path OrientToPath(float lookAhead, Axis lockRotationAxis) =>
        OrientToPath(true, lookAhead, lockRotationAxis);

    public PlugVector3Path OrientToPath(bool orient, float lookAhead, Axis lockRotationAxis)
    {
        if (orient)
            _orientType = OrientType.ToPath;
        _lookAheadVal = lookAhead;
        if (_lookAheadVal < 9.99999974737875E-05)
            _lookAheadVal = 0.0001f;
        else if (_lookAheadVal > 0.999899983406067)
            _lookAheadVal = 0.9999f;
        _lockRotationAxis = lockRotationAxis;
        return this;
    }

    public PlugVector3Path LookAt(Transform transform)
    {
        if (transform != null)
        {
            _orientType = OrientType.LookAtTransform;
            _lookTrans = transform;
        }

        return this;
    }

    public PlugVector3Path LookAt(Vector3 position)
    {
        _orientType = OrientType.LookAtPosition;
        _lookPos = position;
        _lookTrans = null;
        return this;
    }

    public PlugVector3Path LockPosition(Axis lockAxis)
    {
        _lockPositionAxis = lockAxis;
        return this;
    }

    protected override float GetSpeedBasedDuration(float speed) => Path.pathLength / speed;

    protected override void SetChangeVal()
    {
        if (_orientType != OrientType.None && _orientTrans == null)
            _orientTrans = TweenObj.target as Transform;
        var num1 = 1;
        var num2 = IsClosedPath ? 1 : 0;
        var length1 = _points.Length;
        Vector3[] vector3Array;
        if (IsRelative)
        {
            HasAdditionalStartingP = false;
            var vector3 = _points[0] - _typedStartVal;
            vector3Array = new Vector3[length1 + 2 + num2];
            for (var index = 0; index < length1; ++index)
                vector3Array[index + num1] = _points[index] - vector3;
        }
        else
        {
            var vector31 = (Vector3)GetValue();
            var vector32 = vector31 - _points[0];
            if (vector32.x < 0.0)
                vector32.x = -vector32.x;
            if (vector32.y < 0.0)
                vector32.y = -vector32.y;
            if (vector32.z < 0.0)
                vector32.z = -vector32.z;
            if (vector32.x < 1.0 / 1000.0 && vector32.y < 1.0 / 1000.0 && vector32.z < 1.0 / 1000.0)
            {
                HasAdditionalStartingP = false;
                vector3Array = new Vector3[length1 + 2 + num2];
            }
            else
            {
                HasAdditionalStartingP = true;
                vector3Array = new Vector3[length1 + 3 + num2];
                if (TweenObj.isFrom)
                {
                    vector3Array[vector3Array.Length - 2] = vector31;
                }
                else
                {
                    vector3Array[1] = vector31;
                    num1 = 2;
                }
            }

            for (var index = 0; index < length1; ++index)
                vector3Array[index + num1] = _points[index];
        }

        var length2 = vector3Array.Length;
        if (IsClosedPath)
            vector3Array[length2 - 2] = vector3Array[1];
        if (IsClosedPath)
        {
            vector3Array[0] = vector3Array[length2 - 3];
            vector3Array[length2 - 1] = vector3Array[2];
        }
        else
        {
            vector3Array[0] = vector3Array[1];
            var vector31 = vector3Array[length2 - 2];
            var vector32 = vector31 - vector3Array[length2 - 3];
            vector3Array[length2 - 1] = vector31 + vector32;
        }

        if (_lockPositionAxis != Axis.None)
        {
            var flag1 = (_lockPositionAxis & Axis.X) == Axis.X;
            var flag2 = (_lockPositionAxis & Axis.Y) == Axis.Y;
            var flag3 = (_lockPositionAxis & Axis.Z) == Axis.Z;
            var typedStartVal = _typedStartVal;
            for (var index = 0; index < length2; ++index)
            {
                var vector3 = vector3Array[index];
                vector3Array[index] = new Vector3(flag1 ? typedStartVal.x : vector3.x,
                    flag2 ? typedStartVal.y : vector3.y, flag3 ? typedStartVal.z : vector3.z);
            }
        }

        Path = new Path(pathType, vector3Array);
        Path.StoreTimeToLenTables(Path.path.Length * 16);
        if (IsClosedPath)
            return;
        _diffChangeVal = vector3Array[length2 - 2] - vector3Array[1];
    }

    protected override void SetIncremental(int diffIncr)
    {
        if (IsClosedPath)
            return;
        var path = Path.path;
        var length = path.Length;
        for (var index = 0; index < length; ++index)
            path[index] += _diffChangeVal * diffIncr;
        Path.changed = true;
    }

    protected override void DoUpdate(float totElapsed)
    {
        PathPerc = Ease(totElapsed, _startPerc, _changePerc, Duration, TweenObj.easeOvershootOrAmplitude, TweenObj.easePeriod);
        var constPointOnPath = GetConstPointOnPath(PathPerc, true, Path, out var outWaypointIndex);
        SetValue(constPointOnPath);
        if (_orientType == OrientType.None || !(_orientTrans != null) || _orientTrans.Equals(null))
            return;
        var transform = _usesLocalPosition ? _orientTrans.parent : null;
        switch (_orientType)
        {
            case OrientType.ToPath:
                Vector3 vector3;
                if (pathType == PathType.Linear && _lookAheadVal <= 9.99999974737875E-05)
                {
                    vector3 = constPointOnPath + Path.path[outWaypointIndex] - Path.path[outWaypointIndex - 1];
                }
                else
                {
                    var t = PathPerc + _lookAheadVal;
                    if (t > 1.0)
                        t = IsClosedPath ? t - 1f : 1.000001f;
                    vector3 = Path.GetPoint(t);
                }

                var worldUp = _orientTrans.up;
                if (_usesLocalPosition && transform != null)
                    vector3 = transform.TransformPoint(vector3);
                if (_lockRotationAxis != Axis.None && _orientTrans != null)
                {
                    if ((_lockRotationAxis & Axis.X) == Axis.X)
                    {
                        var position = _orientTrans.InverseTransformPoint(vector3);
                        position.y = 0.0f;
                        vector3 = _orientTrans.TransformPoint(position);
                        worldUp = !_usesLocalPosition || !(transform != null) ? Vector3.up : transform.up;
                    }

                    if ((_lockRotationAxis & Axis.Y) == Axis.Y)
                    {
                        var position = _orientTrans.InverseTransformPoint(vector3);
                        if (position.z < 0.0)
                            position.z = -position.z;
                        position.x = 0.0f;
                        vector3 = _orientTrans.TransformPoint(position);
                    }

                    if ((_lockRotationAxis & Axis.Z) == Axis.Z)
                        worldUp = !_usesLocalPosition || !(transform != null) ? Vector3.up : transform.up;
                }

                _orientTrans.LookAt(vector3, worldUp);
                break;
            case OrientType.LookAtTransform:
                if (!(_orientTrans != null) || _orientTrans.Equals(null))
                    break;
                _orientTrans.LookAt(_lookTrans.position, Vector3.up);
                break;
            case OrientType.LookAtPosition:
                _orientTrans.LookAt(_lookPos, Vector3.up);
                break;
        }
    }

    internal override void Rewind()
    {
        if (_isPartialPath)
            DoUpdate(0.0f);
        else
            base.Rewind();
    }

    internal override void Complete()
    {
        if (_isPartialPath)
            DoUpdate(Duration);
        else
            base.Complete();
    }

    internal Vector3 GetConstPointOnPath(float t) =>
        GetConstPointOnPath(t, false, null, out var _);

    internal Vector3 GetConstPointOnPath(float t, bool updatePathPerc, Path path, out int outWaypointIndex)
    {
        if (updatePathPerc)
            return path.GetConstPoint(t, out PathPerc, out outWaypointIndex);
        outWaypointIndex = -1;
        return Path.GetConstPoint(t);
    }

    internal float GetWaypointsLengthPercentage(int pathWaypointId0, int pathWaypointId1)
    {
        if (pathType == PathType.Linear)
        {
            if (Path.waypointsLength == null)
                Path.StoreWaypointsLengths(16);
            return Path.timesTable[pathWaypointId1] - Path.timesTable[pathWaypointId0];
        }

        if (Path.waypointsLength == null)
            Path.StoreWaypointsLengths(16);
        var num1 = 0.0f;
        for (var index = pathWaypointId0; index < pathWaypointId1; ++index)
            num1 += Path.waypointsLength[index];
        var num2 = num1 / Path.pathLength;
        if (num2 > 1.0)
            num2 = 1f;
        return num2;
    }

    private bool IsWaypoint(Vector3 position, out int waypointIndex)
    {
        var length = Path.path.Length;
        for (var index = 0; index < length; ++index)
        {
            var num1 = Path.path[index].x - position.x;
            var num2 = Path.path[index].y - position.y;
            var num3 = Path.path[index].z - position.z;
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

    internal void SwitchToPartialPath(float duration, EaseType easeType, float partialStartPerc, float partialChangePerc)
    {
        _isPartialPath = true;
        Duration = duration;
        SetEase(easeType);
        _startPerc = partialStartPerc;
        _changePerc = partialChangePerc;
    }

    internal void ResetToFullPath(float duration, EaseType easeType)
    {
        _isPartialPath = false;
        Duration = duration;
        SetEase(easeType);
        _startPerc = 0.0f;
        _changePerc = 1f;
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