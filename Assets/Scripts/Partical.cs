using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Partical : MonoBehaviour
{
    public ComputeShader computeShader;
    private ComputeBuffer mParticleDataBuffer;
    private const int mParticleCount = 2000000;
    private int kernelId;
    public Material material;
    public struct ParticleData
    {
        public Vector3 pos;//等价于float3
        public Color color;//等价于float4
    }
    
    // Start is called before the first frame update
    void Start()
    {
        mParticleDataBuffer = new ComputeBuffer(mParticleCount, 28);
        ParticleData[] particleDatas = new ParticleData[mParticleCount];
        mParticleDataBuffer.SetData(particleDatas);
        kernelId = computeShader.FindKernel("UpdateParticle");
        
    }

    // Update is called once per frame
    void Update()
    {
        computeShader.SetBuffer(kernelId, "ParticleBuffer", mParticleDataBuffer);
        computeShader.SetFloat("Time", Time.time);
        computeShader.Dispatch(kernelId,mParticleCount/1000,1,1);
        material.SetBuffer("_particleDataBuffer", mParticleDataBuffer);
    }


    private void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, mParticleCount);
    }

    void OnDestroy()
    {
        mParticleDataBuffer.Release();
        mParticleDataBuffer = null;
    }
}
