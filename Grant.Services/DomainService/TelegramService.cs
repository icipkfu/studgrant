
namespace Grant.Services.DomainService
{
    using Core;
    using Core.Entities;
    using DataAccess;
    using Grant.Core.Config;
    using Grant.Core.Context;
    using Grant.Core.Services;
    using LightInject;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using xNet;


    public class TelegramService : BaseDomainService<TelegramUser>, ITelegramService
    {
        private const string telegramApi = "https://api.telegram.org";
        public string token { get; private set; }
        public string address { get; private set; }
        public string login { get; private set; }
        public string pass { get; private set; }
        public string ip { get; private set; }
        public string port { get; private set; }

        private const string GetUpdates = "getUpdates";
        private const string getUserProfilePhotos = "getUserProfilePhotos";
        private const string getFile = "getFile";

        // <TelegramUser.TelegramId,  TelegramUser>
        private Dictionary<string, TelegramUser> userDict = new Dictionary<string, TelegramUser>();

        private IRepository<Settings> settingsRepo;
        private ITelegramCommand telegramCommand;

        protected IServiceContainer Container
        {
            get
            {
                return ApplicationContext.Current.Container;
            }

        }

        public TelegramService(IRepository<TelegramUser> repository, IRepository<Settings> settingsRepo, IConfigProvider configProvider) : base(repository)
        {
            this.settingsRepo = settingsRepo;

            var config = configProvider.GetModuleConfig("TelegramToken");
            var configTelegram = configProvider.GetModuleConfig("TelegramProxy");
            token = config.GetAs<string>("token");
            ip = configTelegram.GetAs<string>("IP");
            port = configTelegram.GetAs<string>("Port");
            login = configTelegram.GetAs<string>("Login");
            pass = configTelegram.GetAs<string>("Password");
        }

        public async Task Execute(dynamic update)
        {
            var message = update != null ? update["message"] : null;

            string id = message["id"];

            var from = update != null ? message["from"] : null;

            var sender = new TelegramUser
            {
                FirstName = from.first_name,
                Username = from.username,
                TelegramId = from.id
                // CanSendCommand == false by default
            };

            if (userDict.ContainsKey(sender.TelegramId))
            {
                sender = userDict[sender.TelegramId];
            }
            else
            {
                await Create(sender);
            }

            string text = message["text"];

            string response = "К сожалению, такая команда не найдена.";

            if (!string.IsNullOrEmpty(text))
            {
                var cmd = text.Split(' ')[0];

                // проверка на существование команды cmd
                try
                {
                    telegramCommand = Container.GetInstance<ITelegramCommand>(cmd);
                }
                catch (Exception ex)
                {

                }

                if (telegramCommand != null)
                {
                    if (sender.CanSendCommmand || cmd == "getaccess")
                    {
                        response = await telegramCommand.ExecuteCommandAsync(message);
                    }
                    else
                    {
                        response = "К сожалению, Вы не имеете достаточно прав для использования данного бота";
                    }
                }

            }

            send(response, sender.TelegramId);
        }

        public async Task CheckTelegramMsgTask()
        {

            if (!Cache.isCheckingNewMessages)
            {
                try
                {
                    Cache.isCheckingNewMessages = true;

                    userDict = await GetAll()
                      .Include(x => x.Picture)
                      .ToDictionaryAsync(x => x.TelegramId); // получаем всех TelegramUser-ов


                    var set = await settingsRepo.GetAll().Where(x => x.Key == "telegramUpdateId").FirstOrDefaultAsync();

                    // если ключ-значенеие в базе еще не создано
                    if (set == null)
                    {
                        set = new Settings()
                        {
                            Key = "telegramUpdateId",
                            Value = "0"
                        };
                        await settingsRepo.Create(set);
                    }
                    // получаем id последнего обработанного сообщения, чтобы знать на какие необработанные следует ответить
                    var updateId = set?.Value;

                    var response = await ExecuteGetRequest($"{GetUpdates}?offset={updateId}");

                    dynamic data = null;
                    try
                    {
                        data = JsonConvert.DeserializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        sendSystemNotification($"{ex.Message}{ex.StackTrace}{ex.InnerException?.Message}");
                    }

                    if (data != null)
                    {
                        var updateArr = data != null ? data["result"] : null;

                        foreach (dynamic update in updateArr)
                        {
                            string newUpdateId = update["update_id"];

                            if (newUpdateId != updateId)
                            {
                                await Execute(update);

                                set.Value = newUpdateId;
                                await settingsRepo.Update(set);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    sendSystemNotification($"{ex.Message}{ex.StackTrace}{ex.InnerException?.Message}");
                    throw;
                }
                finally
                {
                    Cache.isCheckingNewMessages = false;
                }
            }
        }

        protected async Task<string> ExecuteGetRequest(string methodAndParams)
        {
            try
            {
                var url = $"{telegramApi}/{token}/{methodAndParams}";

                HttpRequest req = new HttpRequest();

                if (ip.Length != 0 && port.Length != 0)
                {
                    req.Proxy = ProxyClient.Parse(ProxyType.Socks5, ip + ":" + port);
                }

                return await Task.Run(() =>
                {
                    return req.Get(url).ToString();
                });
            }
            catch (Exception ex)
            {
                sendSystemNotification(ex.Message + ex.StackTrace);
                return "";
            }

        }

        private void send(string text, string id)
        {
            try
            {
                var url =
                   $"{telegramApi}/{token}/sendMessage?chat_id={id}&text={text}";

                HttpRequest req = new HttpRequest();
                if (ip.Length != 0 && port.Length != 0)
                {
                    req.Proxy = ProxyClient.Parse(ProxyType.Socks5, ip + ":" + port);
                }

                var tg_result = req.Get(url).ToString();
            }
            catch (Exception ex)
            {
                sendSystemNotification(ex.Message + ex.StackTrace);
            }
        }

        public void sendSystemNotification(string text)
        {
            //var id = "telegramId";
            //try
            //{
            //    var url =
            //       $"{telegramApi}/{token}/sendMessage?chat_id={id}&text={text}";

            //    HttpRequest req = new HttpRequest();
            //    if (ip.Length != 0 && port.Length != 0)
            //    {
            //        req.Proxy = ProxyClient.Parse(ProxyType.Socks5, ip + ":" + port);
            //    }

            //    var tg_result = req.Get(url).ToString();
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
        }

    }
}
