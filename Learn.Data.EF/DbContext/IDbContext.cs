using System;
using System.Data.Entity.Infrastructure;

namespace Learn.Data.EF
{
    /// <summary>
    /// 创建人：翁建勋
    /// 日 期：2016.04.07
    /// 描 述：数据库连接接口 
    /// </summary>
    public interface IDbContext: IDisposable, IObjectContextAdapter
    {
    }
}
