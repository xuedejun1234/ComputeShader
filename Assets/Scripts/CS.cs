using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS : MonoBehaviour
{
    
    public ComputeShader computeShader;
    public Material material;
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        //核函数下标
        int kernelIndex = computeShader.FindKernel("CSMain");
        
        RenderTexture mRenderTexture = new RenderTexture(256, 256, 16);
        //深度什么作用
        mRenderTexture.enableRandomWrite = true;
        mRenderTexture.Create();
        //赋值给材质
        material.mainTexture = mRenderTexture;
        computeShader.SetTexture(kernelIndex, "Result", mRenderTexture);
        computeShader.Dispatch(kernelIndex, 256/8, 256/8, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
