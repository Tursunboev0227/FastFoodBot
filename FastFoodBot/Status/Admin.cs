using FastFoodBot.Models;
using FastFoodBot.Servise;
using Newtonsoft.Json;
namespace FastFoodBot.Status
{
    public class Admin
    {
        public string CategoryPath = "C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Categories.json";
        private string AdminsPath = "C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Admins.json";
        private string PayTypePath = "C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\PayTypes.json";
        private string OrdersTypePath = "C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\UsersOrders.json";
        private string ProductsPath = "C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Products.json";
        private string UsersOrdersPath = "C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\UsersOrders.json";
        private List<CategoriesClass> categories;
        private List<PayTypesModel> payTypes;
        private List<OrdersModel> orders;
        private List<ProductsModel> products;
        private List<OrderedProducts> usersOrders;
        
        public async Task AddAdminAsync(long id, HashSet<long> admins)
        {
            try
            {
                admins.Add(id);
                await File.WriteAllTextAsync(AdminsPath, JsonConvert.SerializeObject(admins));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding admin: {ex.Message}");
            }
        }

        public async Task<string> AddCategoryAsync(CategoriesClass category)
        {
            categories = await CRUD.GetAllInfo<CategoriesClass>(CategoryPath);
            try
            {


                if (categories.Any(res => res.CategoryName.ToLower() == category.CategoryName.ToLower()))
                {
                    return "Already exist";
                }

                categories.Add(category);
                await CRUD.UpdateDB(CategoryPath, CRUD.Serialize(categories).Result);

                return "Category added successfully";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return $"Error adding category: ";
            }
        }

        public async Task<string> UpdateCategory(string category_name,string new_categoryname)
        {
            categories = await CRUD.GetAllInfo<CategoriesClass>(CategoryPath);
            for(int i = 0; i < categories.Count; i++)
            {
                if (categories[i].CategoryName.ToLower() == category_name.ToLower())
                {
                    categories[i].CategoryName = new_categoryname;
                    CRUD.UpdateDB(CategoryPath, CRUD.Serialize(categories).Result);
                    return "Seccesfully updated";
                }
            }
            return "Not found";

        }

