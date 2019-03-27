using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSimulation : MonoBehaviour {

	[SerializeField]
    CustomRenderTexture texture;

    void Start()
    {
        texture.Initialize();
    }

    void Update()
    {
		texture.ClearUpdateZones();
		//UpdateZones();
        texture.Update(5);
    }

    void UpdateZones()
    {
        bool leftClick = Input.GetMouseButton(0);
        bool rightClick = Input.GetMouseButton(1);
        if (!leftClick && !rightClick) return;

        RaycastHit hit;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            var defaultZone = new CustomRenderTextureUpdateZone();
            defaultZone.needSwap = true;
            defaultZone.passIndex = 0; // 波動方程式のシミュレーションのパス
            defaultZone.rotation = 0f;
            defaultZone.updateZoneCenter = new Vector2(0.5f, 0.5f);
            defaultZone.updateZoneSize = new Vector2(1f, 1f);

            var clickZone = new CustomRenderTextureUpdateZone();
            clickZone.needSwap = true;
            clickZone.passIndex = leftClick ? 1 : 2; // 1 または -1 にバッファを塗るパス
            clickZone.rotation = 0f;
            clickZone.updateZoneCenter = new Vector2(hit.textureCoord.x, 1f - hit.textureCoord.y);
            clickZone.updateZoneSize = new Vector2(0.01f, 0.01f);

            texture.SetUpdateZones(new CustomRenderTextureUpdateZone[] { defaultZone, clickZone });
        }
    }

    public void OnCollision(List<Ray> rays)
    {
        RaycastHit hit;
        /*var mesh = GetComponent<MeshFilter>().mesh;
        var triangles = mesh.triangles;
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            //1. 交点pが与えられた3点に置いて平面上に存在するか
            var index0 = i + 0;
            var index1 = i + 1;
            var index2 = i + 2;

            var p1 = mesh.vertices[mesh.triangles[index0]];
            var p2 = mesh.vertices[mesh.triangles[index1]];
            var p3 = mesh.vertices[mesh.triangles[index2]];
            var p = transform.InverseTransformPoint(pos);

            var v1 = p2 - p1;
            var v2 = p3 - p1;
            var vp = p - p1;

            var nv = Vector3.Cross(v1, v2);
            var val = Vector3.Dot(nv, vp);

            //誤差
            var suc = -0.000001f < val && val < 0.000001f;

            //同一平面上に存在する交点pが三角形内部に存在するか
            if (!suc)
                continue;
            else
            {
                Debug.Log("1");
                var a = Vector3.Cross(p1 - p3, p - p1).normalized;
                var b = Vector3.Cross(p2 - p1, p - p2).normalized;
                var c = Vector3.Cross(p3 - p2, p - p3).normalized;

                var d_ab = Vector3.Dot(a, b);
                var d_bc = Vector3.Dot(b, c);

                suc = 0.999f < d_ab && 0.999f < d_bc;
            }

            //交点のuv値を求める
            if (!suc)
                continue;
            else
            {
                Debug.Log("2");
                var uv1 = mesh.uv[mesh.triangles[index0]];
                var uv2 = mesh.uv[mesh.triangles[index1]];
                var uv3 = mesh.uv[mesh.triangles[index2]];

                //PerspectiveCollet
                Matrix4x4 mvp = Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix * transform.localToWorldMatrix;
                //各点をProjectionSpaceへの変換
                Vector4 p1_p = mvp * new Vector4(p1.x, p1.y, p1.z, 1);
                Vector4 p2_p = mvp * new Vector4(p2.x, p2.y, p2.z, 1);
                Vector4 p3_p = mvp * new Vector4(p3.x, p3.y, p3.z, 1);
                Vector4 p_p = mvp * new Vector4(p.x, p.y, p.z, 1);
                //w除算で画面上の座標に変換
                Vector2 p1_n = new Vector2(p1_p.x, p1_p.y) / p1_p.w;
                Vector2 p2_n = new Vector2(p2_p.x, p2_p.y) / p2_p.w;
                Vector2 p3_n = new Vector2(p3_p.x, p3_p.y) / p3_p.w;
                Vector2 p_n = new Vector2(p_p.x, p_p.y) / p_p.w;
                //頂点のなす三角形を点pにより3分割し、必要になる面積を計算
                var s = 0.5f * ((p2_n.x - p1_n.x) * (p3_n.y - p1_n.y) - (p2_n.y - p1_n.y) * (p3_n.x - p1_n.x));
                var s1 = 0.5f * ((p3_n.x - p_n.x) * (p1_n.y - p_n.y) - (p3_n.y - p_n.y) * (p1_n.x - p_n.x));
                var s2 = 0.5f * ((p1_n.x - p_n.x) * (p2_n.y - p_n.y) - (p1_n.y - p_n.y) * (p2_n.x - p_n.x));
                //面積比からuvを補間
                var u = s1 / s;
                var v = s2 / s;
                var w = 1 / ((1 - u - v) * 1 / p1_p.w + u * 1 / p2_p.w + v * 1 / p3_p.w);
                var uv = w * ((1 - u - v) * uv1 / p1_p.w + u * uv2 / p2_p.w + v * uv3 / p3_p.w);

                //uvが求まったよ!!!!
                Debug.Log(uv);
                return;
            }
        }*/

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
            updateZones[i].passIndex = 1; // 1 または -1 にバッファを塗るパス
            updateZones[i].rotation = 0f;
            updateZones[i].updateZoneCenter = new Vector2(uvs[i].x, 1f - uvs[i].y);
            updateZones[i].updateZoneSize = new Vector2(0.01f, 0.01f);
        }


        updateZones[hitCnt].needSwap = true;
        updateZones[hitCnt].passIndex = 0; // 波動方程式のシミュレーションのパス
        updateZones[hitCnt].rotation = 0f;
        updateZones[hitCnt].updateZoneCenter = new Vector2(0.5f, 0.5f);
        updateZones[hitCnt].updateZoneSize = new Vector2(1f, 1f);


        texture.SetUpdateZones(updateZones);
    }

    /// <summary>
    /// 点pが辺(v1,v2)上に存在するかどうかを調査する
    /// </summary>
    /// <param name="p">調査点</param>
    /// <param name="v1">辺をなす頂点</param>
    /// <param name="v2">辺をなす頂点</param>
    /// <returns>点pが辺上に存在しているかどうか</returns>
    public static bool ExistPointOnEdge(Vector3 p, Vector3 v1, Vector3 v2)
    {
        float TOLERANCE = 0.0001f;
        return 1 - TOLERANCE < Vector3.Dot(v2 - p, v2 - v1);
    }

    /// <summary>
    /// 点pが与えられた3点がなす三角形の辺上に存在するかを調査する
    /// </summary>
    /// <param name="p">調査点p</param>
    /// <param name="t1">三角形をなす頂点</param>
    /// <param name="t2">三角形をなす頂点</param>
    /// <param name="t3">三角形をなす頂点</param>
    /// <returns>点pが三角形の辺城に存在するかどうか</returns>
    public static bool ExistPointOnTriangleEdge(Vector3 p, Vector3 t1, Vector3 t2, Vector3 t3)
    {
        if (ExistPointOnEdge(p, t1, t2) || ExistPointOnEdge(p, t2, t3) || ExistPointOnEdge(p, t3, t1))
            return true;
        return false;
    }
}

