using UnityEngine;
using Unity.Cinemachine;

public class CinemachineTargetFix : MonoBehaviour
{
    private CinemachineCamera vcam;

    void Start()
    {
        // Lấy component Cinemachine trên đối tượng CamGoc
        vcam = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        // Kiểm tra nếu chưa gán mục tiêu Follow
        if (vcam != null && vcam.Follow == null)
        {
            // Tự động tìm nhân vật bằng Tag
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                // Gán vào cả 2 ô Follow và Look At của Cinemachine
                vcam.Follow = player.transform;
                vcam.LookAt = player.transform;

                Debug.Log("🎯 Cinemachine đã tìm thấy Player và tự động khóa mục tiêu!");
            }
        }
    }
}