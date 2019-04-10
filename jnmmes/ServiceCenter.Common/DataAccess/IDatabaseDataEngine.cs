using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ServiceCenter.Model;

namespace ServiceCenter.DataAccess
{
    /// <summary>
    /// 数据库数据操作接口。
    /// </summary>
    public interface IDatabaseDataEngine<T, K> where T : BaseModel<K>
    {

        ISessionFactory SessionFactory
        {
            get;
            set;
        }
        /// <summary>
        /// 新增数据。
        /// </summary>
        /// <param name="obj">T 类型对象。</param>
        void Insert(T obj);

        void Insert(T obj,ISession session);
        /// <summary>
        /// 修改数据。
        /// </summary>
        /// <param name="obj">T 类型对象。</param>
        void Update(T obj);

        void Update(T obj, ISession session);
        /// <summary>
        /// 删除数据。
        /// </summary>
        /// <param name="key">K 类型对象，数据表关键字。</param>
        void Delete(K key);

        void Delete(K key, ISession session);
        /// <summary>
        /// 删除数据。
        /// </summary>
        /// <param name="condition">删除条件。</param>
        int DeleteByCondition(string condition);

        int DeleteByCondition(string condition, ISession session);
        /// <summary>
        /// 修改数据。如果数据不存在则新增，如果数据存在则更新。
        /// </summary>
        /// <param name="obj">T 类型对象。</param>
        void Modify(T obj);

        void Modify(T obj, ISession session);
        /// <summary>
        /// 是否存在指定关键字的记录。
        /// </summary>
        /// <param name="key">K 类型对象，数据表关键字。</param>
        /// <returns>true存在；false不存在。</returns>
        bool IsExists(K key);

        bool IsExists(K key, ISession session);
        /// <summary>
        /// 查询数据。
        /// </summary>
        /// <param name="key">K 类型对象，数据表关键字。</param>
        /// <returns>T 类型对象。。</returns>
        T Get(K key);

        T Get(K key, ISession session);
        /// <summary>
        /// 查询数据。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns>数据集合。</returns>
        IList<T> Get(PagingConfig cfg);

        IList<T> Get(PagingConfig cfg, ISession session);
    }
}
