using System;
using System.Linq;
using Unity.Collections;
//using Unity.Mathematics;
using UnityEngine;
//using Unity.Collections
//using Unity.Burst;

namespace uLipSync
{

//[BurstCompile]
public static class Algorithm
{
    public static float GetMaxValue(in NativeArray<float> array)
    {
        return GetMaxValue(array, array.Length);
    }
    
    //[BurstCompile]
    static float GetMaxValue(NativeArray<float> array, int len)
    {
        float max = 0f;
        for (int i = 0; i < len; ++i)
        {
            max = Mathf.Max(max, Mathf.Abs(array[i]));
        }
        return max;
    }

    public static float GetRMSVolume(in NativeArray<float> array)
    {
        return GetRMSVolume(array, array.Length);
    }

    //[BurstCompile]
    static float GetRMSVolume(NativeArray<float> array, int len)
    {
        float average = 0f;
        for (int i = 0; i < len; ++i)
        {
            average += array[i] * array[i];
        }
        return Mathf.Sqrt(average / len);
    }

    public static void CopyRingBuffer(in NativeArray<float> input, out NativeArray<float> output, int startSrcIndex)
    {
        output = new NativeArray<float>(input.Length, Allocator.Temp);
        CopyRingBuffer(
            input, 
            output, 
            input.Length, 
            startSrcIndex);
    }

    //[BurstCompile]
    static void CopyRingBuffer(in NativeArray<float> input, NativeArray<float> output, int len, int startSrcIndex)
    {
        for (int i = 0; i < len; ++i)
        {
            output[i] = input[(startSrcIndex + i) % len];
        }
    }

    public static void Normalize(ref NativeArray<float> array, float value = 1f)
    {
        Normalize(array, array.Length, value);
    }
        const float EPSILON = 1.1920928955078125e-7f;
    //[BurstCompile]
    static void Normalize(NativeArray<float> array, int len, float value = 1f)
    {
        float max = GetMaxValue(array, len);
        if (max < Mathf.Epsilon) return;
        float r = value / max;
        for (int i = 0; i < len; ++i)
        {
            array[i] *= r;
        }
    }

    public static void LowPassFilter(ref NativeArray<float> data, float sampleRate, float cutoff, float range)
    {
        cutoff = (cutoff - range) / sampleRate;
        range /= sampleRate;

        var tmp = new NativeArray<float>(data, Allocator.Temp);

        int n = (int)MathF.Round(3.1f / range);
        if ((n + 1) % 2 == 0) n += 1;
        var b = new NativeArray<float>(n, Allocator.Temp);

        LowPassFilter(
            data,
            data.Length,
            cutoff,
            tmp,
            b,
            n);

        tmp.Dispose();
        b.Dispose();
    }

    //[BurstCompile]
    static void LowPassFilter(NativeArray<float> data, int len, float cutoff, in NativeArray<float> tmp, NativeArray<float> b, int bLen)
    {
        for (int i = 0; i < bLen; ++i)
        {
            float x = i - (bLen - 1) / 2f;
            float ang = 2f * Mathf.PI * cutoff * x;
            b[i] = 2f * cutoff * Mathf.Sin(ang) / ang;
        }

        for (int i = 0; i < len; ++i)
        {
            for (int j = 0; j < bLen; ++j)
            {
                if (i - j >= 0)
                {
                    data[i] += b[j] * tmp[i - j];
                }
            }
        }
    }

    public static void DownSample(in NativeArray<float> input, out NativeArray<float> output, int sampleRate, int targetSampleRate)
    {
        if (sampleRate <= targetSampleRate)
        {
            output = new NativeArray<float>(input, Allocator.Temp);
        }
        else if (sampleRate % targetSampleRate == 0)
        {
            int skip = sampleRate / targetSampleRate;
            output = new NativeArray<float>(input.Length / skip, Allocator.Temp);
            DownSample1(
                input, 
                output, 
                output.Length,
                skip);
        }
        else
        {
            float df = (float)sampleRate / targetSampleRate;
            int n = (int)Mathf.Round(input.Length / df);
            output = new NativeArray<float>(n, Allocator.Temp);
            DownSample2(
                input, 
                input.Length,
                output, 
                output.Length,
                df);
        }
    }

    //[BurstCompile]
    static void DownSample1(in NativeArray<float>  input, NativeArray<float> output, int outputLen, int skip)
    {
        for (int i = 0; i < outputLen; ++i)
        {
            output[i] = input[i * skip];
        }
    }

