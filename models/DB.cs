using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Avalonia.Controls;
using Dapper;
using Npgsql;

namespace obyv010;

public class DB
{
    string connectionString = "Host = localhost; Port = 5432; Database = db_obyv010; Username = postgres; Password = PostgreSQL";

    public void Authorization(string login, string password)
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string sql = @"select u.id as id, r.name as role
                            from role_users r inner join users u
                            on r.id = u.role_user_id
                            where u.login = @Login and u.password = @Password";

            using(var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("Login", login);
                command.Parameters.AddWithValue("Password", password);

                using(var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        UserAuthorization.id = reader.GetInt32(0);
                        UserAuthorization.role = reader.GetString(1);
                    }
                    else
                    {
                        UserAuthorization.id = -1;
                        UserAuthorization.role = "";
                    }
                }
            }

            connection.Close();
        }
    }

    public List<Product> GetProducts()
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"select p.id as id, p.article as article, p.title_product_id as titleProductId,
                            t.name as titleProduct, p.unit as unit, p.price as price, p.supplier_product_id as supplierId,
                            s.name as supplier, p.manufacture_product_id as manufactureId, m.name as manufacture,
                            p.category_product_id as categoryId, c.name as category, p.discount as discount,
                            p.count as count, coalesce(p.description, '') as description, coalesce(p.image, '') as image
                            from title_products t inner join products p
                            on t.id = p.title_product_id
                            inner join supplier_products s
                            on s.id = p.supplier_product_id
                            inner join manufacture_products m
                            on m.id = p.manufacture_product_id
                            inner join category_products c
                            on c.id = p.category_product_id";

            return connection.Query<Product>(sql).ToList();
        }
    }

    public List<Supplier> GetSuppliers()
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"select id, name from supplier_products";

            return connection.Query<Supplier>(sql).ToList();
        }
    }

    public List<Manufacture> GetManufactures()
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"select id, name from manufacture_products";

            return connection.Query<Manufacture>(sql).ToList();
        }
    }

    public List<TitleProduct> GetTitleProducts()
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"select id, name from title_products";

            return connection.Query<TitleProduct>(sql).ToList();
        }
    }

    public List<Category> GetCategories()
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"select id, name from category_products";

            return connection.Query<Category>(sql).ToList();
        }
    }

    public List<Order> GetOrders()
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"select o.id as id, o.date_order as dateOrder, o.date_delivery as dateDelivery,
                                    o.pick_up_point_id as pickUpPointId, 
                                    concat(p.code, ' г. ', p.city, ' ул. ', p.street, coalesce(' ' || p.home, '')) as pickUpPoint,
                                    o.user_id as userId, o.code as code, o.status_order_id as statusId, s.name as status 
                                    from status_orders s inner join orders o
                                    on s.id = o.status_order_id
                                    inner join pick_up_points p
                                    on p.id = o.pick_up_point_id";

            return connection.Query<Order>(sql).ToList();
        }
    }

    public List<StatusOrder> GetStatusOrders()
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"select id, name from status_orders";

            return connection.Query<StatusOrder>(sql).ToList();
        }
    }

    public List<PickUpPoint> GetPickUpPoints()
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"select id, code, city, street, home from pick_up_points";

            return connection.Query<PickUpPoint>(sql).ToList();
        }
    }

    public int IdNewProduct()
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"select coalesce(max(id), 0) + 1 from products";

            return connection.QueryFirstOrDefault<int>(sql);
        }
    }

    public string InsertProduct(Product product)
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"select count(*) > 0 from products where article = @Article";

            bool result = connection.QueryFirstOrDefault<Boolean>(sql, new {Article = product.article});

            if (result)
            {
                return "-1";
            }

            sql = @"insert into products(id, article, title_product_id, unit, price, supplier_product_id,
                                            manufacture_product_id, category_product_id, discount, count, 
                                            description, image) values
                    (@Id, @Article, @TitleProduct, @Unit, @Price, @Supplier, @Manufacture, @Category, @Discount,
                        @Count, @Description, @Image)";

            string description = null;
            string image = null;

            if(product.description != "") description = product.description;
            if(product.image != "") image = product.image;

            connection.Execute(sql, new
            {
                Id = product.id,
                Article = product.article,
                TitleProduct = product.titleProductId,
                Unit = product.unit,
                Price = product.price,
                Supplier = product.supplierId,
                Manufacture = product.manufactureId,
                Category = product.categoryId,
                Discount = product.discount,
                Count = product.count,
                Description = description,
                Image = image
            });

            return "1";
        }
    }

    public void UpdateProduct(Product product)
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            string description = null;
            string image = null;

            if(product.description != "") description = product.description;
            if(product.image != "") image = product.image;


            string sql = @"update products
                            set title_product_id = @TitleProduct, unit = @Unit, price = @Price, supplier_product_id = @Supplier,
                                manufacture_product_id = @Manufacture, category_product_id = @Category, discount = @Discount,
                                count = @Count, description = @Description, image = @Image
                                where article = @Article";

            connection.Execute(sql, new
            {
                Article = product.article,
                TitleProduct = product.titleProductId,
                Unit = product.unit,
                Price = product.price,
                Supplier = product.supplierId,
                Manufacture = product.manufactureId,
                Category = product.categoryId,
                Discount = product.discount,
                Count = product.count,
                Description = description,
                Image = image
            });
        }
    }

    public string DeleteProduct(int id)
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"select count(*) > 0 from order_details where product_id = @Id";

            bool result = connection.QueryFirstOrDefault<bool>(sql, new {Id = id});

            if(result) return "-1";

            sql = @"delete from products where id = @Id";

            connection.Execute(sql, new {Id = id});

            return "1";
        }
    } 

    public List<OrderDetail> GetOrderDetails(int orderId)
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"select id, order_id as orderId, product_id as productId, count from order_details
                            where order_id = @OrderId";

            return connection.Query<OrderDetail>(sql, new{OrderId = orderId}).ToList();
        }
    }

    public string InsertOrder(Order order, List<OrderDetail> orderDetails)
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            using(var transaction = connection.BeginTransaction())
            {
                try
                {
                    string sql = @"select count(*) > 0 from orders where id = @Id";

                    bool result = connection.QueryFirstOrDefault<bool>(sql, new {Id = order.id});

                    if(result) return "-1";

                    sql = @"insert into orders(id, date_order, date_delivery, pick_up_point_id, user_id, code,
                                                status_order_id) values
                                                (@Id, @DateOrder, @DateDelivery, @Pick, @User, @Code, @Status)";

                    connection.Execute(sql, new
                    {
                        Id = order.id,
                        DateOrder = order.dateOrder.Date,
                        DateDelivery = order.dateDelivery.Date,
                        Pick = order.pickUpPointId,
                        User = UserAuthorization.id,
                        Code = order.code,
                        Status = order.statusId 
                    }, transaction:transaction);

                    sql = "select coalesce(max(id), 0) + 1 from order_details;";

                    int id0 = connection.QueryFirstOrDefault<int>(sql, transaction:transaction);

                    foreach(var detail in orderDetails)
                    {

                        sql = @"select count from products where id = @ProductId";
                        int currentCount = connection.QueryFirstOrDefault<int>(sql, 
                            new { ProductId = detail.productId }, 
                            transaction: transaction);
                        
                        if (currentCount < detail.count)
                        {
                            transaction.Rollback();
                            return "-3";
                        }

                        sql = @"insert into order_details(id, order_id, product_id, count) values
                        (@ID, @OrderId, @ProductId, @Count)";

                        connection.Execute(sql, new
                        {
                            ID = detail.id + id0,
                            OrderId = detail.orderId,
                            ProductId = detail.productId,
                            Count = detail.count 
                        }, transaction:transaction);

                        sql = @"update products
                                set count = count - @Count
                                where id = @ID";

                        connection.Execute(sql, new
                        {
                            ID = detail.productId,
                            Count = detail.count 
                        }, transaction:transaction);
                    }


                    transaction.Commit();

                    return "1";
                }
                catch
                {
                    transaction.Rollback();
                    return "-2";
                }
            }

            
            

            
        }
    }

    public string UpdateOrder(Order order, List<OrderDetail> orderDetails, List<OrderDetail> oldOrderDetails)
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            using(var transaction = connection.BeginTransaction())
            {
                try
                {
                    string sql = @"update orders
                            set date_order = @DateOrder, date_delivery = @DateDelivery, pick_up_point_id = @Pick,
                            code = @Code, status_order_id = @Status
                            where id = @Id";

                    connection.Execute(sql, new
                    {
                        Id = order.id,
                        DateOrder = order.dateOrder.Date,
                        DateDelivery = order.dateDelivery.Date,
                        Pick = order.pickUpPointId,
                        Code = order.code,
                        Status = order.statusId 
                    }, transaction:transaction);

                    
                    foreach(var detail in oldOrderDetails)
                    {

                        sql = @"update products
                                set count = count + @Count
                                where id = @ID";

                        connection.Execute(sql, new
                        {
                            ID = detail.productId,
                            Count = detail.count 
                        }, transaction:transaction);
                    }

                    sql = @"delete from order_details where order_id = @Id";

                    connection.Execute(sql, new
                    {
                        Id = order.id
                    }, transaction:transaction);

                    sql = "select coalesce(max(id), 0) + 1 from order_details;";

                    int id0 = connection.QueryFirstOrDefault<int>(sql, transaction:transaction);

                    foreach(var detail in orderDetails)
                    {

                        sql = @"select count from products where id = @ProductId";
                        int currentCount = connection.QueryFirstOrDefault<int>(sql, 
                            new { ProductId = detail.productId }, 
                            transaction: transaction);
                        
                        if (currentCount < detail.count)
                        {
                            transaction.Rollback();
                            return "-3";
                        }


                        sql = @"insert into order_details(id, order_id, product_id, count) values
                        (@ID, @OrderId, @ProductId, @Count)";

                        connection.Execute(sql, new
                        {
                            ID = detail.id + id0,
                            OrderId = detail.orderId,
                            ProductId = detail.productId,
                            Count = detail.count 
                        }, transaction:transaction);

                        

                        sql = @"update products
                                set count = count - @Count
                                where id = @ID";

                        connection.Execute(sql, new
                        {
                            ID = detail.productId,
                            Count = detail.count 
                        }, transaction:transaction);
                    }

                    transaction.Commit();

                    return "1";
                }
                catch
                {
                    transaction.Rollback();

                    return "-1";
                }
            }

            

        }
    }

    public void DeleteOrder(int id)
    {
        using(var connection = new NpgsqlConnection(connectionString))
        {
            var orderDetails = GetOrderDetails(id);

            string sql = @"";

            foreach(var detail in orderDetails)
            {

                sql = @"update products
                        set count = count + @Count
                        where id = @ID";

                connection.Execute(sql, new
                {
                    ID = detail.productId,
                    Count = detail.count 
                });
            }

            sql = @"delete from order_details where order_id = @Id";

            connection.Execute(sql, new
            {
               Id = id
            });

            sql = @"delete from orders where id = @Id";

            connection.Execute(sql, new
            {
               Id = id
            });

        }
    }
}
