namespace NetMVP.Infrastructure.Helpers;

/// <summary>
/// 树形结构工具类
/// </summary>
public static class TreeHelper
{
    /// <summary>
    /// 构建树形结构
    /// </summary>
    public static List<T> BuildTree<T>(List<T> list, long parentId = 0) where T : ITreeNode<T>
    {
        var tree = new List<T>();

        foreach (var item in list.Where(x => x.ParentId == parentId))
        {
            item.Children = BuildTree(list, item.Id);
            tree.Add(item);
        }

        return tree;
    }

    /// <summary>
    /// 展平树形结构
    /// </summary>
    public static List<T> FlattenTree<T>(List<T> tree) where T : ITreeNode<T>
    {
        var result = new List<T>();

        foreach (var item in tree)
        {
            result.Add(item);
            if (item.Children != null && item.Children.Count > 0)
            {
                result.AddRange(FlattenTree(item.Children));
            }
        }

        return result;
    }
}

/// <summary>
/// 树形节点接口
/// </summary>
public interface ITreeNode<T>
{
    long Id { get; }
    long ParentId { get; }
    List<T>? Children { get; set; }
}
