using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(OilPaintRenderer), PostProcessEvent.AfterStack, "Custom/OilPaint")]
public sealed class OilPaint : PostProcessEffectSettings
{
    [Range(0f, 100f), Tooltip("Radius")]
    public FloatParameter radius = new FloatParameter { value = 5.0f };
}

public sealed class OilPaintRenderer : PostProcessEffectRenderer<OilPaint>
{
    Shader _shader = null;

    public override void Init()
    {
        base.Init();
        _shader = Shader.Find("Hidden/Custom/OilPaint");
    }

    public override void Render(PostProcessRenderContext context)
    {
        PropertySheet sheet = context.propertySheets.Get(_shader);
        sheet.properties.SetFloat("_Radius", settings.radius);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}