        public async Task<string> DeleteCategory(string category_name)
        {
            categories = await CRUD.GetAllInfo<CategoriesClass>(CategoryPath);
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].CategoryName == category_name)
                {
                   categories.RemoveAt(i);
                   await CRUD.UpdateDB(CategoryPath, CRUD.Serialize(categories).Result);
                    return "Seccesfully deleted";
                }
            }
            return "Not found";
        }

        public async Task<string> AddPayType(PayTypesModel payType)
        {
            payTypes = await CRUD.GetAllInfo<PayTypesModel>(PayTypePath);
            try
            {


                if (payTypes.Any(res => res.PayTypy.ToLower() == payType.PayTypy.ToLower()))
                {
                    return "Already exist";
                }

                payTypes.Add(payType);
                await CRUD.UpdateDB(PayTypePath, CRUD.Serialize(payTypes).Result);

                return "Category added successfully";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return $"Error adding category: ";
            }
        }
        public async Task<string> UpdatePayTypeAsync(string old_pay_type, string new_pay_type)
        {
            payTypes = await CRUD.GetAllInfo<PayTypesModel>(PayTypePath);
            for (int i = 0; i < payTypes.Count; i++)
            {
                if (payTypes[i].PayTypy.ToLower() == old_pay_type.ToLower())
                {
                    payTypes[i].PayTypy = new_pay_type;
                    CRUD.UpdateDB(PayTypePath, CRUD.Serialize(payTypes).Result);
                    return "Seccesfully updated";
                }
            }
            return "Not found";
        }
        public async Task<string> DeletePayTypeAsync(string payType)
        {
             payTypes = await CRUD.GetAllInfo<PayTypesModel>(PayTypePath);

            for (int i = 0; i < payTypes.Count; i++)
            {
                if (payTypes[i].PayTypy == payType)
                {
                    payTypes.RemoveAt(i);
                   CRUD.UpdateDB(PayTypePath, CRUD.Serialize(payTypes).Result);
                    return "Seccesfully deleted";
                }
            }
            return "Not found";
        }
        public async Task<string> AddOrder(OrdersModel order)
        {
            orders = await CRUD.GetAllInfo<OrdersModel>(OrdersTypePath);
            try
            {


                if (orders.Any(res => res.OrdersName.ToLower() == order.OrdersName.ToLower()))
                {
                    return "Already exist";
                }

                orders.Add(order);
                await CRUD.UpdateDB(OrdersTypePath, CRUD.Serialize(orders).Result);

                return "Category added successfully";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return $"Error adding category: ";
            }
        }
        public async Task<string> UpdateOrderAsync(string old_order_type, string new_order_type)
        
        {
            orders = await CRUD.GetAllInfo<OrdersModel>(PayTypePath);
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].OrdersName.ToLower() == old_order_type.ToLower())
                {
                    payTypes[i].PayTypy = new_order_type;
                    CRUD.UpdateDB(OrdersTypePath, CRUD.Serialize(orders).Result);
                    return "Seccesfully updated";
                }
            }
            return "Not found";
        }
        public async Task<string>DeleteOrderAsync(string order)
        {
            orders = await CRUD.GetAllInfo<OrdersModel>(OrdersTypePath);
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].OrdersName == order)
                {
                    orders.RemoveAt(i);
                    CRUD.UpdateDB(OrdersTypePath, CRUD.Serialize(orders).Result);
                    return "Seccesfully updated";
                }
            }
            return "Not found";
        }
        public async Task<string> AddProductAsync(ProductsModel product)
        {
            products = await CRUD.GetAllInfo<ProductsModel>(ProductsPath);
            if(products.Any(res => res.ProductName.ToLower() == product.ProductName.ToLower()))
            {
                return "Already exist";
            }
            products.Add(product);
            await CRUD.UpdateDB(ProductsPath, CRUD.Serialize(products).Result);
            return "Successfully added";

        }
        public async Task<string> DeleteProductAsync(string productName)
        {
            products = await CRUD.GetAllInfo<ProductsModel>(ProductsPath);

            for (int i =  0; i < products.Count; i++)
            {
                if (products[i].ProductName == productName)
                {
                    products.RemoveAt(i);
                   await CRUD.UpdateDB(ProductsPath,CRUD.Serialize(products).Result);
                    return "Seccesfully deleted";
                }
            }
            return "Not found";
        }
        public async Task<string> UpdateProductAsync(string oldProductName,ProductsModel newProduct)
        {
            products = await CRUD.GetAllInfo<ProductsModel>(ProductsPath);
            for(int i = 0; i<products.Count; i++)
            {
                if (products[i].ProductName == oldProductName)
                {
                    products[i].ProductName = newProduct.ProductName;
                    products[i].Category = newProduct.Category;
                    products[i].ProductDescription = newProduct.ProductDescription;
                    products[i].Price = newProduct.Price;
                }
                return "Seccessfully updated";
            }
            return "Not found";
        }
        public async Task<string> UpdateOrderStatusAsync(string oldOrderStatus,string newOrderStatus)
        {
            usersOrders = await CRUD.GetAllInfo<OrderedProducts>("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Orders.json");
            for(int i = 0; i<usersOrders.Count; i++)
            {
                if (usersOrders[i].OrderedStatus == oldOrderStatus)
                {
                    usersOrders[i].OrderedStatus = newOrderStatus;
                   await CRUD.UpdateDB("C:\\Users\\USER\\Desktop\\HomeWork\\FastFoodBot\\FastFoodBot\\DB\\Orders.json", CRUD.Serialize(usersOrders).Result);
                   return "Updated";
                }
            }
            return "Not found";
        }
    }
}