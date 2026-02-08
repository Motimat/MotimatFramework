using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Mf.Core.SubEntities.QueryModel;

namespace Mf.Razor.Entity.Services;

public class FluentQuery<T>
    {
        private HttpClient httpClient;

        public FluentQuery(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
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
                var result = await httpClient.PostAsJsonAsync("api/generic/get", _request);
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
                //ServiceLocator.Log($"Error read from api {ex.Message}");
            }
            return new List<T>();
        }


        public async Task<HttpResponseMessage> InsertAsync(T model,string token)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                AlterRequest req = new AlterRequest() { Model = model, ObjectType = typeof(T).AssemblyQualifiedName };
                return await httpClient.PostAsJsonAsync("api/generic/insert", req);
            }
            finally
            {

                Clear();
            }

        }


        public async Task<HttpResponseMessage> UpdateAsync(T model,string token)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                AlterRequest req = new AlterRequest() { Model = model, ObjectType = typeof(T).AssemblyQualifiedName };
                return await httpClient.PostAsJsonAsync("api/generic/update", req);
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
                return await httpClient.PostAsJsonAsync("api/generic/delete", req);
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

