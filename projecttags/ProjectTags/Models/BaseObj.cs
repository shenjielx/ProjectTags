using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectTags.Models
{
    public class BaseObj
    {
        /// <summary>
        ///  主键
        /// </summary>
        [Key]
        public long ID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public long CreateID { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 最后更新人
        /// </summary>
        public long? UpdateID { get; set; }
        /// <summary>
        /// 最后操作IP
        /// </summary>
        [MaxLength(32)]
        public string WebClientIP { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsEnable { get; set; }

        [ForeignKey("CreateID")]
        public virtual Users Create { get; set; }
        [ForeignKey("UpdateID")]
        public virtual Users Update { get; set; }
        public BaseObj()
        {
            IsEnable = true;
            CreateTime = DateTime.Now;
        }
    }
}