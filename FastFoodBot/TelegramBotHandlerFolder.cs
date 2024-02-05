using FastFoodBot.Models;
using FastFoodBot.Servise;
using FastFoodBot.Status;
using Newtonsoft.Json;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FastFoodBot
{
    public class BotHandler
    {
        public string Token { get; set; }
        private bool Contact = true;
        private string Password;
        private bool AdminAccess = false;
        private long num = 0;
        private Message message;
        private Admin admin = new Admin();
        private short AdminChangeProduct = 0;
        private short AdminOrderNum = 0;
        private string oldCategory;
        private short AdminPayType = 0;
        private short AdminChangeStatus = 0;
        private short AdminProductUpdate = 0;
        private string oldProuct;
        private List<PayTypesModel> paytypes;
        private string oldPayType;
        private ProductsModel productsModel = new ProductsModel();
        private string oldOrderStatus;
        private string newOrderStatus;
        public List<AdminStatus> adminModel;
        public HashSet<long> admins = new HashSet<long>() { 5750105361 };
        public UsersModel user = new UsersModel();
        public BinModel binModel = new BinModel();
        public OrderedProducts orderedProducts1 = new OrderedProducts();
        private double Finalsum = 0;
        private short orderedProductsId = 0;
        private string BINPath = "C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\BIN.json";
        private short AdminUserOrderStatus = 0;


        public BotHandler(string token)
        {
            Token = token;
        }
        public async Task BotHandle()
        {
            var botClient = new TelegramBotClient(this.Token);

            using CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cts.Cancel();
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            var chatId = update.Message.Chat.Id;
            message = update.Message;
            adminModel = await CRUD.GetAllInfo<AdminStatus>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Admins.json");
            admins = await FillAdmins(adminModel);

            if (admins.Any(res => res == chatId))
            {
                AdminAccess = true;
                Contact = false;
            }

            if (message.Text == "/start" && !AdminAccess)
            {
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Welcome", cancellationToken: cancellationToken);
                await ShareContact(botClient, cancellationToken, chatId);
            }
            else if (message.Type == MessageType.Contact)
            {
                user.ChatId = chatId;
                user.PhoneNumber = message.Contact.PhoneNumber.ToString();
                Contact = false;
                await LoginAsync(chatId, botClient, cancellationToken, message.Text);
                //PhoneNumber = message.Contact.PhoneNumber.ToString();

                await botClient.SendTextMessageAsync(chatId, text: "All right", replyMarkup: new ReplyKeyboardRemove());


            }
            else if (Contact)
            {
                await ShareContact(botClient, cancellationToken, chatId);
            }
            else if (AdminAccess)
            {
                await AdminPanelAsync(botClient, cancellationToken, chatId, message.Text);
            }
            else if (!AdminAccess)
            {
                await UserPanelAsync(botClient, cancellationToken, chatId, message.Text, user);
            }
            else if (message.Type == MessageType.Text && !Contact)
            {
                Password = message.Text;
                await LoginAsync(chatId, botClient, cancellationToken, Password);
            }
        }

        private async Task ShareContact(ITelegramBotClient botClient, CancellationToken cancellationToken, long chatId)
        {
            var keybord = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton(text:"Share your contact"){RequestContact = true},
                        }

                    })
            {
                ResizeKeyboard = true
            };
            await botClient.SendTextMessageAsync(chatId, text: "For using this bot please send your contact", replyMarkup: keybord);
            // await LoginAsync(chatId, botClient, cancellationToken, message.Text);

        }
        private async Task LoginAsync(long chatId, ITelegramBotClient botClient, CancellationToken cancellationToken, string message)
        {
            var admins = await CRUD.GetAllInfo<AdminStatus>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Admins.json");
            if (admins.Any(res => res.chatId == chatId))
            {
                AdminAccess = true;
                await AdminPanelAsync(botClient, cancellationToken, chatId, message);
                return;
            }
            else
            {
                await UserPanelAsync(botClient, cancellationToken, chatId, message, user);
            }

        }
        private async Task AdminPanelAsync(ITelegramBotClient botClient, CancellationToken cancellationToken, long chatId, string message)
        {
            if (num == 0)
            {
                var keybord = new ReplyKeyboardMarkup(
                new List<KeyboardButton[]>()
                {
                new KeyboardButton[]
                {
                    new KeyboardButton("AddAdmin"),
                    new KeyboardButton("Categories"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Products"),
                    new KeyboardButton("PayType"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("OrderStatus"),
                    new KeyboardButton("ChangeStatusOfOrders"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("DownloadOrders"),
                    new KeyboardButton("DowloadUsers"),
                }
                })
                {
                    ResizeKeyboard = true,
                };
                num = 10;

                await botClient.SendTextMessageAsync(chatId, text: "Select one comand", replyMarkup: keybord);
            }

            if (message == "Categories")
            {
                var keybordCategories = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>()
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Back"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Create new"),
                    new KeyboardButton("Show all"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Update"),
                    new KeyboardButton("Delete")
                }
                })
                {
                    ResizeKeyboard = true,
                };
                await botClient.SendTextMessageAsync(chatId, text: "Select one comand", replyMarkup: keybordCategories);
                num = 1;
            }
            else if (message == "Products")
            {
                var keybordProducts = new ReplyKeyboardMarkup(
                new List<KeyboardButton[]>()
                    {
                        new KeyboardButton[]
                    {
                        new KeyboardButton("Back"),
                    },
                        new KeyboardButton[]
                    {
                        new KeyboardButton("Create new"),
                        new KeyboardButton("Show all"),
                    },
                        new KeyboardButton[]
                    {
                        new KeyboardButton("Update"),
                        new KeyboardButton("Delete")
                    }
})
                {
                    ResizeKeyboard = true,
                };
                await botClient.SendTextMessageAsync(chatId, text: "Select one comand", replyMarkup: keybordProducts);

                num = 2;
            }
            else if (message == "PayType")
            {
                var keybordPayType = new ReplyKeyboardMarkup(
                new List<KeyboardButton[]>()
                    {
                        new KeyboardButton[]
                    {
                        new KeyboardButton("Back"),
                    },
                        new KeyboardButton[]
                    {
                        new KeyboardButton("Create new"),
                        new KeyboardButton("Show all"),
                    },
                        new KeyboardButton[]
                    {
                        new KeyboardButton("Update"),
                        new KeyboardButton("Delete")
                    }
})
                {
                    ResizeKeyboard = true,
                };
                await botClient.SendTextMessageAsync(chatId, text: "Select one comand", replyMarkup: keybordPayType);
                num = 3;
            }
            else if (message == "OrderStatus")
            {
                var keybordOrderStatus = new ReplyKeyboardMarkup(
                new List<KeyboardButton[]>()
                    {
                        new KeyboardButton[]
                    {
                        new KeyboardButton("Back"),
                    },
                        new KeyboardButton[]
                    {
                        new KeyboardButton("Create new"),
                        new KeyboardButton("Show all"),
                    },
                        new KeyboardButton[]
                    {
                        new KeyboardButton("Update"),
                        new KeyboardButton("Delete")
                    }
})
                {
                    ResizeKeyboard = true,
                };
                await botClient.SendTextMessageAsync(chatId, text: "Select one comand", replyMarkup: keybordOrderStatus);
                num = 4;
            }
            else if (message == "ChangeStatusOfOrders")
            {
                var keybordStatusOfOrders = new ReplyKeyboardMarkup(
                new List<KeyboardButton[]>()
                    {
                        new KeyboardButton[]
                    {
                        new KeyboardButton("Back"),
                    },
                        new KeyboardButton[]
                    {
                         new KeyboardButton("Update"),
                        new KeyboardButton("Show all"),
                    },

})
                {
                    ResizeKeyboard = true,
                };
                await botClient.SendTextMessageAsync(chatId, text: "Select one comand", replyMarkup: keybordStatusOfOrders);
                num = 5;
            }
            else if (message == "DownloadOrders")
            {
                FileMaker.WriteToExcel("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Orders.xlsx", "C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Orders.txt");
                await using Stream stream = System.IO.File.OpenRead("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Orders.xlsx");
                await botClient.SendDocumentAsync(
                    chatId: chatId,
                    document: InputFile.FromStream(stream: stream, fileName: "Clients.xlsx"),
                    caption: "Clients list");
                return;

            }
            else if (message == "DowloadUsers")
            {
                var users = await CRUD.GetAllInfo<UsersModel>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Users.json");
                string s = "";
                foreach (var item in users)
                {
                    s += $"{item.ChatId} => {item.PhoneNumber}\n" +
                    "--------------------------------------------\n";
                }
                FileMaker.UsersList(s);
                await using Stream stream = System.IO.File.OpenRead("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Users.pdf");
                await botClient.SendDocumentAsync(
                    chatId: chatId,
                    document: InputFile.FromStream(stream: stream, fileName: "Clients.pdf"),
                    caption: "Clients list");
                return;
            }


            else if (message == "AddAdmin")
            {
                num = 6;
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new admin's chatId");
                return;
            }


            switch (num)
            {
                case 1:
                    if (message == "Create new")
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new category", cancellationToken: cancellationToken);
                        AdminOrderNum = 1;
                        return;
                    }
                    else if (message == "Show all")
                    {
                        List<CategoriesClass> categories = await CRUD.GetAllInfo<CategoriesClass>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Categories.json");
                        for (int i = 0; i < categories.Count; i++)
                        {
                            await botClient.SendTextMessageAsync(chatId: chatId, text: $"{i + 1} => {categories[i].CategoryName}", cancellationToken: cancellationToken);
                        }
                        return;
                    }
                    else if (message == "Update")
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter old category", cancellationToken: cancellationToken);
                        AdminOrderNum = 2;
                        return;
                    }
                    else if (message == "Delete")
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter category", cancellationToken: cancellationToken);
                        AdminOrderNum = 4;
                        return;
                    }
                    else if (message == "Back")
                    {
                        num -= 1;
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Please push any button to make sure", cancellationToken: cancellationToken);
                    }
                    if (AdminOrderNum == 1 && message != "All right" && message != "Create new" && message != "Enter new category" && message != "Show all" && message != "Update" && message != "Delete" && message != "Back")

                    {
                        CategoriesClass categoriesClass = new CategoriesClass();
                        categoriesClass.CategoryName = message;

                        var mes = await admin.AddCategoryAsync(categoriesClass);
                        await botClient.SendTextMessageAsync(chatId: chatId, text: mes, cancellationToken: cancellationToken);
                    }
                    else if (AdminOrderNum == 2)
                    {
                        oldCategory = message;
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new category", cancellationToken: cancellationToken);
                        AdminOrderNum = 3;
                    }
                    else if (AdminOrderNum == 3)
                    {
                        var newCategory = message;
                        var res = await admin.UpdateCategory(oldCategory, newCategory);
                        await botClient.SendTextMessageAsync(chatId: chatId, text: $"{res}", cancellationToken: cancellationToken);
                    }
                    else if (AdminOrderNum == 4)
                    {
                        var res = await admin.DeleteCategory(message);
                        await botClient.SendTextMessageAsync(chatId: chatId, text: $"{res}", cancellationToken: cancellationToken);
                    }


                    break;
                case 2:

                    List<ProductsModel> products = await CRUD.GetAllInfo<ProductsModel>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Products.json");
                    // Products CRUD
                    if (message == "Create new")
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new products name", cancellationToken: cancellationToken);
                        AdminChangeProduct = 1;
                        return;
                    }

                    else if (message == "Show all")
                    {
                        for (int i = 0; i < products.Count; i++)
                        {
                            await botClient.SendTextMessageAsync(chatId: chatId, text: $"{i + 1} => {products[i].ProductName} => {products[i].Category}");
                        }
                    }
                    else if (message == "Update")
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter old products price", cancellationToken: cancellationToken);
                        AdminProductUpdate = 1;
                        return;
                    }
                    else if (message == "Delete")
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter product name", cancellationToken: cancellationToken);
                        AdminChangeProduct = 6;
                        return;
                    }
                    else if (message == "Back")
                    {
                        num = 0;
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Push any button to make sure", cancellationToken: cancellationToken);
                        return;
                    }
                    switch (AdminProductUpdate)
                    {
                        case 1:
                            {
                                oldProuct = message;
                                await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new product name", cancellationToken: cancellationToken);
                                AdminProductUpdate = 2;
                                break;
                            }
                        case 2:
                            productsModel.ProductName = message;
                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new product description", cancellationToken: cancellationToken);
                            AdminProductUpdate = 3;
                            break;
                        case 3:
                            productsModel.ProductDescription = message;
                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter price", cancellationToken: cancellationToken);
                            AdminProductUpdate = 4;
                            break;
                        case 4:
                            if (double.TryParse(message, out double value))
                            {
                                productsModel.Price = value;
                                AdminProductUpdate = 5;
                                await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter any button to make sure", cancellationToken: cancellationToken);
                            }
                            else
                            {
                                AdminChangeProduct = 0;
                                await botClient.SendTextMessageAsync(chatId: chatId, text: "Please enter only numbers", cancellationToken: cancellationToken);
                                await botClient.SendTextMessageAsync(chatId: chatId, text: "Try add all again", cancellationToken: cancellationToken);
                            }

                            break;
                        case 5:
                            var mes = await admin.UpdateProductAsync(oldProuct, productsModel);
                            await botClient.SendTextMessageAsync(chatId: chatId, text: $"{mes}", cancellationToken: cancellationToken);
                            AdminProductUpdate = 0;
                            break;

                    }
                    switch (AdminChangeProduct)
                    {
                        case 1:
                            productsModel.ProductName = message;
                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new products category", cancellationToken: cancellationToken);
                            AdminChangeProduct = 2;
                            break;
                        case 2:
                            List<CategoriesClass> categories = await CRUD.GetAllInfo<CategoriesClass>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Categories.json");
                            if (categories.Any(res => res.CategoryName.ToLower() == message.ToLower()))
                            {

                                productsModel.Category = message;
                                await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new products description", cancellationToken: cancellationToken);
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chatId: chatId, text: "Categorie not found", cancellationToken: cancellationToken);
                                await botClient.SendTextMessageAsync(chatId: chatId, text: "Firstly show all categories", cancellationToken: cancellationToken);
                                AdminChangeProduct = 0;
                            }
                            AdminChangeProduct = 3;
                            break;

                        case 3:
                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new products price", cancellationToken: cancellationToken);
                            productsModel.ProductDescription = message;
                            AdminChangeProduct = 4;
                            break;
                        case 4:
                            if (double.TryParse(message, out double value))
                            {
                                productsModel.Price = (double)value;
                                AdminChangeProduct = 5;
                                await botClient.SendTextMessageAsync(chatId: chatId, text: "Push any button to make sure", cancellationToken: cancellationToken);
                            }
                            else
                            {
                                AdminChangeProduct = 0;
                                await botClient.SendTextMessageAsync(chatId: chatId, text: "Please enter only numbers", cancellationToken: cancellationToken);
                                await botClient.SendTextMessageAsync(chatId: chatId, text: "Try add all again", cancellationToken: cancellationToken);
                            }
                            break;
                        case 5:
                            var mes = await admin.AddProductAsync(productsModel);
                            await botClient.SendTextMessageAsync(chatId: chatId, text: $"{mes}", cancellationToken: cancellationToken);
                            break;
                        case 6:
                            var delMes = await admin.DeleteProductAsync(message);
                            await botClient.SendTextMessageAsync(chatId: chatId, text: $"{delMes}", cancellationToken: cancellationToken);
                            break;
                    }
                    break;
                case 3:
                    if (message == "Create new")
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new category", cancellationToken: cancellationToken);
                        AdminPayType = 1;
                    }
                    else if (message == "Show all")
                    {
                        List<PayTypesModel> paytypes = await CRUD.GetAllInfo<PayTypesModel>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\PayTypes.json");
                        for (int i = 0; i < paytypes.Count; i++)
                        {
                            await botClient.SendTextMessageAsync(chatId: chatId, text: $"{i + 1} => {paytypes[i].PayTypy}", cancellationToken: cancellationToken);
                        }
                        return;
                    }
                    else if (message == "Update")
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter old category", cancellationToken: cancellationToken);
                        AdminPayType = 2;
                        return;
                    }
                    else if (message == "Delete")
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter category", cancellationToken: cancellationToken);
                        AdminPayType = 4;
                        return;
                    }
                    else if (message == "Back")
                    {
                        num = 0;
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Push any button to make sure", cancellationToken: cancellationToken);
                    }
                    else if (AdminPayType == 1 && message != "All right" && message != "Create new" && message != "Enter new category" && message != "Show all" && message != "Update" && message != "Delete" && message != "Back")
                    {
                        PayTypesModel payType = new PayTypesModel();
                        payType.PayTypy = message;
                        var mes = await admin.AddPayType(payType);
                        await botClient.SendTextMessageAsync(chatId: chatId, text: mes, cancellationToken: cancellationToken);
                    }
                    else if (AdminPayType == 2)
                    {
                        oldPayType = message;
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new category", cancellationToken: cancellationToken);
                        AdminPayType = 3;
                        return;
                    }
                    else if (AdminPayType == 3)
                    {
                        var newPayType = message;
                        var res = await admin.UpdateCategory(oldPayType, newPayType);
                        await botClient.SendTextMessageAsync(chatId: chatId, text: $"{res}", cancellationToken: cancellationToken);
                        return;
                    }
                    else if (AdminPayType == 4)
                    {
                        var res = await admin.DeletePayTypeAsync(message);
                        await botClient.SendTextMessageAsync(chatId: chatId, text: $"{res}", cancellationToken: cancellationToken);
                        return;
                    }
                    break;
                case 4:
                    if (message == "Create new")
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new category", cancellationToken: cancellationToken);
                        AdminOrderNum = 1;
                    }
                    else if (message == "Show all")
                    {
                        List<OrdersModel> orders = await CRUD.GetAllInfo<OrdersModel>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\UsersOrders.json");
                        for (int i = 0; i < orders.Count; i++)
                        {
                            await botClient.SendTextMessageAsync(chatId: chatId, text: $"{i + 1} => {orders[i].OrdersName}", cancellationToken: cancellationToken);
                        }
                        return;
                    }
                    else if (message == "Update")
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter old category", cancellationToken: cancellationToken);
                        AdminOrderNum = 2;
                        return;
                    }
                    else if (message == "Delete")
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter category", cancellationToken: cancellationToken);
                        AdminOrderNum = 4;
                        return;
                    }
                    else if (message == "Back")
                    {
                        num = 0;
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Push any button to make sure", cancellationToken: cancellationToken);
                    }
                    else if (AdminOrderNum == 1 && message != "All right" && message != "Create new" && message != "Enter new category" && message != "Show all" && message != "Update" && message != "Delete" && message != "Back")
                    {
                        OrdersModel ordersClass = new OrdersModel();
                        ordersClass.OrdersName = message;
                        var mes = await admin.AddOrder(ordersClass);
                        await botClient.SendTextMessageAsync(chatId: chatId, text: mes, cancellationToken: cancellationToken);
                        return;
                    }
                    else if (AdminOrderNum == 2)
                    {
                        oldPayType = message;
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new category", cancellationToken: cancellationToken);
                        AdminOrderNum = 3;
                        return;
                    }
                    else if (AdminOrderNum == 3)
                    {
                        var newPayType = message;
                        var res = await admin.UpdateOrderAsync(oldPayType, newPayType);
                        await botClient.SendTextMessageAsync(chatId: chatId, text: $"{res}", cancellationToken: cancellationToken);
                    }
                    else if (AdminOrderNum == 4)
                    {
                        var res = await admin.DeleteOrderAsync(message);
                        await botClient.SendTextMessageAsync(chatId: chatId, text: $"{res}", cancellationToken: cancellationToken);
                    }
                    break;
                case 5:
                    // Orders Status CRUD
                    if (message == "Back")
                    {
                        num = 0;
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Push any button to make sure", cancellationToken: cancellationToken);
                    }
                    else if (message == "Show all")
                    {
                        List<OrderedProducts> orders = await CRUD.GetAllInfo<OrderedProducts>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Orders.json");
                        for (int i = 0; i < orders.Count; i++)
                        {
                            await botClient.SendTextMessageAsync(chatId: chatId, text: $"{orders[i].Id} => {orders[i].OrderedStatus} => {orders[i].OrderCost} sums", cancellationToken: cancellationToken);
                        }
                    }
                    else if (message == "Update")
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter Orders Id", cancellationToken: cancellationToken);
                        AdminChangeStatus = 1;
                        return;
                    }
                    else if (AdminChangeStatus == 1)
                    {

                        List<OrderedProducts> orders = await CRUD.GetAllInfo<OrderedProducts>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Orders.json");
                        try
                        {
                            for (int i = 0; i < orders.Count; i++)
                            {
                                if (orders[i].Id == Convert.ToInt16(message))
                                {
                                    oldOrderStatus = orders[i].OrderedStatus;
                                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter new status", cancellationToken: cancellationToken);
                                    AdminChangeStatus = 2;
                                    return;
                                }
                            }
                        }
                        catch
                        {
                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Please enter only numbers", cancellationToken: cancellationToken);
                            num = 0;
                        }
                    }
                    else if (AdminChangeStatus == 2)
                    {
                        List<OrdersModel> orders = await CRUD.GetAllInfo<OrdersModel>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\UsersOrders.json");

                        if (orders.Any(res => res.OrdersName == newOrderStatus))
                        {
                            newOrderStatus = message;
                            var mes = await admin.UpdateOrderStatusAsync(oldOrderStatus, newOrderStatus);
                            await botClient.SendTextMessageAsync(chatId: chatId, text: $"{mes}", cancellationToken: cancellationToken);
                            return;
                        }
                    }

                    break;
                case 6:
                    // Add Admin
                    long messageAsLong;
                    if (long.TryParse(message, out messageAsLong))
                    {
                        if (messageAsLong.ToString().Length == 10)
                        {
                            await admin.AddAdminAsync(messageAsLong, admins);
                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Succesfully added", cancellationToken: cancellationToken);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Please enter correct chatId", cancellationToken: cancellationToken);

                        }
                    }
                    break;
            }

        }
        private async Task UserPanelAsync(ITelegramBotClient botClient, CancellationToken cancellationToken, long chatId, string message, UsersModel user)
        {
            List<CategoriesClass> categoris = await CRUD.GetAllInfo<CategoriesClass>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Categories.json");
            List<UsersModel> users = await CRUD.GetAllInfo<UsersModel>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Users.json");
            if (users.Any(res => res.ChatId == user.ChatId) || user.ChatId == 0) { }
            else
            {
                users.Add(user);
                await CRUD.UpdateDB("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Users.json", await CRUD.Serialize(users));
            }
            if (user.UserStatus == 0 || message.Contains("Ok"))
            {
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Select one category", cancellationToken: cancellationToken);
                for (int i = 0; i < categoris.Count; i++)
                {
                    await botClient.SendTextMessageAsync(chatId: chatId, text: $"{i + 1} => {categoris[i].CategoryName}", cancellationToken: cancellationToken);
                }
                user.UserStatus = 1;
            }
            else if (user.UserStatus == 1)
            {
                var OrderedCategory = message;
                List<ProductsModel> products = await CRUD.GetAllInfo<ProductsModel>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Products.json");
                foreach (var product in products)
                {
                    if (product.Category.ToLower() == OrderedCategory.ToLower())
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: $"{product.ProductName}:\n\t\t{product.ProductDescription}\n\t\t\t\t\t\t\t{product.Price} sum", cancellationToken: cancellationToken);
                    }
                }
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Enter name of product", cancellationToken: cancellationToken);
                user.UserStatus = 2;
            }
            else if (user.UserStatus == 2)
            {
                var OrderedPr = message;

                List<ProductsModel> products = await CRUD.GetAllInfo<ProductsModel>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Products.json");
                foreach (var product in products)
                {
                    if (product.ProductName.ToLower() == OrderedPr.ToLower())
                    {
                        binModel.ProductName = product.ProductName;
                        binModel.Price = product.Price;
                        await DrawButtonsAsync(botClient, cancellationToken, chatId);
                        user.UserStatus = 3;
                        return;
                    }
                }
            }
            else if (user.UserStatus == 3 || message.Contains("Ok"))
            {
                if (message == "BIN")
                {

                    List<BinModel> OrderedProducts = await CRUD.GetAllInfo<BinModel>(BINPath);
                    var res = "";
                    short index = 0;
                    foreach (var product in OrderedProducts)
                    {
                        res += $"{++index}{product.ProductName} :\n\t\t\t\t {product.Price} x {product.Count} = {product.Price * product.Count} sum\n";
                        Finalsum += product.Price * product.Count;
                    }
                    res += $"\t\t\t\t\t\t\t\t\t\t\t\t\t FinalSum {Finalsum} sum";
                    await botClient.SendTextMessageAsync(chatId: chatId, text: res, cancellationToken: cancellationToken);
                    var keybord = new ReplyKeyboardMarkup(
                 new List<KeyboardButton[]>()
                        {
                            new KeyboardButton[]
                        {
                            new KeyboardButton("Purchase"),
                            new KeyboardButton("Cancel"),

                        },

                            new KeyboardButton[]
                        {
                            new KeyboardButton("Back"),
                        },

                        })
                    {
                        ResizeKeyboard = true,
                    };

                    await botClient.SendTextMessageAsync(chatId, text: "Select one comand", replyMarkup: keybord);
                    user.UserStatus = 4;
                    return;
                }
                else if (message == "Back")
                {
                    user.UserStatus = 0;
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Ok,enter something to make sure", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());
                    return;
                }
                else if (int.TryParse(message, out var count))
                {
                    binModel.Count = count;
                    List<BinModel> OrderedProducts = await CRUD.GetAllInfo<BinModel>(BINPath);
                    OrderedProducts.Add(binModel);
                    await CRUD.UpdateDB(BINPath, CRUD.Serialize(OrderedProducts).Result);
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Added to bin", cancellationToken: cancellationToken);
                    return;
                }
            }
            else if (user.UserStatus == 4)
            {
                if (message == "Purchase")
                {
                    List<PayTypesModel> payTypes = await CRUD.GetAllInfo<PayTypesModel>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\PayTypes.json");
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Choose type of purchase", cancellationToken: cancellationToken);
                    user.UserStatus = 5;

                    foreach (var item in payTypes)
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: $"{item.PayTypy}", cancellationToken: cancellationToken);
                    }
                    return;
                }
                else if (message == "Cancel")
                {
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Choose id of order", cancellationToken: cancellationToken);
                    user.UserStatus = 6;
                    return;
                }
                else if (message == "Back")
                {
                    user.UserStatus = 3;
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Send Ok to make sure", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());
                    return;
                }
            }
            else if (user.UserStatus == 5)
            {
                List<PayTypesModel> patypes = await CRUD.GetAllInfo<PayTypesModel>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\PayTypes.json");
                List<OrdersModel> ordersCategories = await CRUD.GetAllInfo<OrdersModel>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Orders.json");
                var PurchagaseType = message;
                if (patypes.Any(res => res.PayTypy.ToLower() == PurchagaseType.ToLower()))
                {

                    orderedProducts1.OrdersPayType = PurchagaseType;
                    orderedProducts1.OrderCost = Finalsum;
                    orderedProducts1.OrderedStatus = "Delivering";
                    orderedProducts1.Id = orderedProductsId++;
                    await WriteOrders(orderedProducts1);
                    List<OrderedProducts> orderedProducts = await CRUD.GetAllInfo<OrderedProducts>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\OrderedProducts.json");
                    orderedProducts.Add(orderedProducts1);
                    await CRUD.UpdateDB("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Orders.json", CRUD.Serialize(orderedProducts).Result);
                    List<BinModel> binmodel = await CRUD.GetAllInfo<BinModel>(BINPath);
                    binmodel.Clear();
                    await CRUD.UpdateDB(BINPath, CRUD.Serialize(binmodel).Result);
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "All right", cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());
                    Finalsum = 0;

                    user.UserStatus = 0;
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Leave your coments", cancellationToken: cancellationToken);
                }
            }
            else if (user.UserStatus == 6)
            {
                List<BinModel> OrderedProducts = await CRUD.GetAllInfo<BinModel>(BINPath);
                if (int.TryParse(message, out int id))
                {
                    try
                    {
                        OrderedProducts.RemoveAt(id - 1);
                        await CRUD.UpdateDB(BINPath, CRUD.Serialize(OrderedProducts).Result);
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Deleted", cancellationToken: cancellationToken);
                        user.UserStatus = 4;
                        return;
                    }
                    catch
                    {

                    }
                }
            }
        }
        private async Task WriteOrders(OrderedProducts product)
        {
            string TextPath = "C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Orders.txt";
            string text = System.IO.File.ReadAllText(TextPath);
            text += product.Id + '|' + product.OrderedStatus + '|' + product.OrderCost + '|' + product.OrdersPayType;
            System.IO.File.WriteAllText(TextPath, text);
        }
        private async Task DrawButtonsAsync(ITelegramBotClient botClient, CancellationToken cancellationToken, long chatId)
        {
            var keybord = new ReplyKeyboardMarkup(
                new List<KeyboardButton[]>()
                {
                new KeyboardButton[]
                {
                    new KeyboardButton("1"),
                    new KeyboardButton("2"),
                    new KeyboardButton("3"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("4"),
                    new KeyboardButton("5"),
                    new KeyboardButton("6"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("7"),
                    new KeyboardButton("8"),
                    new KeyboardButton("9"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("BIN"),
                    new KeyboardButton("Back"),
                }
                })
            {
                ResizeKeyboard = true,
            };

            await botClient.SendTextMessageAsync(chatId, text: "Select one comand", replyMarkup: keybord);
        }
        private async Task<HashSet<long>> FillAdmins(List<AdminStatus> adminsM)
        {
            for (int i = 0; i < adminsM.Count; i++)
            {
                admins.Add(adminsM[i].chatId);
            }
            return admins;
        }
        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}