using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;

public static class NoiseTools
{
    public static float ComputeNoiseOctave(System.Func<float, float> noiseFunction, int octaves, float time, float frequency, float persistence, float lacunarity)
    {
        float noise = noiseFunction(frequency * time);

        for (int i = 0; i < octaves - 1; i++) // move this to "octave noise" or "rbm noise"
        {
            frequency *= lacunarity;
            noise = persistence * noise + (1f - persistence) * noiseFunction(frequency * time);
        }
        return noise;
    }
}

[System.Serializable]
public class PerlinNoiseGenerator : DampedNoiseGenerator1D
{

    protected override float ComputeNoise(float time)
    {
        return -1 + 2 * Mathf.PerlinNoise(1.345f, time);

    }

    public PerlinNoiseGenerator(float damping, float frequency) : base(damping, frequency)
    {

    }
}

[System.Serializable]
public class HarmonicNoiseGenerator : DampedNoiseGenerator1D
{
    protected override float ComputeNoise(float time)
    {
        return Mathf.Sin(time);
    }
    public HarmonicNoiseGenerator(float damping, float frequency) : base(damping, frequency)
    {

    }

}

public abstract class DampedNoiseGenerator1D
{
    [SerializeField] private float damping;
    [SerializeField] private float magnitude;
    [SerializeField] private float frequency;
    [SerializeField] private float intensity;
    [SerializeField] private int octaves = 1;
    [SerializeField] private float persistence = 0.6f;
    [SerializeField] private float lacunarity = 1.9f;
    private float time;
    private float maxAmplitude = 1f;
    private float cutoffAmplitude = 0.01f;

    protected DampedNoiseGenerator1D(float damping, float frequency)
    {
        this.frequency = frequency;
        this.damping = damping;
    }

    public void AddIntensity(float increment)
    {
        intensity += increment;
        intensity = Mathf.Clamp(intensity, 0, maxAmplitude);
    }
    public void ResetTime() => SetTime(0f);
    
    public void SetTime(float value)
    {
        time = value;
    }

    protected abstract float ComputeNoise(float time);

    public virtual void Update(float dt, out float noise)
    {
        noise = NoiseTools.ComputeNoiseOctave(ComputeNoise, octaves, time, frequency, persistence, lacunarity);

        noise *= intensity * intensity;

        time += dt;

        intensity *= Mathf.Exp(-damping * dt);
        intensity *= intensity < cutoffAmplitude ? 0 : 1;
    }
}
