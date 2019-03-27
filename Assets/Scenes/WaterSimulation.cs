using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSimulation : MonoBehaviour {

	[SerializeField]
    CustomRenderTexture _texture;

    void Start()
    {
        _texture.Initialize();
    }

    void Update()
    {
		_texture.ClearUpdateZones();
        _texture.Update(5);
    }

    public void OnCollision(List<Ray> rays)
    {
        RaycastHit hit;

        List<Vector2> uvs = new List<Vector2>();
       
        foreach (var ray in rays)
        if (Physics.Raycast(ray,out hit))
        {
            uvs.Add(hit.textureCoord);
        }

        int hitCnt = uvs.Count;
        CustomRenderTextureUpdateZone[] updateZones = new CustomRenderTextureUpdateZone[hitCnt + 1];
        for (int i = 0; i < hitCnt; i++)
        {
            updateZones[i].needSwap = true;
            updateZones[i].passIndex = 1;
            updateZones[i].rotation = 0f;
            updateZones[i].updateZoneCenter = new Vector2(uvs[i].x, 1f - uvs[i].y);
            updateZones[i].updateZoneSize = new Vector2(0.01f, 0.01f);
            Debug.Log("bbbb"); 
        }


        updateZones[hitCnt].needSwap = true;
        updateZones[hitCnt].passIndex = 0; // 波動方程式のシミュレーションのパス
        updateZones[hitCnt].rotation = 0f;
        updateZones[hitCnt].updateZoneCenter = new Vector2(0.5f, 0.5f);
        updateZones[hitCnt].updateZoneSize = new Vector2(1f, 1f);


        _texture.SetUpdateZones(updateZones);
    }
}

