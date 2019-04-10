using NHibernate;
using ServiceCenter.DataAccess;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.Common.DataAccess.NHibernate
{
    /// <summary>
    /// 表示数据库数据访问类。
    /// </summary>
    public abstract class DatabaseDataEngine<T, K> :
        IDatabaseDataEngine<T, K> where T : BaseModel<K>
    {
        /// <summary>
        /// 数据库会话工厂。
        /// </summary>
        public ISessionFactory SessionFactory
        {
            get;
            set;
        }

        public DatabaseDataEngine(ISessionFactory sf)
        {
            this.SessionFactory = sf;
        }

        public void Insert(T obj)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                object identifier = session.Save(obj);
                session.Flush();
            }
        }

        public void Insert(T obj, ISession session)
        {
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenSession();
            }
            object identifier = session.Save(obj);
            //session.Flush();
        }

        public void Update(T obj)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                session.Update(obj);
                session.Flush();                
            }
        }

        public void Update(T obj, ISession session)
        {
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenSession();
            }
            session.Update(obj);
            //session.Flush();
           
        }

        public void Delete(K key)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                T obj = session.Get<T>(key);
                session.Delete(obj);
                session.Flush();
            }
        }

        public void Delete(K key, ISession session)
        {
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenSession();
            }
            T obj = session.Get<T>(key);
            session.Delete(obj);
            //session.Flush();
        }

        public int DeleteByCondition(string condition)
        {
            string typeName = typeof(T).ToString();
            using (ISession session = this.SessionFactory.OpenSession())
            {
                var sql = string.Format(@"DELETE FROM {1} 
                                          WHERE {0}"
                                          ,condition
                                          ,typeName);
                IQuery qry = session.CreateQuery(sql);
                return qry.ExecuteUpdate();
            }
        }

        public int DeleteByCondition(string condition, ISession session)
        {
            string typeName = typeof(T).ToString();
          
            var sql = string.Format(@"DELETE FROM {1} 
                                        WHERE {0}"
                                        , condition
                                        , typeName);
            IQuery qry = session.CreateQuery(sql);
            return qry.ExecuteUpdate();
            
        }

        public void Modify(T obj)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                session.SaveOrUpdate(obj);
                session.Flush();
            }
        }
        public void Modify(T obj, ISession session)
        {
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenSession();
            }
            session.SaveOrUpdate(obj);
            //session.Flush();
           
        }
        
        public bool IsExists(K key)
        {
            string typeName = typeof(T).ToString();
            using (ISession session = this.SessionFactory.OpenSession())
            {
                var sql = string.Format(@"SELECT COUNT(*) FROM {0} WHERE Key=:key", 
                    typeName);
                IQuery qry = session.CreateQuery(sql)
                                    .SetParameter<K>("key", key);
                return qry.UniqueResult<long>()>0;
            }
        }

        public bool IsExists(K key, ISession session)
        {
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenSession();
            }
            string typeName = typeof(T).ToString();          
            var sql = string.Format(@"SELECT COUNT(*) FROM {0} WHERE Key=:key",
                typeName);
            IQuery qry = session.CreateQuery(sql)
                                .SetParameter<K>("key", key);
            return qry.UniqueResult<long>() > 0;
           
        }

        public T Get(K key)
        {
            T obj = null;
            using (ISession session = this.SessionFactory.OpenSession())
            {
                obj = session.Get<T>(key);
                session.Evict(obj);
            }
            return obj;
        }

        public T Get(K key, ISession session)
        {
            T obj = null;
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenSession();
            }
            obj = session.Get<T>(key);
            session.Evict(obj);
            return obj;            
        }

        public IList<T> Get(ServiceCenter.Model.PagingConfig cfg)
        {
            IList<T> objList;
            using (ISession session = this.SessionFactory.OpenSession())
            {
                objList = session.GetList<T>(cfg);
            }
            return objList;
        }

        public IList<T> Get(ServiceCenter.Model.PagingConfig cfg, ISession session)
        {
            IList<T> objList;
            if(session==null ||session.IsConnected==false || session.IsOpen==false)
            {
                session = this.SessionFactory.OpenSession();
            }
            objList = session.GetList<T>(cfg);
            foreach(T obj in objList)
            {
                session.Evict(obj);
            }
            return objList;            
        }
    }


    public class CommonObjectDataEngine<T, K> :
        IDatabaseDataEngine<T, K> where T : BaseModel<K>
    {
        /// <summary>
        /// 数据库会话工厂。
        /// </summary>
        public ISessionFactory SessionFactory
        {
            get;
            set;
        }

        public CommonObjectDataEngine(ISessionFactory sf)
        {
            this.SessionFactory = sf;       
        }

        public void Insert(T obj)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                object identifier = session.Save(obj);
                session.Flush();
            }
        }

        public void Insert(T obj, ISession session)
        {
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenSession();
            }
            object identifier = session.Save(obj);
            //session.Flush();
        }

        public void Update(T obj)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                session.Update(obj);
                session.Flush();
            }
        }

        public void Update(T obj, ISession session)
        {
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenSession();
            }
            session.Update(obj);
            //session.Flush();

        }

        public void Delete(K key)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                T obj = session.Get<T>(key);
                session.Delete(obj);
                session.Flush();
            }
        }

        public void Delete(K key, ISession session)
        {
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenSession();
            }
            T obj = session.Get<T>(key);
            session.Delete(obj);
            //session.Flush();
        }

        public int DeleteByCondition(string condition)
        {
            string typeName = typeof(T).ToString();
            using (ISession session = this.SessionFactory.OpenSession())
            {
                var sql = string.Format(@"DELETE FROM {1} 
                                          WHERE {0}"
                                          , condition
                                          , typeName);
                IQuery qry = session.CreateQuery(sql);
                return qry.ExecuteUpdate();
            }
        }

        public int DeleteByCondition(string condition, ISession session)
        {
            string typeName = typeof(T).ToString();

            var sql = string.Format(@"DELETE FROM {1} 
                                        WHERE {0}"
                                        , condition
                                        , typeName);
            IQuery qry = session.CreateQuery(sql);
            return qry.ExecuteUpdate();

        }

        public void Modify(T obj)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                session.SaveOrUpdate(obj);
                session.Flush();
            }
        }
        public void Modify(T obj, ISession session)
        {
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenSession();
            }
            session.SaveOrUpdate(obj);
            //session.Flush();

        }

        public bool IsExists(K key)
        {
            string typeName = typeof(T).ToString();
            using (ISession session = this.SessionFactory.OpenSession())
            {
                var sql = string.Format(@"SELECT COUNT(*) FROM {0} WHERE Key=:key",
                    typeName);
                IQuery qry = session.CreateQuery(sql)
                                    .SetParameter<K>("key", key);
                return qry.UniqueResult<long>() > 0;
            }
        }

        public bool IsExists(K key, ISession session)
        {
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenSession();
            }
            string typeName = typeof(T).ToString();
            var sql = string.Format(@"SELECT COUNT(*) FROM {0} WHERE Key=:key",
                typeName);
            IQuery qry = session.CreateQuery(sql)
                                .SetParameter<K>("key", key);
            return qry.UniqueResult<long>() > 0;

        }

        public T Get(K key)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                return session.Get<T>(key);
            }
        }

        public T Get(K key, ISession session)
        {
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenSession();
            }
            return session.Get<T>(key);

        }

        public T Get(K key, IStatelessSession session)
        {
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenStatelessSession();
            }
            return session.Get<T>(key);

        }

        public IList<T> Get(ServiceCenter.Model.PagingConfig cfg)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                return session.GetList<T>(cfg);
            }
        }

        public IList<T> Get(ServiceCenter.Model.PagingConfig cfg, ISession session)
        {
            if (session == null || session.IsConnected == false || session.IsOpen == false)
            {
                session = this.SessionFactory.OpenSession();
            }
            return session.GetList<T>(cfg);

        }
    }
}
