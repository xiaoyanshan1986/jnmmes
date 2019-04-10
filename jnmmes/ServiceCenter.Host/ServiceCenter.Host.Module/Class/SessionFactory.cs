using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Cfg;

namespace ServiceCenter.Host.Module
{
    /// <summary>
    /// 
    /// </summary>
    public class SessionFactoryProxy : ISessionFactory
    {
        private ISessionFactory _sessionFactory;
         /// <summary>
        /// 构造函数。
        /// </summary>
        public SessionFactoryProxy():this(string.Empty)
        {
           
        }
        /// <summary>
        /// 构造函数。
        /// </summary>
        public SessionFactoryProxy(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string cfgFileName = HttpContext.Current.Server.MapPath(fileName);
                Configuration cfg = new Configuration().Configure(cfgFileName);
                this._sessionFactory = cfg.BuildSessionFactory();
            }
            else
            {
                Configuration cfg = new Configuration().Configure();
                this._sessionFactory = cfg.BuildSessionFactory();
            }
        }

        #region ISessionFactory 成员

        public void Close()
        {
            this._sessionFactory.Close();
        }

        public ICollection<string> DefinedFilterNames
        {
            get { return this._sessionFactory.DefinedFilterNames; }
        }

        public void Evict(Type persistentClass, object id)
        {
            this._sessionFactory.Evict(persistentClass, id);
        }

        public void Evict(Type persistentClass)
        {
            this._sessionFactory.Evict(persistentClass);
        }

        public void EvictCollection(string roleName, object id)
        {
            this._sessionFactory.EvictCollection(roleName, id);
        }

        public void EvictCollection(string roleName)
        {
            this._sessionFactory.EvictCollection(roleName);
        }

        public void EvictEntity(string entityName, object id)
        {
            this._sessionFactory.EvictEntity(entityName, id);
        }

        public void EvictEntity(string entityName)
        {
            this._sessionFactory.EvictEntity(entityName);
        }

        public void EvictQueries(string cacheRegion)
        {
            this._sessionFactory.EvictQueries(cacheRegion);
        }

        public void EvictQueries()
        {
            this._sessionFactory.EvictQueries();
        }

        public IDictionary<string, NHibernate.Metadata.IClassMetadata> GetAllClassMetadata()
        {
            return this._sessionFactory.GetAllClassMetadata();
        }

        public IDictionary<string, NHibernate.Metadata.ICollectionMetadata> GetAllCollectionMetadata()
        {
            return this._sessionFactory.GetAllCollectionMetadata();
        }

        public NHibernate.Metadata.IClassMetadata GetClassMetadata(string entityName)
        {
            return this._sessionFactory.GetClassMetadata(entityName);
        }

        public NHibernate.Metadata.IClassMetadata GetClassMetadata(Type persistentClass)
        {
            return this._sessionFactory.GetClassMetadata(persistentClass);
        }

        public NHibernate.Metadata.ICollectionMetadata GetCollectionMetadata(string roleName)
        {
            return this._sessionFactory.GetCollectionMetadata(roleName);
        }

        public ISession GetCurrentSession()
        {
            return this._sessionFactory.GetCurrentSession();
        }

        public NHibernate.Engine.FilterDefinition GetFilterDefinition(string filterName)
        {
            return this._sessionFactory.GetFilterDefinition(filterName);
        }

        public bool IsClosed
        {
            get { return this._sessionFactory.IsClosed; }
        }

        public ISession OpenSession()
        {
            return this._sessionFactory.OpenSession();
        }

        public ISession OpenSession(System.Data.IDbConnection conn, IInterceptor sessionLocalInterceptor)
        {
            return this._sessionFactory.OpenSession(conn, sessionLocalInterceptor);
        }

        public ISession OpenSession(IInterceptor sessionLocalInterceptor)
        {
            return this._sessionFactory.OpenSession(sessionLocalInterceptor);
        }

        public ISession OpenSession(System.Data.IDbConnection conn)
        {
            return this._sessionFactory.OpenSession(conn);
        }

        public IStatelessSession OpenStatelessSession(System.Data.IDbConnection connection)
        {
            return this._sessionFactory.OpenStatelessSession(connection);
        }

        public IStatelessSession OpenStatelessSession()
        {
            return this._sessionFactory.OpenStatelessSession();
        }

        public NHibernate.Stat.IStatistics Statistics
        {
            get { return this._sessionFactory.Statistics; }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            this._sessionFactory.Dispose();
        }

        #endregion
    }
}