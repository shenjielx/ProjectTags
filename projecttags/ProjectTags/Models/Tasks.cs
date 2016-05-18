using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectTags.Models
{
    /// <summary>
    /// 任务
    /// </summary>
    public class Tasks : BaseObj
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public long ProjectID { get; set; }
        /// <summary>
        /// 状态ID
        /// </summary>
        public long? StateID { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        [MaxLength(320)]
        public string Name { get; set; }
        /// <summary>
        /// 任务简介
        /// </summary>
        [MaxLength(2000)]
        public string Desc { get; set; }
        /// <summary>
        /// 计划开始时间
        /// </summary>
        public DateTime? PlanStart { get; set; }
        /// <summary>
        /// 计划结束时间
        /// </summary>
        public DateTime? PlanEnd { get; set; }
        /// <summary>
        /// 实际开始时间
        /// </summary>
        public DateTime? RealStart { get; set; }
        /// <summary>
        /// 实际结束时间
        /// </summary>
        public DateTime? RealEnd { get; set; }
        /// <summary>
        /// 类型（1-需求，2-Bug）
        /// </summary>
        public TasksType Type { get; set; }

        /// <summary>
        /// 项目信息
        /// </summary>
        [ForeignKey("ProjectID")]
        public virtual Projects Project { get; set; }

        [ForeignKey("StateID")]
        public virtual Status State { get; set; }
    }
    /// <summary>
    /// 任务类型
    /// </summary>
    public enum TasksType
    {
        DEFAULT = 0,
        REQUIRED = 1,
        BUG = 2,
    }
}