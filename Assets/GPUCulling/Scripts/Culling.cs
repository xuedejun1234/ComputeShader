using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Culling : MonoBehaviour
{
    public int instanceCount = 10000;
    public Mesh instanceMesh;
    public Material instanceMaterial;
    public int subMeshIndex = 0;


    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    
    
    public ComputeShader compute;
    private ComputeBuffer localToWorldMatrixBuffer;
    private ComputeBuffer cullResult;
    List<Matrix4x4> localToWorldMatrixs = new List<Matrix4x4>();
    private int kernel;
    private Camera mainCamera;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        
        
        mainCamera = Camera.main;
        subMeshIndex = Mathf.Clamp(subMeshIndex, 0, instanceMesh.subMeshCount - 1);
        kernel = compute.FindKernel("ViewPortCulling");
        
        // 每个矩阵是 16 个 float，也就是 64 字节。
        localToWorldMatrixBuffer =
            new ComputeBuffer(instanceCount, 16 * sizeof(float));
        mainCamera = Camera.main;
        cullResult = new ComputeBuffer(instanceCount, sizeof(float) * 16, ComputeBufferType.Append);
        UpdateBuffer();
        
        args[0] = instanceMesh.GetIndexCount(subMeshIndex);
        args[1] = (uint)instanceCount;
        args[2] = instanceMesh.GetIndexStart(subMeshIndex);
        args[3] = instanceMesh.GetBaseVertex(subMeshIndex);
        args[4] = 0;
        argsBuffer.SetData(args);
        
        instanceMaterial.enableInstancing = true;
        instanceMaterial.SetBuffer("positionBuffer", cullResult);
    }

    // Update is called once per frame
    void Update()
    {
        if (localToWorldMatrixBuffer == null || cullResult == null)
            return;
        
        Vector4[] planes = CullTool.GetFrustumPlane(mainCamera);
        compute.SetBuffer(kernel, "input", localToWorldMatrixBuffer);
        compute.SetBuffer(kernel, "cullresult", cullResult);
        cullResult.SetCounterValue(0);
        compute.SetInt("instanceCount", instanceCount);
        compute.SetVectorArray("planes", planes);
        compute.Dispatch(kernel, 1 +(instanceCount / 640), 1, 1);
        instanceMaterial.SetBuffer("positionBuffer", cullResult);
        
        //获取实际要渲染的数量
        ComputeBuffer.CopyCount(cullResult, argsBuffer, sizeof(uint));

        
        Graphics.DrawMeshInstancedIndirect(instanceMesh, subMeshIndex, instanceMaterial, 
            new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer);
    }

    void UpdateBuffer()
    {
        localToWorldMatrixs.Clear();
        
        for (int i = 0; i < instanceCount; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float radius = Random.Range(20f, 100f);
            float height = Random.Range(-2f, 2f);
            float scale = Random.Range(0.05f, 0.25f);

            Vector3 position = new Vector3(
                Mathf.Sin(angle) * radius,
                height,
                Mathf.Cos(angle) * radius);
            
            
            localToWorldMatrixs.Add(Matrix4x4.TRS(position, Quaternion.identity,
                Vector3.one * scale));
        }
        argsBuffer.SetData(args);
        localToWorldMatrixBuffer.SetData(localToWorldMatrixs);
        
    }


    private void OnDisable()
    {
        localToWorldMatrixBuffer.Release();
        localToWorldMatrixBuffer = null;
        
        cullResult.Release();
        cullResult = null;
        argsBuffer?.Release();
    }
}
