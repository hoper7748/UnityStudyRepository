using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace ChatUIProject
{
    public class AreaScript : MonoBehaviour
    {
        [System.Serializable]
        public struct RandomAnswer
        {
            public string Answers;
        }
        
        public RandomAnswer[] Answers;

        public string GetRandomAnswer { get { return Answers[Random.Range(0, Answers.Length)].Answers; } }

        public RectTransform AreaRect, BoxRect, TextRect, InputtingRect;
        public GameObject Tial;
        public TextMeshProUGUI TimeText, DateTest;  
        public string Time, Sender;

        

        private void OnEnable()
        {
            if (InputtingRect != null)
            {
                //AreaRect.gameObject.SetActive(false);
                BoxRect.gameObject.SetActive(false);
                TextRect.gameObject.SetActive(false);
                InputtingRect.gameObject.SetActive(true);
            }
            else
            {
                //AreaRect.gameObject.SetActive(true);
                BoxRect.gameObject.SetActive(true);
                TextRect.gameObject.SetActive(true);
            }
        }

        //private IEnumerator WaitAnimation()
        //{

        //    yield return new WaitForSeconds(1f);
        //    InputtingRect.gameObject.SetActive(false);
        //    yield return new WaitForSeconds(0.25f);
        //    BoxRect.gameObject.SetActive(true);
        //    TextRect.gameObject.SetActive(true);
        //}
    }
}