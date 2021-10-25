using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raymarcher : MonoBehaviour
{
    [SerializeField] ComputeShader _raymarchingShader;
    RenderTexture _render;

    [SerializeField] float _threshold;

    Dictionary<string, int> _kernels;

    [SerializeField] Light _light;

    [SerializeField, Range(2, 6)] float _height;
    [SerializeField, Range(5, 10)] float _width;
    [SerializeField, Range(5, 10)] float _depth;
    [SerializeField, Range(5, 10)] float _smallDepth;
    [SerializeField, Range(2, 6)] float _windowHeight;
    [SerializeField, Range(2, 6)] float _smallwindowHeight;
    [SerializeField, Range(0, 2)] float _windowWidth;


    

    void Update()
    {
        if (_kernels == null)
        {
            _kernels = new Dictionary<string, int>();

            _kernels["CSMain"] = _raymarchingShader.FindKernel("CSMain");
            if (_render != null)
            {
                _raymarchingShader.SetTexture(_kernels["CSMain"], "_Result", _render);
            }
        }

        if ( _render == null || _render.width != Screen.width || _render.height != Screen.height)
        {
            Debug.Log("reset Texture");
            _render?.Release();
            _render = new RenderTexture(Screen.width, Screen.height, 0);
            _render.enableRandomWrite = true;
            _render.Create();
            _raymarchingShader.SetTexture(_kernels["CSMain"], "_Result", _render);
            _raymarchingShader.SetVector("_resolution", new Vector2(_render.width, _render.height));
        }

        _raymarchingShader.SetVector("_cameraPos", transform.position);
        _raymarchingShader.SetVector("_lightDir", _light.transform.forward);
        
        _raymarchingShader.SetMatrix("_cameraMatrix", transform.localToWorldMatrix);

        _raymarchingShader.SetFloat("_threshold", _threshold);
        _raymarchingShader.SetFloat("_height", _height);
        _raymarchingShader.SetFloat("_width", _width);
        _raymarchingShader.SetFloat("_depth", _depth);
        _raymarchingShader.SetFloat("_smallDepth", _smallDepth);
        _raymarchingShader.SetFloat("_windowHeight", _windowHeight);
        _raymarchingShader.SetFloat("_windowWidth", _windowWidth);
        _raymarchingShader.SetFloat("_smallwindowHeight", _smallwindowHeight);


        _raymarchingShader.Dispatch(_kernels["CSMain"], _render.width / 8, _render.height / 8, 1);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(_render, destination);
    }
}
