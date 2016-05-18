using ProjectTags.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectTags.Unitil
{
    public class ToolDict
    {
        #region 等级
        /// <summary>
        /// 等级
        /// </summary>
        public static Dictionary<int, string> UsersRankDict = new Dictionary<int, string>()
        {
            { (int)UsersRank.DEFAULT,"普通用户"},
            { (int)UsersRank.ADMIN,"管理员"},
            { (int)UsersRank.SUPER,"超级管理员"}
        };
        #endregion

        #region 性别
        /// <summary>
        /// 性别
        /// </summary>
        public static Dictionary<int, string> UsersGenderDict = new Dictionary<int, string>()
        {
            { (int)UsersGender.MALE,"男"},
            { (int)UsersGender.FEMALE,"女"},
            { (int)UsersGender.SECRET,"保密"}
        };
        #endregion

        #region 任务类型
        /// <summary>
        /// 任务类型
        /// </summary>
        public static Dictionary<int, string> TasksTypeDict = new Dictionary<int, string>()
        {
            { (int)TasksType.REQUIRED,"需求"},
            { (int)TasksType.BUG,"Bug"}
        };
        #endregion

        #region 根据Key获取Value
        /// <summary>
        /// 根据Key获取Value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string GetDictValue(int key, Dictionary<int, string> dict)
        {
            var value = "";
            if (dict != null)
            {
                if (dict.ContainsKey(key))
                {
                    value = dict[key];
                }
            }
            return value;
        }
        #endregion
    }
}