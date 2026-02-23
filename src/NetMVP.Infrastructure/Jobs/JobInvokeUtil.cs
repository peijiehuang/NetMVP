using System.Reflection;
using System.Text.RegularExpressions;

namespace NetMVP.Infrastructure.Jobs;

/// <summary>
/// 任务调用工具类
/// 用于解析调用目标字符串并通过反射调用目标方法
/// </summary>
public static class JobInvokeUtil
{
    /// <summary>
    /// 执行方法
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    /// <param name="invokeTarget">调用目标字符串，格式：ClassName.MethodName 或 ClassName.MethodName(params)</param>
    public static async Task InvokeMethodAsync(IServiceProvider serviceProvider, string invokeTarget)
    {
        var beanName = GetBeanName(invokeTarget);
        var methodName = GetMethodName(invokeTarget);
        var methodParams = GetMethodParams(invokeTarget);

        // 从DI容器获取实例
        var beanType = FindTypeByName(beanName);
        if (beanType == null)
        {
            throw new InvalidOperationException($"找不到类型: {beanName}");
        }

        var bean = serviceProvider.GetService(beanType);
        if (bean == null)
        {
            throw new InvalidOperationException($"无法从DI容器获取实例: {beanName}");
        }

        await InvokeMethodAsync(bean, methodName, methodParams);
    }

    /// <summary>
    /// 调用任务方法
    /// </summary>
    private static async Task InvokeMethodAsync(object bean, string methodName, List<(object Value, Type Type)>? methodParams)
    {
        var beanType = bean.GetType();
        MethodInfo? method;

        if (methodParams != null && methodParams.Count > 0)
        {
            var paramTypes = methodParams.Select(p => p.Type).ToArray();
            method = beanType.GetMethod(methodName, paramTypes);
            
            if (method == null)
            {
                throw new InvalidOperationException($"找不到方法: {beanType.Name}.{methodName}({string.Join(", ", paramTypes.Select(t => t.Name))})");
            }

            var paramValues = methodParams.Select(p => p.Value).ToArray();
            var result = method.Invoke(bean, paramValues);
            
            // 如果方法返回Task，等待完成
            if (result is Task task)
            {
                await task;
            }
        }
        else
        {
            method = beanType.GetMethod(methodName, Type.EmptyTypes);
            
            if (method == null)
            {
                throw new InvalidOperationException($"找不到方法: {beanType.Name}.{methodName}()");
            }

            var result = method.Invoke(bean, null);
            
            // 如果方法返回Task，等待完成
            if (result is Task task)
            {
                await task;
            }
        }
    }

    /// <summary>
    /// 获取Bean名称
    /// 例如：DemoTask.NoParams -> DemoTask
    /// </summary>
    public static string GetBeanName(string invokeTarget)
    {
        var beanName = invokeTarget.Contains('(') 
            ? invokeTarget.Substring(0, invokeTarget.IndexOf('(')) 
            : invokeTarget;
        
        var lastDotIndex = beanName.LastIndexOf('.');
        return lastDotIndex > 0 ? beanName.Substring(0, lastDotIndex) : beanName;
    }

    /// <summary>
    /// 获取方法名称
    /// 例如：DemoTask.NoParams -> NoParams
    /// </summary>
    public static string GetMethodName(string invokeTarget)
    {
        var methodName = invokeTarget.Contains('(') 
            ? invokeTarget.Substring(0, invokeTarget.IndexOf('(')) 
            : invokeTarget;
        
        var lastDotIndex = methodName.LastIndexOf('.');
        return lastDotIndex > 0 ? methodName.Substring(lastDotIndex + 1) : methodName;
    }

    /// <summary>
    /// 获取方法参数列表
    /// 支持的参数类型：
    /// - String: 'text' 或 "text"
    /// - Boolean: true 或 false
    /// - Long: 1000L
    /// - Double: 99.99D
    /// - Integer: 100
    /// </summary>
    public static List<(object Value, Type Type)>? GetMethodParams(string invokeTarget)
    {
        if (!invokeTarget.Contains('('))
        {
            return null;
        }

        var startIndex = invokeTarget.IndexOf('(');
        var endIndex = invokeTarget.LastIndexOf(')');
        
        if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex)
        {
            return null;
        }

        var methodStr = invokeTarget.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
        
        if (string.IsNullOrEmpty(methodStr))
        {
            return null;
        }

        // 使用正则表达式分割参数，支持引号内的逗号
        var methodParams = SplitParams(methodStr);
        var result = new List<(object Value, Type Type)>();

        foreach (var param in methodParams)
        {
            var str = param.Trim();
            
            // String类型，以'或"开头
            if (str.StartsWith("'") || str.StartsWith("\""))
            {
                var value = str.Substring(1, str.Length - 2);
                result.Add((value, typeof(string)));
            }
            // Boolean类型
            else if (str.Equals("true", StringComparison.OrdinalIgnoreCase) || 
                     str.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                result.Add((bool.Parse(str), typeof(bool)));
            }
            // Long类型，以L结尾
            else if (str.EndsWith("L", StringComparison.OrdinalIgnoreCase))
            {
                var value = long.Parse(str.Substring(0, str.Length - 1));
                result.Add((value, typeof(long)));
            }
            // Double类型，以D结尾
            else if (str.EndsWith("D", StringComparison.OrdinalIgnoreCase))
            {
                var value = double.Parse(str.Substring(0, str.Length - 1));
                result.Add((value, typeof(double)));
            }
            // 其他类型归类为整型
            else
            {
                result.Add((int.Parse(str), typeof(int)));
            }
        }

        return result.Count > 0 ? result : null;
    }

    /// <summary>
    /// 分割参数字符串，支持引号内的逗号
    /// </summary>
    private static List<string> SplitParams(string paramsStr)
    {
        var result = new List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;
        var quoteChar = '\0';

        for (int i = 0; i < paramsStr.Length; i++)
        {
            var c = paramsStr[i];

            if ((c == '\'' || c == '"') && (i == 0 || paramsStr[i - 1] != '\\'))
            {
                if (!inQuotes)
                {
                    inQuotes = true;
                    quoteChar = c;
                }
                else if (c == quoteChar)
                {
                    inQuotes = false;
                }
                current.Append(c);
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
        {
            result.Add(current.ToString());
        }

        return result;
    }

    /// <summary>
    /// 根据类名查找类型
    /// </summary>
    private static Type? FindTypeByName(string typeName)
    {
        // 在当前应用程序域的所有程序集中查找类型
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            // 跳过系统程序集
            if (assembly.FullName?.StartsWith("System") == true || 
                assembly.FullName?.StartsWith("Microsoft") == true)
            {
                continue;
            }

            // 尝试完全限定名
            var type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }

            // 尝试在命名空间中查找
            var types = assembly.GetTypes()
                .Where(t => t.Name == typeName || t.FullName?.EndsWith("." + typeName) == true)
                .ToList();

            if (types.Count == 1)
            {
                return types[0];
            }
            else if (types.Count > 1)
            {
                // 如果找到多个，优先返回Jobs命名空间下的
                var jobType = types.FirstOrDefault(t => t.Namespace?.Contains("Jobs") == true);
                if (jobType != null)
                {
                    return jobType;
                }
                return types[0];
            }
        }

        return null;
    }
}
