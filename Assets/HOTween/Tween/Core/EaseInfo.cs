using Holoville.HOTween.Core.Easing;

namespace Holoville.HOTween.Core {

public class EaseInfo
{
    public TweenDelegate.EaseFunc Ease;

    public TweenDelegate.EaseFunc InverseEase;

    private static readonly EaseInfo EaseInSineInfo = new EaseInfo(Sine.EaseIn, Sine.EaseOut);

    private static readonly EaseInfo EaseOutSineInfo = new EaseInfo(Sine.EaseOut, Sine.EaseIn);

    private static readonly EaseInfo EaseInOutSineInfo = new EaseInfo(Sine.EaseInOut, null);

    private static readonly EaseInfo EaseInQuadInfo = new EaseInfo(Quad.EaseIn, Quad.EaseOut);

    private static readonly EaseInfo EaseOutQuadInfo = new EaseInfo(Quad.EaseOut, Quad.EaseIn);

    private static readonly EaseInfo EaseInOutQuadInfo = new EaseInfo(Quad.EaseInOut, null);

    private static readonly EaseInfo EaseInCubicInfo = new EaseInfo(Cubic.EaseIn, Cubic.EaseOut);

    private static readonly EaseInfo EaseOutCubicInfo = new EaseInfo(Cubic.EaseOut, Cubic.EaseIn);

    private static readonly EaseInfo EaseInOutCubicInfo = new EaseInfo(Cubic.EaseInOut, null);

    private static readonly EaseInfo EaseInQuartInfo = new EaseInfo(Quart.EaseIn, Quart.EaseOut);

    private static readonly EaseInfo EaseOutQuartInfo = new EaseInfo(Quart.EaseOut, Quart.EaseIn);

    private static readonly EaseInfo EaseInOutQuartInfo = new EaseInfo(Quart.EaseInOut, null);

    private static readonly EaseInfo EaseInQuintInfo = new EaseInfo(Quint.EaseIn, Quint.EaseOut);

    private static readonly EaseInfo EaseOutQuintInfo = new EaseInfo(Quint.EaseOut, Quint.EaseIn);

    private static readonly EaseInfo EaseInOutQuintInfo = new EaseInfo(Quint.EaseInOut, null);

    private static readonly EaseInfo EaseInExpoInfo = new EaseInfo(Expo.EaseIn, Expo.EaseOut);

    private static readonly EaseInfo EaseOutExpoInfo = new EaseInfo(Expo.EaseOut, Expo.EaseIn);

    private static readonly EaseInfo EaseInOutExpoInfo = new EaseInfo(Expo.EaseInOut, null);

    private static readonly EaseInfo EaseInCircInfo = new EaseInfo(Circ.EaseIn, Circ.EaseOut);

    private static readonly EaseInfo EaseOutCircInfo = new EaseInfo(Circ.EaseOut, Circ.EaseIn);

    private static readonly EaseInfo EaseInOutCircInfo = new EaseInfo(Circ.EaseInOut, null);

    private static readonly EaseInfo EaseInElasticInfo = new EaseInfo(Elastic.EaseIn, Elastic.EaseOut);

    private static readonly EaseInfo EaseOutElasticInfo = new EaseInfo(Elastic.EaseOut, Elastic.EaseIn);

    private static readonly EaseInfo EaseInOutElasticInfo = new EaseInfo(Elastic.EaseInOut, null);

    private static readonly EaseInfo EaseInBackInfo = new EaseInfo(Back.EaseIn, Back.EaseOut);

    private static readonly EaseInfo EaseOutBackInfo = new EaseInfo(Back.EaseOut, Back.EaseIn);

    private static readonly EaseInfo EaseInOutBackInfo = new EaseInfo(Back.EaseInOut, null);

    private static readonly EaseInfo EaseInBounceInfo = new EaseInfo(Bounce.EaseIn, Bounce.EaseOut);

    private static readonly EaseInfo EaseOutBounceInfo = new EaseInfo(Bounce.EaseOut, Bounce.EaseIn);

