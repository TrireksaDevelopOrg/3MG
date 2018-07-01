﻿using MySql.Data.MySqlClient;
using Ocph.DAL.DbContext;
using Ocph.DAL.ExpressionHandler;
using Ocph.DAL.Mapping;
using Ocph.DAL.QueryBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Ocph.DAL.Provider.MySql
{
    internal class MySqlDbContext<T>:IDataTable<T>
    {
        public EntityInfo Entity { get; set; }
        public IDbConnection connection { get; set; }

        public MySqlDbContext(IDbConnection con)
        {
            this.Entity = new EntityInfo(typeof(T));
            connection = con;
        }

        public bool Insert(T t)
        {
            try
            {
                IDbCommand cmd = connection.CreateCommand();
                cmd.Parameters.Clear();
                var iq = new InsertQuery(Entity);

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = iq.GetQuerywithParameter(t);
                iq.SetParameter(ref cmd, t);
                 var result = (Int32)cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (MySqlException ex)
            {
                throw new System.Exception(Helpers.ErrorHandle(ex));
            }
        }

        public bool Delete(Expression<Func<T, bool>> Predicate)
        {
            bool result = false;
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = new DeleteQuery(Entity).GetQuery(Predicate);

            try
            {
                if (cmd.ExecuteNonQuery() > 0)
                    result = true;

            }
            catch (MySqlException ex)
            {
                throw new System.Exception(Helpers.ErrorHandle(ex));
            }
            return result;
        }

        public bool Update(Expression<Func<T, dynamic>> fieldUpdate, Expression<Func<T, bool>> whereClause, object source)
        {

            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            bool result = false;
            var Query = new UpdateQuery(Entity);
            cmd.CommandText = Query.GetQueryWithParameter(fieldUpdate, whereClause, source);
            Query.SetParameters(cmd);


            try
            {
                if (cmd.ExecuteNonQuery() > 0)
                    result = true;

            }
            catch (MySqlException ex)
            {
                throw new System.Exception(ex.Message);

            }
            return result;


        }


        public IQueryable<T> Select(Expression<Func<T, bool>> expression)
        {
            List<T> list = new List<T>();
            StringBuilder sb = new StringBuilder();

            sb.Append("(Select * From ").Append(Entity.TableName).Append(" Where ");

            sb.Append(new WhereTranslator().Translate(expression));

            sb.Append(")");
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sb.ToString();
            IDataReader dr = null;
            try
            {
                dr = cmd.ExecuteReader() as MySqlDataReader;
                var map = new MappingColumn(Entity);
                list=map.MappingWithoutInclud<T>(dr);
            }
            catch (MySqlException ex)
            {

                throw new SystemException(ex.Message);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return list.AsQueryable();
        }


        public IQueryable<T> SelectAll()
        {
            List<T> list = new List<T>();
            StringBuilder sb = new StringBuilder();
            sb.Append("Select * From ").Append(Entity.TableName);
            IDataReader dr = null;
            try
            {
                IDbCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sb.ToString();
                dr = cmd.ExecuteReader();
                list = new MappingColumn(Entity).MappingWithoutInclud<T>(dr);
            }
            catch (MySqlException ex)
            {
                throw new SystemException(ex.Message);

            }
            finally
            {
                if(dr!=null)
                     dr.Close();
            }
            return list.AsQueryable<T>();
        }



        public IQueryable<T> Select(Expression<Func<T, dynamic>> expression)
        {
            EntityInfo entity = new EntityInfo(typeof(T));
            List<T> list = new List<T>();
            StringBuilder sb = new StringBuilder();
            var job = new CollectPropertyFromExpression().Translate(expression);
            sb.Append("Select ");
            if (job.Count < 1)
                sb.Append(" * ");
            else
            {
                int count = job.Count;
                for (int i = 0; i < count; i++)
                {
                    var att = entity.GetAttributDbColumn(job[i]);

                    sb.Append(string.Format("{0}", att));
                    if (i < count)
                    {
                        sb.Append(", ");
                    }
                }

            }
            string temp = sb.ToString();
            sb.Clear();

            temp = temp.Substring(0, temp.Length - 2);
            sb.Append(temp + " ");
            sb.Append(" From ").Append(Entity.TableName);

            //    sb.Append(new WhereTranslator().Translate(expression));
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sb.ToString();
            IDataReader dr = null;
            try
            {
                dr = cmd.ExecuteReader() as MySqlDataReader;
                var map= new MappingColumn(Entity);
                list=map.MappingWithoutInclud<T>(dr);
            }
            catch (MySqlException ex)
            {
                throw new  SystemException(ex.Message);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return list.AsQueryable();
        }



        public IQueryable<T> Includ(IQueryable<T> query, Expression<Func<T, dynamic>> expression)
        {
            var job = new CollectPropertyFromExpression().Translate(expression);

            foreach (T Item in query)
            {
                foreach (PropertyInfo propertyJOb in job)
                {
                    EntityInfo entityChild = null;
                    if (propertyJOb.PropertyType.GenericTypeArguments.Count() > 0)
                    {
                        entityChild = new EntityInfo(propertyJOb.PropertyType.GenericTypeArguments[0]);
                        string vsql = new InsertQuery(Entity).GetChildInsertQuery(propertyJOb, Item, entityChild);
                        if (vsql != string.Empty)
                        {

                            IDataReader dr = null;
                            try
                            {
                                IDbCommand cmd = connection.CreateCommand();
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandText = vsql;
                                dr = cmd.ExecuteReader();

                                var propertyproduct = Entity.GetPropertyByPropertyName(propertyJOb.Name);
                                IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(entityChild.GetEntityType()));
                                var map = new MappingColumn(Entity);
                                var resultMapping = (IList)map.MappingWithoutInclud(dr, entityChild.GetEntityType());

                                foreach (var item in resultMapping)
                                {
                                    list.Add(item);
                                }

                                propertyproduct.SetValue(Item, list, null);
                            }
                            catch (Exception ex)
                            {
                                throw new System.Exception(ex.Message);
                            }
                            finally
                            {
                                if (dr != null)
                                    dr.Close();
                            }
                        }
                    }
                    else
                    {
                        entityChild = new EntityInfo(propertyJOb.ReflectedType);
                    }



                }
            }
            return query;
        }



        public IQueryable<T> ExecuteStoreProcedureQuery(string storeProcedure)
        {
            List<T> list = new List<T>();
            IDataReader dr = null;
            try
            {
                IDbCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "CallEmployee";
                dr = cmd.ExecuteReader();
                list = new MappingColumn(Entity).MappingWithoutInclud<T>(dr);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);

            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return list.AsQueryable<T>();
        }

        public object ExecuteStoreProcedureNonQuery(string storeProcedure)
        {
            try
            {
                IDbCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storeProcedure;
                var a = cmd.ExecuteNonQuery();
                return a;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);

            }

        }



        public int GetLastID(T t)
        {
            int result = 0;
            try
            {
                IDbCommand cmd = connection.CreateCommand();
                cmd.Parameters.Clear();
                var iq = new InsertQuery(Entity);

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = iq.GetQuerywithParameter(t) + "; select Last_Insert_ID();";
                iq.SetParameter(ref cmd, t);
                result = Convert.ToInt32(cmd.ExecuteScalar());

            }
            catch (MySqlException ex)
            {
                throw new System.Exception(Helpers.ErrorHandle(ex));
            }

            return result;
        }

        public object GetLastItem()
        {
            List<T> list = new List<T>();
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select * From " + Entity.TableName + " Order By " + Entity.GetAttributPrimaryKeyName() + " Desc Limit 1";
            IDataReader dr = null;
            try
            {
                dr = cmd.ExecuteReader() as MySqlDataReader;
                list = new MappingColumn(Entity).MappingWithoutInclud<T>(dr);
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return list.FirstOrDefault();
        }
        
        public object ExecuteNonQuery(string query)
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            bool result = false;
            cmd.CommandText = query;
            try
            {
                if (cmd.ExecuteNonQuery() > 0)
                    result = true;

            }
            catch (Exception ex)
            {
                throw new System.Exception(ex.Message);

            }
            return result;
        }

        public IDataReader ExecuteQuery(string Query)
        {
            List<T> list = new List<T>();
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = Query;
            IDataReader dr = null;
            try
            {
                dr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
            return dr;
        }
    }
}
