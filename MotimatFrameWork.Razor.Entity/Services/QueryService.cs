using MotimatFramework.Core.SubEntities.QueryModel;
using MotimatFrameWork.Razor.Entity.Interfaces;
using MotimatFrameWork.Razor.Entity.Services;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MotimatFramework.Razor.Entity.Services
{
    public class FluentQuery<T>
    {
        private readonly IFrontLogger _logger;
        private readonly RouteConfig _routeConfig;
        public FluentQuery(IFrontLogger logger,HttpClient httpClient,RouteConfig routeConfig)
        {
            _logger = logger;
            this.httpClient = httpClient;
            _routeConfig = routeConfig;
        }


        private HttpClient httpClient;
        private readonly QueryRequest _request = new();

        public FluentQuery<T> Include(Expression<Func<T, object>> expression)
        {
            var propName = GetPropertyPath(expression.Body);
            _request.Includes.Add(propName);
            return this;
        }

        public void Clear()
        {
            _request.Filters = new();
            _request.Includes = new();
            _request.Take = null;
            _request.Skip = null;
            _request.ObjectType = null;

        }

        public FluentQuery<T> Where(Expression<Func<T, bool>> expression)
        {
            if (expression.Body is BinaryExpression binary)
            {
                var left = GetPropertyName((MemberExpression)binary.Left);
                var op = GetOperator(binary.NodeType);
                var right = Expression.Lambda(binary.Right).Compile().DynamicInvoke()?.ToString();

                _request.Filters.Add(new Filter
                {
                    Field = left,
                    Operator = op,
                    Value = right
                });
            }
            else if (expression.Body is MethodCallExpression methodCall)
            {
                var method = methodCall.Method.Name;

                if (method is "Contains" or "StartsWith" or "EndsWith")
                {
                    var field = GetPropertyName(methodCall.Object ?? methodCall.Arguments[0]);
                    var value = Expression.Lambda(methodCall.Arguments[^1]).Compile().DynamicInvoke()?.ToString();

                    _request.Filters.Add(new Filter
                    {
                        Field = field,
                        Operator = method, // توی سرور بررسی کن با این نام یا تبدیل کن
                        Value = value
                    });
                }
                else
                {
                    throw new NotSupportedException($"Unsupported method: {method}");
                }
            }





            return this;
        }

        public FluentQuery<T> Skip(int count)
        {
            _request.Skip = count;
            return this;
        }

        public FluentQuery<T> Take(int count)
        {
            _request.Take = count;
            return this;
        }



        public QueryRequest Build() => _request;

        public async Task<HttpResponseMessage> PostAsync(string token)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _request.ObjectType = typeof(T).AssemblyQualifiedName;
                var result = await httpClient.PostAsJsonAsync(_routeConfig.GenericApiGetRequest, _request);
                return result;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage();
            }
            finally
            {
                Clear();
            }


        }

        public async Task<List<T>> ResultPostAsync(string token)
        {
            try
            {
                var result = await PostAsync(token);
                //var ssss = await result.Content.ReadFromJsonAsync<List<T>>();
                if (result != null && await result.Content.ReadFromJsonAsync<List<T>>() is List<T> rr)
                {
                    return rr;
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Error read from api {ex.Message}",LogMode.Error);
            }
            return new List<T>();
        }


        public async Task<HttpResponseMessage> InsertAsync(T model, string token)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                AlterRequest req = new AlterRequest() { Model = model, ObjectType = typeof(T).AssemblyQualifiedName };
                return await httpClient.PostAsJsonAsync(_routeConfig.GenericApiCreateRequest, req);
            }
            finally
            {

                Clear();
            }

        }


        public async Task<HttpResponseMessage> UpdateAsync(T model, string token)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                AlterRequest req = new AlterRequest() { Model = model, ObjectType = typeof(T).AssemblyQualifiedName };
                return await httpClient.PostAsJsonAsync(_routeConfig.GenericApiUpdateRequest, req);
            }
            finally
            {
                Clear();
            }

        }

        public async Task<HttpResponseMessage> DeleteAsync(T model,string token)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                AlterRequest req = new AlterRequest() { Model = model, ObjectType = typeof(T).AssemblyQualifiedName };
                return await httpClient.PostAsJsonAsync(_routeConfig.GenericApiDeleteRequest, req);
            }
            finally
            {
                Clear();
            }

        }



        private string GetOperator(ExpressionType type)
        {
            return type switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.NotEqual => "!=",
                _ => throw new NotSupportedException("Unsupported operator")
            };
        }



        private string GetPropertyName(Expression expression)
        {
            return expression switch
            {
                MemberExpression me => me.Member.Name,
                UnaryExpression ue when ue.Operand is MemberExpression me => me.Member.Name,
                _ => throw new NotSupportedException("Unsupported expression")
            };
        }


        private string GetPropertyPath(Expression expression)
        {
            if (expression is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression)
            {
                expression = unaryExpr.Operand;
            }

            var members = new Stack<string>();

            while (expression is MemberExpression member)
            {
                members.Push(member.Member.Name);
                expression = member.Expression;
            }

            return string.Join(".", members);
        }


    }


    public static class GenericQuery
    {
        public static async Task<List<object>?> GetTable(this HttpClient httpClient, Type tableType,string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await httpClient.PostAsJsonAsync("api/generic/get", new QueryRequest { ObjectType = tableType.AssemblyQualifiedName });
            return await result.Content.ReadFromJsonAsync<List<object>>();
        }

        public static async Task<List<IEntity>?> GetTable<IEntity>(this HttpClient httpClient,string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await httpClient.PostAsJsonAsync("api/generic/get", new QueryRequest { ObjectType = typeof(IEntity).AssemblyQualifiedName });
            return await result.Content.ReadFromJsonAsync<List<IEntity>>();
        }

        public static async Task<object?> GetItemAsync(this HttpClient httpClient, Type type, long Id)
        {
            var result = await httpClient.PostAsJsonAsync("api/generic/getitem", new { typeId = type.AssemblyQualifiedName, id = Id });
            return await result.Content.ReadFromJsonAsync(type);
        }

        public static async Task<TEntity> GetItemAsync<TEntity>(this HttpClient httpClient, long Id)
        {
            return (TEntity)await httpClient.GetItemAsync(typeof(TEntity), Id);
        }

        public static async Task<object?> GetItemAsync(this HttpClient httpClient, Type type, Guid Guid)
        {
            var result = await httpClient.PostAsJsonAsync("api/generic/getitem", new { typeId = type.AssemblyQualifiedName, gid = Guid });
            return await result.Content.ReadFromJsonAsync(type);
        }

        public static async Task<TEntity> GetItemAsync<TEntity>(this HttpClient httpClient, Guid Guid)
        {
            return (TEntity)await httpClient.GetItemAsync(typeof(TEntity), Guid);
        }

        public static async Task<HttpResponseMessage> InsertAsync(this HttpClient httpClient, Type type, object data)
        {
            AlterRequest req = new AlterRequest() { Model = data, ObjectType = type.AssemblyQualifiedName };
            return await httpClient.PostAsJsonAsync("api/generic/insert", req);
        }

        public static async Task<HttpResponseMessage> InsertAsync<T>(this HttpClient httpClient, T model)
        {
            AlterRequest req = new AlterRequest() { Model = model, ObjectType = typeof(T).AssemblyQualifiedName };
            return await httpClient.PostAsJsonAsync("api/generic/insert", req);
        }


        public static async Task<HttpResponseMessage> UpdateAsync<T>(this HttpClient httpClient, T model)
        {
            AlterRequest req = new AlterRequest() { Model = model, ObjectType = typeof(T).AssemblyQualifiedName };
            return await httpClient.PostAsJsonAsync("api/generic/update", req);
        }


        public static QueryRequest Where<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            QueryRequest _request = new();
            _request.ObjectType = typeof(TEntity).AssemblyQualifiedName;

            if (expression.Body is BinaryExpression binary)
            {
                var left = GetPropertyName((MemberExpression)binary.Left);
                var op = GetOperator(binary.NodeType);
                var right = Expression.Lambda(binary.Right).Compile().DynamicInvoke()?.ToString();

                _request.Filters.Add(new Filter
                {
                    Field = left,
                    Operator = op,
                    Value = right
                });
            }
            else if (expression.Body is MethodCallExpression methodCall)
            {
                var method = methodCall.Method.Name;

                if (method is "Contains" or "StartsWith" or "EndsWith")
                {
                    var field = GetPropertyName(methodCall.Object ?? methodCall.Arguments[0]);
                    var value = Expression.Lambda(methodCall.Arguments[^1]).Compile().DynamicInvoke()?.ToString();

                    _request.Filters.Add(new Filter
                    {
                        Field = field,
                        Operator = method, // توی سرور بررسی کن با این نام یا تبدیل کن
                        Value = value
                    });
                }
                else
                {
                    throw new NotSupportedException($"Unsupported method: {method}");
                }
            }
            return _request;
        }

        public static async Task<List<object>> FetchToListAsync(this HttpClient httpClient, Type type, QueryRequest? query)
        {
            if (query == null)
                query = new();
            var result = await httpClient.PostAsJsonAsync("api/generic/get", query);
            return await result.Content.ReadFromJsonAsync<List<object>>();
        }





        private static string GetOperator(ExpressionType type)
        {
            return type switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.NotEqual => "!=",
                _ => throw new NotSupportedException("Unsupported operator")
            };
        }



        private static string GetPropertyName(Expression expression)
        {
            return expression switch
            {
                MemberExpression me => me.Member.Name,
                UnaryExpression ue when ue.Operand is MemberExpression me => me.Member.Name,
                _ => throw new NotSupportedException("Unsupported expression")
            };
        }

        public static string GetImgSrc(this HttpClient httpClient, string imgName)
        {
            return httpClient.BaseAddress + "api/book-image/" + imgName + "?width=50&&height=50";
        }


    }



}
