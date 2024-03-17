#ifndef OrganicFractal_HLSL
#define OrganicFractal_HLSL

void GetOrganicFractal_float(
	float scale, float scaleMulStep, float rotStep, float iterations,
	float2 uvs, float uvAnimSpeed, float rippleStrength, float rippleMaxFreq,
	float rippleSpeed, float brightness, out float Out
) {
	uvs = float2(uvs - 0.5) * 2.0;
	float2 n, q;
	float invertedRadGrad = pow(length(uvs), 2.0);
	float tempOutput = 0.0;
	float2x2 rotMatrix = float2x2(cos(rotStep), sin(rotStep), -sin(rotStep), cos(rotStep));
	float t = _Time.y;
	float uvTime = t * uvAnimSpeed;
	float ripples = sin((t * rippleSpeed) - (invertedRadGrad * rippleMaxFreq)) * rippleStrength;

	for (int i = 0; i < iterations; i++)
	{
		uvs = mul(rotMatrix, uvs);
		n = mul(rotMatrix, n);
		float2 animUV = (uvs * scale) + uvTime;
		q = animUV + ripples + i + n;
		tempOutput += dot(cos(q) / scale, float2(1.0, 1.0) * brightness);
		n -= sin(q);
		scale *= scaleMulStep;
	}
	Out = tempOutput;
}
#endif