    //[BurstCompile]
    static void DownSample2(in NativeArray<float> input, int inputLen, NativeArray<float> output, int outputLen, float df)
    {
        for (int j = 0; j < outputLen; ++j)
        {
            float fIndex = df * j;
            int i0 = (int)Mathf.Floor(fIndex);
            int i1 = Math.Min(i0, inputLen - 1);
            float t = fIndex - i0;
            float x0 = input[i0];
            float x1 = input[i1];
            output[j] = Mathf.Lerp(x0, x1, t);
        }
    }

    public static void PreEmphasis(ref NativeArray<float> data, float p)
    {
        var tmp = new NativeArray<float>(data, Allocator.Temp);
        PreEmphasis(
            data,
            tmp,
            data.Length,
            p);
        tmp.Dispose();
    }

    //[BurstCompile]
    static void PreEmphasis(NativeArray<float> data, in NativeArray<float> tmp, int len, float p)
    {
        for (int i = 1; i < len; ++i)
        {
            data[i] = tmp[i] - p * tmp[i - 1];
        }
    }

    public static void HammingWindow(ref NativeArray<float> array)
    {
        HammingWindow(array, array.Length);
    }

    //[BurstCompile]
    static void HammingWindow(NativeArray<float> array, int len)
    {
        for (int i = 0; i < len; ++i)
        {
            float x = (float)i / (len - 1);
            array[i] *= 0.54f - 0.46f * Mathf.Cos(2f * Mathf.PI * x);
        }
    }

    public static void ZeroPadding(ref NativeArray<float> data, out NativeArray<float> dataWithPadding)
    {
        int N = data.Length;
        dataWithPadding = new NativeArray<float>(N * 2, Allocator.Temp);

        var slice1 = new NativeSlice<float>(dataWithPadding, 0, N / 2);
        //UnsafeUtility.MemSet((float*)slice1.GetUnsafePtr<float>(), 0, sizeof(float) * slice1.Length);
            System.Array.Clear(slice1.ToArray<float>(), 0, sizeof(float) * slice1.Length);

        var slice2 = new NativeSlice<float>(dataWithPadding, N / 2, N);
        slice2.CopyFrom(data);

        var slice3 = new NativeSlice<float>(dataWithPadding, N * 3 / 2, N / 2);
        //UnsafeUtility.MemSet((float*)slice3.GetUnsafePtr<float>(), 0, sizeof(float) * slice1.Length);
            System.Array.Clear(slice3.ToArray<float>(), 0, sizeof(float) * slice1.Length);
        }

    public static void FFT(in NativeArray<float> data, out NativeArray<float> spectrum)
    {
        int N = data.Length;
        spectrum = new NativeArray<float>(N, Allocator.Temp);
        FFT(data, spectrum, N);
    }

    //[BurstCompile]
    static void FFT(NativeArray<float> data, NativeArray<float> spectrum, int N)
    {
        var spectrumRe = new NativeArray<float>(N, Allocator.Temp);
        var spectrumIm = new NativeArray<float>(N, Allocator.Temp);

        for (int i = 0; i < N; ++i)
        {
            spectrumRe[i] = data[i];
        }
        _FFT(spectrumRe, spectrumIm, N);
            Vector2 temp;
        for (int i = 0; i < N; ++i)
        {
            float re = spectrumRe[i];
            float im = spectrumIm[i];
                temp = new Vector2(re, im);
                //spectrum[i] = math.length(new Vector2(re, im));

                spectrum[i] =  Mathf.Sqrt(Vector2.Dot(temp, temp));
                }

        spectrumRe.Dispose();
        spectrumIm.Dispose();
    }

    //[BurstCompile]
    static void _FFT(NativeArray<float> spectrumRe, NativeArray<float> spectrumIm, int N)
    {
        if (N < 2) return;

        var evenRe = new NativeArray<float>(N / 2, Allocator.Temp);
        var evenIm = new NativeArray<float>(N / 2, Allocator.Temp);
        var oddRe = new NativeArray<float>(N / 2, Allocator.Temp);
        var oddIm = new NativeArray<float>(N / 2, Allocator.Temp);

        for (int i = 0; i < N / 2; ++i)
        {
            evenRe[i] = spectrumRe[i * 2];
            evenIm[i] = spectrumIm[i * 2];
            oddRe[i] = spectrumRe[i * 2 + 1];
            oddIm[i] = spectrumIm[i * 2 + 1];
        }

        _FFT(evenRe, evenIm, N / 2);
        _FFT(oddRe, oddIm, N / 2);

        for (int i = 0; i < N / 2; ++i)
        {
            float er = evenRe[i];
            float ei = evenIm[i];
            float or = oddRe[i];
            float oi = oddIm[i];
            float theta = -2f * Mathf.PI * i / N;
            var c = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            c = new Vector2(c.x * or - c.y * oi, c.x * oi + c.y * or);
            spectrumRe[i] = er + c.x;
            spectrumIm[i] = ei + c.y;
            spectrumRe[N / 2 + i] = er - c.x;
            spectrumIm[N / 2 + i] = ei - c.y;
        }

        evenRe.Dispose();
        evenIm.Dispose();
        oddRe.Dispose();
        oddIm.Dispose();
    }

