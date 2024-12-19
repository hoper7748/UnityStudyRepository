using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;


namespace ChatUIProject
{

    public class ChatInputting : MonoBehaviour
    {
        [System.Serializable]
        public struct DotData
        {
            [SerializeField] 
            private GameObject dot;
            public GameObject GetDotObject { get { return dot; } }

            [SerializeField]
            private AnimationCurve curve;
            public AnimationCurve GetCurve { get { return curve; } }
            [SerializeField]
            private float height;
            [SerializeField]
            private float speed;
            public float GetHeight { get { return height; } }

            [SerializeField]
            private Vector3 originPosition;

            [SerializeField]
            private Vector3 targetPosition;

            public DotData SetOriginPosition(Vector3 originPos)
            {
                originPosition = originPos;
                return this;
            }

            public DotData SetTargetPosition(Vector3 targetPos)
            {
                targetPosition = targetPos;

                return this;
            }

            public DotData UpdateMovement(float value)
            {
                dot.transform.position = Vector3.MoveTowards(dot.transform.position, targetPosition, (speed * Mathf.Abs(originPosition.x - targetPosition.x)) * Time.deltaTime);
                dot.transform.position = new Vector3(dot.transform.position.x, originPosition.y + height * value, dot.transform.position.z);
                return this;
            }

            public DotData ResetPosition()
            {
                dot.transform.position = originPosition;

                return this;
            }

        }

        [SerializeField] private DotData[] dotDatas;
        [SerializeField] private float time;

        private void Awake()
        {

        }

        // Start is called before the first frame update
        private void Start()
        {
            for(int i = 0; i <  dotDatas.Length; i++)
            {
                dotDatas[i] = dotDatas[i].SetOriginPosition(dotDatas[i].GetDotObject.transform.position);
            }

            for (int i = 0; i < dotDatas.Length; i++)
            {
                dotDatas[i] = dotDatas[i].SetTargetPosition(dotDatas[i + 1 >= dotDatas.Length ? 0 : i + 1].GetDotObject.GetComponent<RectTransform>().localPosition);
            }

            foreach(var dotData in dotDatas)
            {
                dotData.GetDotObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(BlinkDots());
        }

        private IEnumerator BlinkDots()
        {
            foreach (var dotData in dotDatas)
            {
                yield return new WaitForSeconds(0.33f);
                dotData.GetDotObject.SetActive(true);
            }
        }


        //float currentTime = 0;

        private void Update()
        {
            //if(currenttime < time)
            //{
            //    currenttime += time.deltatime;
            //    for (int i = 0; i < dotdatas.length; i++)
            //    {
            //        playanimation(dotdatas[i], currenttime / time );
            //    }
            //}
            //else
            //{
                
            //}
        }

        private void ResetDotsPosition()
        {
            for(int i =0; i < dotDatas.Length; i++)
            {
                dotDatas[i].ResetPosition();
            }
        }

        private void PlayAnimation(DotData data, float value)
        {
            float yPos = data.GetCurve.Evaluate(value);
            data.UpdateMovement(yPos);
        }
    }
}