    private static readonly EaseInfo EaseInOutBounceInfo = new EaseInfo(Bounce.EaseInOut, null);

    private static readonly EaseInfo EaseInStrongInfo = new EaseInfo(Strong.EaseIn, Strong.EaseOut);

    private static readonly EaseInfo EaseOutStrongInfo = new EaseInfo(Strong.EaseOut, Strong.EaseIn);

    private static readonly EaseInfo EaseInOutStrongInfo = new EaseInfo(Strong.EaseInOut, null);
    private static readonly EaseInfo DefaultEaseInfo = new EaseInfo(Linear.EaseNone, null);

    /// <summary>Creates a new instance.</summary>
    /// <param name="pEase">The ease function.</param>
    /// <param name="pInverseEase">Inverse ease function.</param>
    private EaseInfo(TweenDelegate.EaseFunc pEase, TweenDelegate.EaseFunc pInverseEase)
    {
        Ease = pEase;
        InverseEase = pInverseEase;
    }

    /// <summary>
    /// Returns an <see cref="T:Holoville.HOTween.Core.EaseInfo" /> instance based on the given <see cref="T:Holoville.HOTween.EaseType" />.
    /// </summary>
    /// <param name="pEaseType">
    /// An <see cref="T:Holoville.HOTween.EaseType" />.
    /// </param>
    internal static EaseInfo GetEaseInfo(EaseType pEaseType)
    {
        switch (pEaseType)
        {
            case EaseType.EaseInSine:
                return EaseInSineInfo;
            case EaseType.EaseOutSine:
                return EaseOutSineInfo;
            case EaseType.EaseInOutSine:
                return EaseInOutSineInfo;
            case EaseType.EaseInQuad:
                return EaseInQuadInfo;
            case EaseType.EaseOutQuad:
                return EaseOutQuadInfo;
            case EaseType.EaseInOutQuad:
                return EaseInOutQuadInfo;
            case EaseType.EaseInCubic:
                return EaseInCubicInfo;
            case EaseType.EaseOutCubic:
                return EaseOutCubicInfo;
            case EaseType.EaseInOutCubic:
                return EaseInOutCubicInfo;
            case EaseType.EaseInQuart:
                return EaseInQuartInfo;
            case EaseType.EaseOutQuart:
                return EaseOutQuartInfo;
            case EaseType.EaseInOutQuart:
                return EaseInOutQuartInfo;
            case EaseType.EaseInQuint:
                return EaseInQuintInfo;
            case EaseType.EaseOutQuint:
                return EaseOutQuintInfo;
            case EaseType.EaseInOutQuint:
                return EaseInOutQuintInfo;
            case EaseType.EaseInExpo:
                return EaseInExpoInfo;
            case EaseType.EaseOutExpo:
                return EaseOutExpoInfo;
            case EaseType.EaseInOutExpo:
                return EaseInOutExpoInfo;
            case EaseType.EaseInCirc:
                return EaseInCircInfo;
            case EaseType.EaseOutCirc:
                return EaseOutCircInfo;
            case EaseType.EaseInOutCirc:
                return EaseInOutCircInfo;
            case EaseType.EaseInElastic:
                return EaseInElasticInfo;
            case EaseType.EaseOutElastic:
                return EaseOutElasticInfo;
            case EaseType.EaseInOutElastic:
                return EaseInOutElasticInfo;
            case EaseType.EaseInBack:
                return EaseInBackInfo;
            case EaseType.EaseOutBack:
                return EaseOutBackInfo;
            case EaseType.EaseInOutBack:
                return EaseInOutBackInfo;
            case EaseType.EaseInBounce:
                return EaseInBounceInfo;
            case EaseType.EaseOutBounce:
                return EaseOutBounceInfo;
            case EaseType.EaseInOutBounce:
                return EaseInOutBounceInfo;
            case EaseType.EaseInStrong:
                return EaseInStrongInfo;
            case EaseType.EaseOutStrong:
                return EaseOutStrongInfo;
            case EaseType.EaseInOutStrong:
                return EaseInOutStrongInfo;
            default:
                return DefaultEaseInfo;
        }
    }
}

}