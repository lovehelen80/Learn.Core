using Learn.Core.Data.Repository;
using Learn.Core.Entity;
using Learn.Core.IDAL.BaseManage;
using Learn.Core.Util;
using Learn.Core.Util.WebControl;
using System.Collections.Generic;

namespace Learn.Core.DAL.BaseManage
{
    public class UserService : RepositoryFactory<UserEntity>, IUserService
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetPageList(Pagination pagination, string queryJson)
        {
            var expression = LinqExtensions.True<UserEntity>();
            var queryParam = queryJson.ToJObject();
            //公司主键
            if (!queryParam["organizeId"].IsEmpty())
            {
                string organizeId = queryParam["organizeId"].ToString();
                expression = expression.And(t => t.OrganizeId.Equals(organizeId));
            }
            //部门主键
            if (!queryParam["departmentId"].IsEmpty())
            {
                string departmentId = queryParam["departmentId"].ToString();
                expression = expression.And(t => t.DepartmentId.Equals(departmentId));
            }
            //主管
            if (!queryParam["ManagerId"].IsEmpty())
            {
                string managerId = queryParam["ManagerId"].ToString();
                expression = expression.And(t => t.ManagerId.Equals(managerId));
            }
            //状态
            if (!queryParam["EnabledMark"].IsEmpty())
            {
                int enabledMark = queryParam["EnabledMark"].ToInt();
                expression = expression.And(t => t.EnabledMark == enabledMark);
            }
            //角色
            if (!queryParam["RoleId"].IsEmpty())
            {
                string roleId = queryParam["RoleId"].ToString();
                expression = expression.And(t => t.RoleId == roleId);
            }
            //查询条件
            if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            {
                string condition = queryParam["condition"].ToString();
                string keyord = queryParam["keyword"].ToString().Trim(); ;
                switch (condition)
                {
                    case "Account":            //账户
                        expression = expression.And(t => t.Account.Contains(keyord));
                        break;
                    case "RealName":          //姓名
                        expression = expression.And(t => t.RealName.Contains(keyord));
                        break;
                    case "Mobile":          //手机
                        expression = expression.And(t => t.Mobile.Contains(keyord));
                        break;
                    //case "ManagerId":          //主管
                    //    expression = expression.And(t => t.ManagerId.Equals(keyord));
                    //    break;
                    default:
                        break;
                }
            }
            //expression = expression.And(t => t.UserId != "System");
            return this.BaseRepository().FindList(expression, pagination);
        }
    }
}
