using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramApp
{
    public class Program
    {
        static string service = string.Empty;
        static string token = "6413105564:AAHz72xq10vUUi15utrAD2qKzc0t1Sgpnuw";

        static void Main(string[] args)
        {
            Thread task = new Thread(BackgroundBot);
            ApplicationDataApi applicationDataApi = new ApplicationDataApi();

            task.Start();
        }
        static void BackgroundBot()
        {
            WebClient wc = new WebClient() { Encoding = Encoding.UTF8 };
            var replyMarkup = new ReplyKeyboardMarkup(new[]
                           {
                                new KeyboardButton[] { new KeyboardButton("Список услуг") },
                                new KeyboardButton[] { new KeyboardButton("Оставить заявку") }
                            });
            int update_id = 0;
            string startUrl = $@"https://api.telegram.org/bot{token}/";
            while (true)
            {
                string url = $"{startUrl}getUpdates?offset={update_id}";
                var r = wc.DownloadString(url);

                var msgs = JObject.Parse(r)["result"].ToArray();
                foreach (dynamic msg in msgs)
                {
                    update_id = Convert.ToInt32(msg.update_id) + 1;

                    string userMessage = msg.message.text;
                    string userId = msg.message.from.id;
                    string userFirstName = msg.message.from.first_name;
                    string userLastName = string.Empty;
                    if (msg.message.from.last_name != null)
                    {
                        userLastName = msg.message.from.last_name;
                    }

                    string text = $"{userFirstName} {userId} {userMessage}";

                    Console.WriteLine(text);

                    if (userMessage == "Список услуг")
                    {
                        int counter = 0;
                        string newString = string.Empty;
                        ApplicationDataApi applicationDataApi = new ApplicationDataApi();
                        foreach (var item in applicationDataApi.GetServices())
                        {
                            counter++;
                            newString += $"\n{counter}. {item.Name}";
                        }

                        if (userLastName != string.Empty)
                        {
                            char[] charName = new char[userLastName.Length];
                            charName = userLastName.SplitTextMethod();
                            for (int i = charName.Length - 1; i < charName.Length; i++)
                            {
                                if (charName[i] == 1072)
                                {
                                    string responseText = $"Уважаемая {userFirstName}. Доступны следующие услуги: {newString}";

                                    url = $"{startUrl}sendMessage?chat_id={userId}&text={responseText}";
                                    wc.DownloadString(url);

                                }
                                else
                                {
                                    string responseText = $"Уважаемый {userFirstName}. Доступны следующие услуги: {newString}";
                                    url = $"{startUrl}sendMessage?chat_id={userId}&text={responseText}";
                                    wc.DownloadString(url);
                                }
                            }
                        }
                        else
                        {
                            string responseText = $"Уважаемый(ая) {userFirstName}. Доступны следующие услуги:{newString}";
                            url = $"{startUrl}sendMessage?chat_id={userId}&text={responseText}";
                            wc.DownloadString(url);
                        }
                    }
                    else if (userMessage == "Оставить заявку")
                    {
                        bool isExit = false;
                        string responseText = "Пожалуйста, введите ваш eMail:";
                        string hideKeyboard = JsonConvert.SerializeObject(new
                        {
                            remove_keyboard = true
                        });
                        url = $"{startUrl}sendMessage?chat_id={userId}&text={responseText}&reply_markup={hideKeyboard}";
                        wc.DownloadString(url);
                        while (!isExit)
                        {
                            url = $"{startUrl}getUpdates?offset={update_id}";
                            r = wc.DownloadString(url);
                            
                            msgs = JObject.Parse(r)["result"].ToArray();
                            
                            foreach (dynamic msg2 in msgs)
                            {
                                if (userMessage == "Оставить заявку" && msg2.message.text != null)
                                {
                                    update_id++;
                                    string userEmail = msg2.message.text;
                                    string requestText = "Пожалуйста, введите текст вашей заявки:";
                                    url = $"{startUrl}sendMessage?chat_id={userId}&text={requestText}";
                                    wc.DownloadString(url);
                                    while (!isExit)
                                    {
                                        url = $"{startUrl}getUpdates?offset={update_id}";
                                        r = wc.DownloadString(url);
                                        msgs = JObject.Parse(r)["result"].ToArray();
                                        Thread.Sleep(100);
                                        foreach (dynamic msg3 in msgs)
                                        {
                                            if (userMessage == "Оставить заявку" && msg3.message.text != null)
                                            {
                                                string requestMessage = msg3.message.text;
                                                try
                                                {
                                                    ApplicationDataApi applicationDataApi = new ApplicationDataApi();
                                                    Application application = new Application
                                                    {
                                                        Date = DateTime.Now,
                                                        Message = requestMessage,
                                                        EMail = userEmail,
                                                        Name = $"{userFirstName} {userLastName}",
                                                        Status = "Получена"
                                                    };
                                                    applicationDataApi.AddApplication(application);
                                                    string endText = "Ваша заявка принята! \nОтветим в ближайшее время!";
                                                    url = $"{startUrl}sendMessage?chat_id={userId}&text={endText}&reply_markup={JsonConvert.SerializeObject(replyMarkup)}";
                                                    wc.DownloadString(url);
                                                }
                                                catch (Exception ex)
                                                {
                                                    string endText = $"Возникла непредвиденная ошибка!!! \nОшибка:{ex}";
                                                    url = $"{startUrl}sendMessage?chat_id={userId}&text={endText}&reply_markup={JsonConvert.SerializeObject(replyMarkup)}";
                                                    wc.DownloadString(url);
                                                }
                                                isExit = true;
                                            }
                                        }
                                        Thread.Sleep(100);
                                    }
                                }
                            }
                            Thread.Sleep(100);
                        }
                    }
                    else if (userMessage == "/start")
                    {
                        string responseText = $"Добро пожаловать, {userFirstName}!.\nЯ бот IT-проекта.\nПрошу, выберите одну из команд:";
                        url = $"{startUrl}sendMessage?chat_id={userId}&text={responseText}&reply_markup={JsonConvert.SerializeObject(replyMarkup)}";
                        wc.DownloadString(url);
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}