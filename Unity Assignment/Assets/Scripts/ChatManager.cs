using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;


namespace ChatUIProject
{
    public class ChatManager : MonoBehaviour
    {

        public GameObject MySender, MyListener;

        public TMP_InputField InputField;
        public RectTransform Background;
        public RectTransform BackgroundSize;

        public RectTransform ContentRect;
        public Scrollbar scrollbar;
        public ScrollRect scrollRect;
        public float scrollSpeed = 50;
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
            if (scrollRect == null)
                scrollRect.scrollSensitivity = scrollSpeed;
            //Background.sizeDelta = new Vector2(Screen.width * 0.35f, Screen.height * 0.85f);
            //Background.sizeDelta = new Vector2(BackgroundSize.rect.width * 0.35f, BackgroundSize.rect.height * 0.85f);
            //Background.position = new Vector2(Background.sizeDelta.x * 0.6f, Background.sizeDelta.y * 0.5f + InputField.GetComponent<RectTransform>().position.y * 2f);

            //// 키보드 타입 설정 (Done 버튼 표시)
            //InputField.keyboardType = TouchScreenKeyboardType.Default;

            //// 줄바꿈 금지 (Single Line 모드로 설정)
            //InputField.lineType = TMP_InputField.LineType.SingleLine;

            //// Enter 키 동작 설정
            //InputField.onSubmit.AddListener(OnSubmit);

        }

        private void Update()
        {
            //InputField.ActivateInputField();
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Chat(true, InputField.text, "me");
                InputField.text = "";
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
            //Area.AreaRect.sizeDelta = new Vector2(Screen.width - 100, Area.AreaRect.sizeDelta.y);
            Area.AreaRect.sizeDelta = new Vector2(Background.rect.width, Area.AreaRect.sizeDelta.y);
            //Area.AreaRect.sizeDelta = new Vector2(Background.rect.width, Background.rect.height * 0.25f);
            Area.transform.SetParent(ContentRect, false);
            Area.BoxRect.sizeDelta = new Vector2(Background.rect.width * 0.7f, Background.rect.height * 0.25f);
            Area.TextRect.GetComponent<TextMeshProUGUI>().text = text;
            Fit(Area.BoxRect);

            // 두 줄 이상이면 크기를 줄여기면서, 한 줄이 아래로 내려가면 바로 전 크기를 대입.
            float x = Area.TextRect.sizeDelta.x + 24;
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
            //Area.AreaRect.sizeDelta = new Vector2(Screen.width - 100, Area.AreaRect.sizeDelta.y);
            Area.AreaRect.sizeDelta = new Vector2(Background.rect.width, Area.AreaRect.sizeDelta.y);
            Area.transform.SetParent(ContentRect, false);
            Area.BoxRect.sizeDelta = new Vector2(Background.rect.width * 0.7f, Background.rect.height * 0.25f);
            Area.TextRect.GetComponent<TextMeshProUGUI>().text = Area.GetRandomAnswer;

            yield return AnswerWaitTime;
            Area.InputtingRect.gameObject.SetActive(false);
            //yield return ShowInputWaitTime;
            Area.BoxRect.gameObject.SetActive(true);
            Area.TextRect.gameObject.SetActive(true);
            Fit(Area.BoxRect);

            // 두 줄 이상이면 크기를 줄여기면서, 한 줄이 아래로 내려가면 바로 전 크기를 대입.
            float x = Area.TextRect.sizeDelta.x + 24;
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