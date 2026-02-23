namespace NetMVP.Application.Jobs;

/// <summary>
/// 定时任务Demo示例
/// </summary>
public class DemoTask : JobTaskBase
{

    /// <summary>
    /// 无参数任务示例
    /// 用途：定期清理临时文件、发送系统通知等
    /// </summary>
    public void NoParams()
    {
        Log("开始执行无参数任务");
        
        // 模拟业务逻辑
        Thread.Sleep(1000); // 模拟耗时操作
        
        Log("任务执行完成");
    }

    /// <summary>
    /// 单参数任务示例
    /// 用途：根据参数执行不同的业务逻辑
    /// </summary>
    /// <param name="message">消息内容</param>
    public void SingleParam(string message)
    {
        Log("接收到参数: {0}", message);
        
        // 根据参数执行不同逻辑
        if (message.Contains("重要"))
        {
            Log("检测到重要消息，优先处理");
        }
        
        Log("参数处理完成");
    }

    /// <summary>
    /// 多参数任务示例
    /// 用途：复杂的业务场景，需要多个参数配合
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="enabled">是否启用</param>
    /// <param name="count">数量</param>
    /// <param name="price">价格</param>
    /// <param name="type">类型</param>
    public void MultipleParams(string name, bool enabled, long count, double price, int type)
    {
        Log("参数: 名称={0}, 启用={1}, 数量={2}, 价格={3}, 类型={4}", 
            name, enabled, count, price, type);
        
        // 根据参数执行业务逻辑
        if (enabled && count > 0)
        {
            var totalAmount = count * price;
            Log("处理 {0}，数量: {1}，总金额: {2}", name, count, totalAmount);
        }
        else
        {
            Log("任务未启用或数量为0，跳过处理");
        }
    }

    /// <summary>
    /// 数据同步任务示例
    /// 用途：定期同步数据、备份数据等
    /// </summary>
    public async Task SyncDataAsync()
    {
        Log("步骤1: 连接数据源");
        await Task.Delay(500);
        
        Log("步骤2: 读取数据");
        await Task.Delay(500);
        
        Log("步骤3: 处理数据");
        await Task.Delay(500);
        
        Log("步骤4: 保存数据");
        await Task.Delay(500);
        
        Log("数据同步成功，共处理 100 条数据");
    }

    /// <summary>
    /// 报表生成任务示例
    /// 用途：定期生成报表、统计数据等
    /// </summary>
    /// <param name="reportType">报表类型（daily/weekly/monthly）</param>
    public void GenerateReport(string reportType)
    {
        var reportName = reportType switch
        {
            "daily" => "日报表",
            "weekly" => "周报表",
            "monthly" => "月报表",
            _ => "未知报表"
        };
        
        Log("开始生成{0}", reportName);
        
        Thread.Sleep(1000); // 模拟生成报表
        
        var filePath = $"/reports/{reportType}_{DateTime.Now:yyyyMMdd}.xlsx";
        Log("生成{0}成功: {1}", reportName, filePath);
    }

    /// <summary>
    /// 邮件发送任务示例
    /// 用途：定期发送邮件通知、提醒等
    /// </summary>
    /// <param name="recipient">收件人</param>
    /// <param name="subject">主题</param>
    public void SendEmail(string recipient, string subject)
    {
        Log("准备发送邮件: {0} - {1}", recipient, subject);
        
        // 模拟邮件发送
        Log("连接邮件服务器");
        Thread.Sleep(500);
        
        Log("发送邮件中");
        Thread.Sleep(500);
        
        Log("邮件发送成功");
    }
}
