#include "UnityCG.cginc"

inline float msdf_median(float3 c)
{
    return max(min(c.r, c.g), min(max(c.r, c.g), c.b));
}

inline float msdf(sampler2D tex, float2 uv)
{
    half3 sample = tex2D(tex, uv);
    return msdf_median(sample) - 0.5;
}

inline float3 msdf_normal(sampler2D tex, float4 texel, float2 uv)
{
    texel *= 2;
    float left = msdf_median(tex2D(tex, float2(uv.x - texel.x, uv.y)));
    float right = msdf_median(tex2D(tex, float2(uv.x + texel.x, uv.y)));
    float bottom = msdf_median(tex2D(tex, float2(uv.x, uv.y - texel.y)));
    float top = msdf_median(tex2D(tex, float2(uv.x, uv.y + texel.y)));
    return float3(left - right, bottom - top, 0);
}

inline float msdf_light(float3 lightDir, float3 normal)
{
    return dot(normalize(lightDir), normal);
}