using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;


namespace ChatUIProject
{
    public class ChatManager : MonoBehaviour
    {

        public GameObject MySender, MyListener;

        public TextMeshProUGUI InputField;

        public RectTransform ContentRect;
        public Scrollbar scrollbar;
        public float waitTime = 0;

        private AreaScript LastArea;

        private WaitForSeconds AnswerWaitTime;
        private WaitForSeconds ShowInputWaitTime;
        private WaitForSeconds InputWaitTime;
        private void Start()
        {
            AnswerWaitTime = new WaitForSeconds(waitTime);
            ShowInputWaitTime = new WaitForSeconds(0.15f);
            InputWaitTime = new WaitForSeconds(0.5f);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Chat(true, InputField.text, "me");
                InputField.text = string.Empty;
            }
        }

        public void Chat(bool isSend, string text, string user)
        {
            if (text.Trim() == "") return;

            ShowChat(isSend, text, user);
            StartCoroutine(GetAnswer(false, user));

        }

        public void SendMessage()
        {
            Chat(true, InputField.text, "me");
        }

        public void ShowChat(bool isSend, string text, string user)
        {

            bool isBottom = scrollbar.value <= 0.0001f;

            AreaScript Area = Instantiate(isSend ? MySender : MyListener).GetComponent<AreaScript>();
            Area.AreaRect.sizeDelta = new Vector2(Screen.width - 100, Area.AreaRect.sizeDelta.y);
            Area.transform.SetParent(ContentRect, false);
            Area.BoxRect.sizeDelta = new Vector2(600, Area.BoxRect.sizeDelta.y);
            Area.TextRect.GetComponent<TextMeshProUGUI>().text = text;
            Fit(Area.BoxRect);

            // 두 줄 이상이면 크기를 줄여기면서, 한 줄이 아래로 내려가면 바로 전 크기를 대입.
            float x = Area.TextRect.sizeDelta.x + 42;
            float y = Area.TextRect.sizeDelta.y;

            if (y > 49)
            {
                for (int i = 0; i < 200; i++)
                {
                    Area.BoxRect.sizeDelta = new Vector2(x - i * 2, Area.BoxRect.sizeDelta.y);
                    Fit(Area.BoxRect);

                    if (y != Area.TextRect.sizeDelta.y)
                    {
                        Area.BoxRect.sizeDelta = new Vector2(x - (i * 2) + 2, y);
                        break;
                    }
                }
            }
            else
            {
                Area.BoxRect.sizeDelta = new Vector2(x, y);
            }

            SetDateTimeText(ref Area, ref user);

            // 이전과 같은 시간 없애기
            bool isSame = LastArea != null && LastArea.Time == Area.Time && LastArea.Sender == Area.Sender;
            if (isSame) LastArea.TimeText.text = "";

            Fit(Area.BoxRect);
            Fit(Area.AreaRect);
            Fit(ContentRect);
            LastArea = Area;
        }

        private IEnumerator GetAnswer(bool isSend, string user)
        {
            yield return InputWaitTime;

            AreaScript Area = Instantiate(isSend ? MySender : MyListener).GetComponent<AreaScript>();
            Area.AreaRect.sizeDelta = new Vector2(Screen.width - 100, Area.AreaRect.sizeDelta.y);
            Area.transform.SetParent(ContentRect, false);
            Area.BoxRect.sizeDelta = new Vector2(600, Area.BoxRect.sizeDelta.y);
            Area.TextRect.GetComponent<TextMeshProUGUI>().text = Area.GetRandomAnswer;

            yield return AnswerWaitTime;
            Area.InputtingRect.gameObject.SetActive(false);
            yield return ShowInputWaitTime;
            Area.BoxRect.gameObject.SetActive(true);
            Area.TextRect.gameObject.SetActive(true);
            Fit(Area.BoxRect);

            // 두 줄 이상이면 크기를 줄여기면서, 한 줄이 아래로 내려가면 바로 전 크기를 대입.
            float x = Area.TextRect.sizeDelta.x + 42;
            float y = Area.TextRect.sizeDelta.y;

            if (y > 49)
            {
                for (int i = 0; i < 200; i++)
                {
                    Area.BoxRect.sizeDelta = new Vector2(x - i * 2, Area.BoxRect.sizeDelta.y);
                    Fit(Area.BoxRect);

                    if (y != Area.TextRect.sizeDelta.y)
                    {
                        Area.BoxRect.sizeDelta = new Vector2(x - (i * 2) + 2, y);
                        break;
                    }
                }
            }
            else
            {
                Area.BoxRect.sizeDelta = new Vector2(x, y);
            }

            SetDateTimeText(ref Area, ref user);

            // 이전과 같은 시간 없애기
            bool isSame = LastArea != null && LastArea.Time == Area.Time && LastArea.Sender == Area.Sender;
            if (isSame) LastArea.TimeText.text = "";

            Fit(Area.BoxRect);
            Fit(Area.AreaRect);
            Fit(ContentRect);
            LastArea = Area;
        }


        private void SetDateTimeText(ref AreaScript Area, ref string sender)
        {
            //// 현재 것에 분까지 나오는 날짜 대입.
            //DateTime t = DateTime.Now;
            //Area.Time = t.ToString("yyyy-MM-dd-HH-mm");
            //Area.Sender = sender;

            //// 현재 것은 항상 새로운 시간 대입.
            //int hour = t.Hour;
            //if (t.Hour == 0) hour = 12;
            //else if (t.Hour > 12) hour -= 12;
            //Area.TimeText.text = (t.Hour > 12 ? "PM " : "AM ") + hour + ":" + t.Minute.ToString("D2");

        }

        void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);

    }
}