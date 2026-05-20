using System.Collections.Generic;
using UnityEngine;

namespace ProjectEva.Combat
{
    [ExecuteAlways]
    public class BattleArenaSetup : MonoBehaviour
    {
        [SerializeField] private Camera battleCamera;
        [SerializeField] private Vector3 centerPoint;
        [SerializeField] private float playerDistance = 4f, playerHeight = 0f;
        [SerializeField] private float enemyDistance = 4f, enemyLineWidth = 5f, enemyHeight = 0.5f;
        [SerializeField] private float cameraDistance = 7f, cameraHeight = 3f, cameraPitch = 15f, cameraFieldOfView = 50f;
        [SerializeField] private bool createFloor = true;
        [SerializeField] private float floorThickness = 0.2f, floorWidth = 20f, floorDepth = 20f;

        [SerializeField] private Transform playerSpot;
        [SerializeField] private Transform[] enemySpots = new Transform[0];

        private Transform floor;

        private void OnValidate() { if (!Application.isPlaying) UpdateSpots(); }
        private void Awake() => UpdateSpots();

        [ContextMenu("Update Spots Now")]
        public void UpdateSpots()
        {
            if (createFloor) CreateFloor(); else if (floor) DestroyImmediate(floor.gameObject);
            EnsurePlayerSpot();
            EnsureEnemySpots();
            if (battleCamera) SetCamera();
        }

        private void CreateFloor()
        {
            if (!floor)
            {
                Transform t = transform.Find("AutoFloor");
                if (t) floor = t;
                else { var go = GameObject.CreatePrimitive(PrimitiveType.Cube); go.name = "AutoFloor"; go.transform.SetParent(transform); DestroyImmediate(go.GetComponent<MeshRenderer>()); floor = go.transform; }
            }
            floor.localScale = new Vector3(floorWidth, floorThickness, floorDepth);
            floor.localPosition = centerPoint + Vector3.down * (floorThickness / 2f);
        }

        private void EnsurePlayerSpot()
        {
            if (!playerSpot)
            {
                var t = transform.Find("PlayerSpot"); if (t) playerSpot = t;
                else { var go = new GameObject("PlayerSpot"); go.transform.SetParent(transform); playerSpot = go.transform; }
            }
            Vector3 pos = transform.TransformPoint(centerPoint) - transform.forward * playerDistance;
            pos.y = transform.TransformPoint(centerPoint).y + playerHeight;
            playerSpot.position = pos;
            playerSpot.rotation = Quaternion.LookRotation(transform.TransformPoint(centerPoint) - pos, Vector3.up);
        }

        private void EnsureEnemySpots()
        {
            int count = enemySpots.Length;
            for (int i = 0; i < count; i++)
            {
                if (!enemySpots[i])
                {
                    var t = transform.Find($"EnemySpot{i}"); if (t) enemySpots[i] = t;
                    else { var go = new GameObject($"EnemySpot{i}"); go.transform.SetParent(transform); enemySpots[i] = go.transform; }
                }
                float spotT = count == 1 ? 0.5f : i / (float)(count - 1);
                float xOff = Mathf.Lerp(-enemyLineWidth / 2f, enemyLineWidth / 2f, spotT);
                Vector3 ePos = transform.TransformPoint(centerPoint) + transform.forward * enemyDistance + transform.right * xOff;
                ePos.y = transform.TransformPoint(centerPoint).y + enemyHeight;
                enemySpots[i].position = ePos;
                enemySpots[i].rotation = Quaternion.LookRotation(playerSpot.position - ePos, Vector3.up);
            }
        }

        private void SetCamera()
        {
            Vector3 camPos = playerSpot.position - transform.forward * cameraDistance + Vector3.up * cameraHeight;
            battleCamera.transform.position = camPos;
            battleCamera.transform.rotation = Quaternion.Euler(cameraPitch, playerSpot.rotation.eulerAngles.y, 0f);
            battleCamera.fieldOfView = cameraFieldOfView;
        }

        public Transform GetPlayerSpot() => playerSpot;
        public Transform GetEnemySpot(int i) => i >= 0 && i < enemySpots.Length ? enemySpots[i] : null;
        public int GetMaxEnemies() => enemySpots.Length;
    }
}