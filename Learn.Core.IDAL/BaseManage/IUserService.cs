using Learn.Core.Entity;
using Learn.Core.Util.WebControl;
using System.Collections.Generic;

namespace Learn.Core.IDAL.BaseManage
{
    public interface IUserService 
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        IEnumerable<UserEntity> GetPageList(Pagination pagination, string queryJson);
    }
}
