using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    private string timeText;
    private DateTime dateTime;
    private bool isReal;
    private string[] parts;
    private bool isDragging = false;
    private bool isRotateMinute;


    [Header("Arrows")]
    public GameObject HourArrow;
    public GameObject MinArrow;
    public GameObject SecArrow;
    [Header("Attributes")]
    public GameObject MoscowText;
    public Text _Text;
    public GameObject FakeLoad;
    public Button EditBtn;
    public TMP_Text BtnTxt;
    public GameObject EditClock;
    public TMP_InputField inputTime;
    public TMP_Text ErrorTxt;
    public GameObject InfoTxt;
    public GameObject RotateMin;


    private void Awake()//���������� ��������� � ���������� ������ ������ �����
    {
        RotateMin.SetActive(true);
        ErrorTxt.text = "������� ����� � ������� HH:MM:SS";
        EditClock.SetActive(false);
        InfoTxt.SetActive(false);
        EditBtn.onClick.AddListener(Edit);
        BtnTxt.text = "�������������";
        FakeLoad.SetActive(true);
        GetYandexTime();
        StartCoroutine(OneSecondLater());
        isReal = true;

    }

    private float lastMouseX; // ��� �������� ���������� ������� ����

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMouseX = Input.mousePosition.x; // ��������� ��������� ������� ����
        }


        if (Input.GetMouseButton(0) && isRotateMinute)
        {
            float deltaX = Input.mousePosition.x - lastMouseX;
            lastMouseX = Input.mousePosition.x;

            float minuteChange = deltaX * Time.deltaTime * 10f; // ��������� � ������� (�������� �� 10 ��� �������� ���������)
            dateTime = dateTime.AddMinutes(minuteChange); // ������ ������� �����

            
            float angle = -dateTime.Minute * 6; // ��������� ���� �������
            MinArrow.transform.rotation = Quaternion.Euler(0, 0, angle);
            HourArrow.transform.rotation = Quaternion.Euler(0, 0, -dateTime.Hour % 12 * 30 + -dateTime.Minute / 2); //������ ��� ������� ���������, ������ ���� ����� ����������� �����
            MinArrow.transform.rotation = Quaternion.Euler(0, 0, -dateTime.Minute * 6);
            SecArrow.transform.rotation = Quaternion.Euler(0, 0, -dateTime.Second * 6);
            timeText = dateTime.ToLongTimeString();
            _Text.text = timeText.ToString();
        }
        else if (Input.GetMouseButtonUp(1))// ��� ������� ��� ����������� ����� �������������
        {
            isRotateMinute = false;
        }
    }
    private void FixedUpdate()
    {
        if (isReal)
        {
            HourArrow.transform.rotation = Quaternion.Euler(0, 0, -dateTime.Hour % 12 * 30 + -dateTime.Minute / 2); // ��������� ������ ��� ������� ���� ��� ������� �������
            MinArrow.transform.rotation = Quaternion.Euler(0, 0, -dateTime.Minute * 6);
            SecArrow.transform.rotation = Quaternion.Euler(0, 0, -dateTime.Second * 6);

            timeText = dateTime.ToLongTimeString();
            _Text.text = timeText.ToString();

        }

    }
    public void RotateHourArrow(bool isHour) // ������ ��� ����� � ����� �������������� 
    {
        isDragging = true;
        inputTime.gameObject.SetActive(false);
        ErrorTxt.gameObject.SetActive(false);
        InfoTxt.SetActive(true);
        if (isHour)
        {
            isRotateMinute = false;
        }
        else
        {
            isRotateMinute = true;
        }
    }
    public void Edit() //������� �� ������ ��������������
    {
        EditBtn.onClick.RemoveAllListeners();
        EditBtn.onClick.AddListener(Confirm);
        isReal = false;
        BtnTxt.text = "���������";
        EditClock.SetActive(true);
        inputTime.gameObject.SetActive(true);
        ErrorTxt.gameObject.SetActive(true);
    }
    public void Confirm()//������� �� ������ ����������
    {
        if (!isDragging)
        {
            string text = inputTime.text; //�������� ����� � ����, ������������� �� ��� HH:MM:SS
            parts = text.Split(':');

            if (parts.Length != 3)
            {
                ErrorTxt.text = "�� �� ����� ����� � ������� HH:MM:SS";
                return;
            }

            int hour = int.Parse(parts[0]);
            int minute = int.Parse(parts[1]);
            int second = int.Parse(parts[2]);
            if (!(hour >= 0 && hour < 24))
            {
                ErrorTxt.text = "�� �� ����� ����� � ������� HH:MM:SS";
                return;
            }
            else if (!(minute >= 0 && minute < 60))
            {
                ErrorTxt.text = "�� �� ����� ����� � ������� HH:MM:SS";
                return;
            }
            else if (!(second >= 0 && second < 60))
            {
                ErrorTxt.text = "�� �� ����� ����� � ������� HH:MM:SS";
                return;
            }

            DateTime newTime = new DateTime(2024, 03, 10, hour, minute, second);
            dateTime = newTime;
        }

        EditBtn.onClick.RemoveAllListeners();
        EditBtn.onClick.AddListener(Edit);
        isReal = true;
        BtnTxt.text = "�������������";
        EditClock.SetActive(false);
        Debug.Log(inputTime.text);
        inputTime.text = "������� �����";
        ErrorTxt.text = "������� ����� � ������� HH:MM:SS";
        InfoTxt.SetActive(false);
        isDragging = false;
        RotateMin.SetActive(true);
    }

    public void ReturnToMainMenu() // ����� � ������� ����, ���� �� ���� ������ � ����
    {
        EditBtn.onClick.RemoveAllListeners();
        EditBtn.onClick.AddListener(Edit);
        isReal = true;
        BtnTxt.text = "�������������";
        EditClock.SetActive(false);
        Debug.Log(inputTime.text);
        inputTime.text = "������� �����";
        ErrorTxt.text = "������� ����� � ������� HH:MM:SS";
        InfoTxt.SetActive(false);
        isDragging = false;
        RotateMin.SetActive(true);
    }

    public void EditInputField() // ��� ���������� ������ ��������, ���� �� ���� ������������� � ��������
    {
        RotateMin.SetActive(false);
    }

    public void GetYandexTime() //��������� ������� � �������
    {
        StartCoroutine(GetTime());
        IEnumerator GetTime()
        {
            UnityWebRequest www = UnityWebRequest.Get("https://yandex.com/time/sync.json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log($"JSON Response: {jsonResponse}");
                long unixTimeMilliseconds = ExtractTimeFromJson(jsonResponse);
                dateTime = UnixTimeMillisecondsToDateTime(unixTimeMilliseconds);
                dateTime = dateTime.AddHours(3);
                Debug.Log($"����� �� Unix timestamp: {dateTime}");
                FakeLoad.SetActive(false);
                MoscowText.SetActive(true);
                yield return new WaitForSeconds(2f);
                MoscowText.SetActive(false);
            }

        }

        StartCoroutine(WaitHourAndCheckTime());




    }

    private long ExtractTimeFromJson(string json) //��������� ������� � ������������� ��������� � �������� 1 ������ 1970 ����
    {
        string timePattern = "\"time\":(\\d+)";
        var match = System.Text.RegularExpressions.Regex.Match(json, timePattern);

        if (match.Success)
        {
            long unixTimeMilliseconds;
            if (long.TryParse(match.Groups[1].Value, out unixTimeMilliseconds))
            {
                return unixTimeMilliseconds;
            }
        }

        Debug.LogError("�� ������� ������� ����� �� JSON.");
        return 0;
    }

    private IEnumerator WaitHourAndCheckTime() // ��������� ��������� ������� � ������� ��� � ���
    {
        yield return new WaitForSecondsRealtime(3600f);
        GetYandexTime();

    }
    private IEnumerator OneSecondLater() // ���������� ����� ������� � ����� �����������
    {
        while (true)
        {

            yield return new WaitForSeconds(1f);
            dateTime = dateTime.AddSeconds(1);
        }

    }

    private DateTime UnixTimeMillisecondsToDateTime(long unixTimeMilliseconds) //�������  ����������� ��������� � �������� 1 ������ 1970 ���� � ���������� ��� �������� ���
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds);
        return dateTimeOffset.UtcDateTime;
    }
}
