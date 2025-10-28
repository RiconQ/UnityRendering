using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeManager : MonoBehaviour
{
    public ComputeShader m_compute;

    private ComputeBuffer _resultBuffer;
    private int _kernelIndex;

    private void Start()
    {
        _kernelIndex = m_compute.FindKernel("CSMain");

        _resultBuffer = new(64, sizeof(float));

        m_compute.SetBuffer(_kernelIndex, "_ResultBuffer", _resultBuffer);
    }

    private void RunShader()
    {
        m_compute.Dispatch(_kernelIndex, 1, 1, 1);

        float[] cpuDataArray = new float[64];
        _resultBuffer.GetData(cpuDataArray);

        Debug.Log("GPU 계산 결과 (5번째 값): " + cpuDataArray[5]);

        _resultBuffer.Release();
    }

    void OnEnable()
    {
        Start(); // 초기화
        RunShader(); // 실행
    }

    void OnDisable()
    {
        // 게임 꺼질 때 버퍼 확실히 정리
        if (_resultBuffer != null)
            _resultBuffer.Release();
    }
}
