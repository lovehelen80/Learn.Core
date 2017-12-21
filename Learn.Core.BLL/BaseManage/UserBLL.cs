using Learn.Core.DAL.BaseManage;
using Learn.Core.Entity;
using Learn.Core.IDAL.BaseManage;
using Learn.Core.Util.WebControl;
using System.Collections.Generic;

namespace Learn.Core.BLL
{
    public class UserBLL
    {
        private IUserService service = new UserService();
        #region 获取数据
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination, queryJson);
        }
        #endregion
    }
}