    public static void MelFilterBank(
        in NativeArray<float> spectrum, 
        out NativeArray<float> melSpectrum,
        float sampleRate,
        int melDiv)
    {
        melSpectrum = new NativeArray<float>(melDiv, Allocator.Temp);
        MelFilterBank(
            spectrum,
            melSpectrum,
            spectrum.Length,
            sampleRate,
            melDiv);
    }

    //[BurstCompile]
    static void MelFilterBank(
        in NativeArray<float> spectrum,
        NativeArray<float> melSpectrum,
        int len,
        float sampleRate,
        int melDiv)
    {
        float fMax = sampleRate / 2;
        float melMax = ToMel(fMax);
        int nMax = len / 2;
        float df = fMax / nMax;
        float dMel = melMax / (melDiv + 1);

        for (int n = 0; n < melDiv; ++n)
        {
            float melBegin = dMel * n;
            float melCenter = dMel * (n + 1);
            float melEnd = dMel * (n + 2);

            float fBegin = ToHz(melBegin);
            float fCenter = ToHz(melCenter);
            float fEnd = ToHz(melEnd);

            int iBegin = (int)Math.Ceiling(fBegin / df);
            int iCenter = (int)Mathf.Round(fCenter / df);
            int iEnd = (int)Mathf.Floor(fEnd / df);

            float sum = 0f;
            for (int i = iBegin + 1; i <= iEnd; ++i)
            {
                float f = df * i;
                float a = (i < iCenter) ? 
                    (f - fBegin) / (fCenter - fBegin) : 
                    (fEnd - f) / (fEnd - fCenter);
                a /= (fEnd - fBegin) * 0.5f;
                sum += a * spectrum[i];
            }
            melSpectrum[n] = sum;
        }
    }

    public static void PowerToDb(ref NativeArray<float> array)
    {
        PowerToDb(array, array.Length);
    }

    //[BurstCompile]
    static void PowerToDb(NativeArray<float> array, int len)
    {
        for (int i = 0; i < len; ++i)
        {
            array[i] = 10f * Mathf.Log10(array[i]);
        }
    }

    //[BurstCompile]
    static float ToMel(float hz, bool slaney = false)
    {
        float a = slaney ? 2595f : 1127f;
        return a * Mathf.Log(hz / 700f + 1f);
    }

    //[BurstCompile]
    static float ToHz(float mel, bool slaney = false)
    {
        float a = slaney ? 2595f : 1127f;
        return 700f * (Mathf.Exp(mel / a) - 1f);
    }

    public static void DCT(
        in NativeArray<float> spectrum,
        out NativeArray<float> cepstrum)
    {
        cepstrum = new NativeArray<float>(spectrum.Length, Allocator.Temp);
        DCT(
            spectrum, 
            cepstrum,
            spectrum.Length);
    }

    //[BurstCompile]
    static void DCT(
        in NativeArray<float> spectrum,
        NativeArray<float> cepstrum,
        int len)
    {
        float a = Mathf.PI / len;
        for (int i = 0; i < len; ++i)
        {
            float sum = 0f;
            for (int j = 0; j < len; ++j)
            {
                float ang = (j + 0.5f) * i * a;
                sum += spectrum[j] * Mathf.Cos(ang);
            }
            cepstrum[i] = sum;
        }
    }

    public static float Norm(in NativeArray<float> array)
    {
        return Norm(array.ToArray(), array.Length);
    }

    public static float Norm(in NativeSlice<float> slice)
    {
        return Norm(slice.ToArray(), slice.Length);
    }

    //[BurstCompile]
    static float Norm(in float[]  array, int len)
    {
        float sum = 0f;
        for (int i = 0; i < len; ++i)
        {
            float x = array[i];
            sum += x * x;
        }
        return Mathf.Sqrt(sum);
    }
}

}
