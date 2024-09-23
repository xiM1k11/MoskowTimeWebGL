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


    private void Awake()//происходят включения и выключения всяких нужных вещей
    {
        RotateMin.SetActive(true);
        ErrorTxt.text = "Введите время в формате HH:MM:SS";
        EditClock.SetActive(false);
        InfoTxt.SetActive(false);
        EditBtn.onClick.AddListener(Edit);
        BtnTxt.text = "Редактировать";
        FakeLoad.SetActive(true);
        GetYandexTime();
        StartCoroutine(OneSecondLater());
        isReal = true;

    }

    private float lastMouseX; // Для хранения предыдущей позиции мыши

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMouseX = Input.mousePosition.x; // Сохраняем начальную позицию мыши
        }


        if (Input.GetMouseButton(0) && isRotateMinute)
        {
            float deltaX = Input.mousePosition.x - lastMouseX;
            lastMouseX = Input.mousePosition.x;

            float minuteChange = deltaX * Time.deltaTime * 10f; // Изменение в минутах (умножаем на 10 для большего изменения)
            dateTime = dateTime.AddMinutes(minuteChange); // Меняем текущее время

            
            float angle = -dateTime.Minute * 6; // Обновляем угол стрелки
            MinArrow.transform.rotation = Quaternion.Euler(0, 0, angle);
            HourArrow.transform.rotation = Quaternion.Euler(0, 0, -dateTime.Hour % 12 * 30 + -dateTime.Minute / 2); //Крутим все стрелки постоянно, теперь даже когда редактируем время
            MinArrow.transform.rotation = Quaternion.Euler(0, 0, -dateTime.Minute * 6);
            SecArrow.transform.rotation = Quaternion.Euler(0, 0, -dateTime.Second * 6);
            timeText = dateTime.ToLongTimeString();
            _Text.text = timeText.ToString();
        }
        else if (Input.GetMouseButtonUp(1))// при нажатии ПКМ отключается режим редакирования
        {
            isRotateMinute = false;
        }
    }
    private void FixedUpdate()
    {
        if (isReal)
        {
            HourArrow.transform.rotation = Quaternion.Euler(0, 0, -dateTime.Hour % 12 * 30 + -dateTime.Minute / 2); // Учитываем минуты при расчете угла для часовой стрелки
            MinArrow.transform.rotation = Quaternion.Euler(0, 0, -dateTime.Minute * 6);
            SecArrow.transform.rotation = Quaternion.Euler(0, 0, -dateTime.Second * 6);

            timeText = dateTime.ToLongTimeString();
            _Text.text = timeText.ToString();

        }

    }
    public void RotateHourArrow(bool isHour) // Кнопка для входа в режим редактирования 
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
    public void Edit() //Нажатие на кнопку редактирования
    {
        EditBtn.onClick.RemoveAllListeners();
        EditBtn.onClick.AddListener(Confirm);
        isReal = false;
        BtnTxt.text = "Применить";
        EditClock.SetActive(true);
        inputTime.gameObject.SetActive(true);
        ErrorTxt.gameObject.SetActive(true);
    }
    public void Confirm()//Нажатие на кнопку применения
    {
        if (!isDragging)
        {
            string text = inputTime.text; //Проверка ввода в поле, соответствует ли оно HH:MM:SS
            parts = text.Split(':');

            if (parts.Length != 3)
            {
                ErrorTxt.text = "Вы не ввели время в формате HH:MM:SS";
                return;
            }

            int hour = int.Parse(parts[0]);
            int minute = int.Parse(parts[1]);
            int second = int.Parse(parts[2]);
            if (!(hour >= 0 && hour < 24))
            {
                ErrorTxt.text = "Вы не ввели время в формате HH:MM:SS";
                return;
            }
            else if (!(minute >= 0 && minute < 60))
            {
                ErrorTxt.text = "Вы не ввели время в формате HH:MM:SS";
                return;
            }
            else if (!(second >= 0 && second < 60))
            {
                ErrorTxt.text = "Вы не ввели время в формате HH:MM:SS";
                return;
            }

            DateTime newTime = new DateTime(2024, 03, 10, hour, minute, second);
            dateTime = newTime;
        }

        EditBtn.onClick.RemoveAllListeners();
        EditBtn.onClick.AddListener(Edit);
        isReal = true;
        BtnTxt.text = "Редактировать";
        EditClock.SetActive(false);
        Debug.Log(inputTime.text);
        inputTime.text = "Введите время";
        ErrorTxt.text = "Введите время в формате HH:MM:SS";
        InfoTxt.SetActive(false);
        isDragging = false;
        RotateMin.SetActive(true);
    }

    public void ReturnToMainMenu() // Выход в главное меню, чтоб не было тупика в игре
    {
        EditBtn.onClick.RemoveAllListeners();
        EditBtn.onClick.AddListener(Edit);
        isReal = true;
        BtnTxt.text = "Редактировать";
        EditClock.SetActive(false);
        Debug.Log(inputTime.text);
        inputTime.text = "Введите время";
        ErrorTxt.text = "Введите время в формате HH:MM:SS";
        InfoTxt.SetActive(false);
        isDragging = false;
        RotateMin.SetActive(true);
    }

    public void EditInputField() // Для отключения кнопки вращения, чтоб не было неприятностей и путаницы
    {
        RotateMin.SetActive(false);
    }

    public void GetYandexTime() //Получение времени с сервера
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
                Debug.Log($"Время по Unix timestamp: {dateTime}");
                FakeLoad.SetActive(false);
                MoscowText.SetActive(true);
                yield return new WaitForSeconds(2f);
                MoscowText.SetActive(false);
            }

        }

        StartCoroutine(WaitHourAndCheckTime());




    }

    private long ExtractTimeFromJson(string json) //Получение времени в миллисекундах прошедших с полуночи 1 января 1970 года
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

        Debug.LogError("Не удалось извлечь время из JSON.");
        return 0;
    }

    private IEnumerator WaitHourAndCheckTime() // Повторное получение времени с сервера раз в час
    {
        yield return new WaitForSecondsRealtime(3600f);
        GetYandexTime();

    }
    private IEnumerator OneSecondLater() // Добавление одной секунды к часам ежесекундно
    {
        while (true)
        {

            yield return new WaitForSeconds(1f);
            dateTime = dateTime.AddSeconds(1);
        }

    }

    private DateTime UnixTimeMillisecondsToDateTime(long unixTimeMilliseconds) //Перевод  миллисекунд прошедших с полуночи 1 января 1970 года в приемлемый для человека вид
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds);
        return dateTimeOffset.UtcDateTime;
    }
}
