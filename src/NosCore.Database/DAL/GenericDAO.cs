﻿//  __  _  __    __   ___ __  ___ ___  
// |  \| |/__\ /' _/ / _//__\| _ \ __| 
// | | ' | \/ |`._`.| \_| \/ | v / _|  
// |_|\__|\__/ |___/ \__/\__/|_|_\___| 
// 
// Copyright (C) 2019 - NosCore
// 
// NosCore is a free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mapster;
using NosCore.Core;
using NosCore.Data.Enumerations;
using Serilog;

namespace NosCore.Database.DAL
{
    public class GenericDao<TEntity, TDto> : IGenericDao<TDto> where TEntity : class
    {
        private readonly ILogger _logger;
        private readonly PropertyInfo _primaryKey;

        public GenericDao(ILogger logger)
        {
            _logger = logger;
            try
            {
                var pis = typeof(TDto).GetProperties();
                var exit = false;
                for (var index = 0; index < pis.Length && !exit; index++)
                {
                    var pi = pis[index];
                    var attrs = pi.GetCustomAttributes(typeof(KeyAttribute), false);
                    if (attrs.Length != 1)
                    {
                        continue;
                    }

                    exit = true;
                    _primaryKey = pi;
                }

                if (_primaryKey != null)
                {
                    return;
                }

                throw new KeyNotFoundException();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }
        }

        public SaveResult Delete(object dtokey)
        {
            var context = DataAccessHelper.Instance.CreateContext();

            var dbset = context.Set<TEntity>();

            if (dtokey is IEnumerable enumerable)
            {
                foreach (var dto in enumerable)
                {
                    object value;
                    try
                    {
                        value = _primaryKey.GetValue(dto, null);
                    }
                    catch
                    {
                        value = dto;
                    }

                    TEntity entityfound = null;
                    if (value is object[] objects)
                    {
                        entityfound = dbset.Find(objects);
                    }
                    else
                    {
                        entityfound = dbset.Find(value);
                    }

                    if (entityfound == null)
                    {
                        continue;
                    }

                    dbset.Remove(entityfound);
                    context.SaveChanges();
                }
            }
            else
            {
                object value;
                try
                {
                    value = _primaryKey.GetValue(dtokey, null);
                }
                catch
                {
                    value = dtokey;
                }

                var entityfound = dbset.Find(value);

                if (entityfound != null)
                {
                    dbset.Remove(entityfound);
                }
            }

            context.SaveChanges();

            return SaveResult.Saved;

        }

        public TDto FirstOrDefault(Expression<Func<TDto, bool>> predicate)
        {
            try
            {
                if (predicate == null)
                {
                    return default;
                }

                var context = DataAccessHelper.Instance.CreateContext();
                var dbset = context.Set<TEntity>();
                var ent = dbset.FirstOrDefault(predicate.ReplaceParameter<TDto, TEntity>());
                return ent.Adapt<TDto>();

            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
                return default;
            }
        }

        public SaveResult InsertOrUpdate(ref TDto dto)
        {
            try
            {
                var context = DataAccessHelper.Instance.CreateContext();

                var entity = dto.Adapt<TEntity>();
                var dbset = context.Set<TEntity>();

                var value = _primaryKey.GetValue(dto, null);
                TEntity entityfound = null;
                if (value is object[] objects)
                {
                    entityfound = dbset.Find(objects);
                }
                else
                {
                    entityfound = dbset.Find(value);
                }

                entity = entity.Adapt<TDto>().Adapt<TEntity>();
                if (entityfound != null)
                {
                    context.Entry(entityfound).CurrentValues.SetValues(entity);
                    context.SaveChanges();
                }

                if (value == null || entityfound == null)
                {
                    dbset.Add(entity);
                }

                context.SaveChanges();
                dto = entity.Adapt<TDto>();

                return SaveResult.Saved;

            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
                return SaveResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(IEnumerable<TDto> dtos)
        {
            try
            {
                var context = DataAccessHelper.Instance.CreateContext();

                context.ChangeTracker.AutoDetectChangesEnabled = false;

                var dbset = context.Set<TEntity>();
                var entitytoadd = new List<TEntity>();
                foreach (var dto in dtos)
                {
                    var entity = dto.Adapt<TEntity>();
                    var value = _primaryKey.GetValue(dto, null);

                    TEntity entityfound = null;
                    if (value is object[] objects)
                    {
                        entityfound = dbset.Find(objects);
                    }
                    else
                    {
                        entityfound = dbset.Find(value);
                    }

                    entity = entity.Adapt<TDto>().Adapt<TEntity>();
                    if (entityfound != null)
                    {
                        context.Entry(entityfound).CurrentValues.SetValues(entity);
                    }

                    if (value == null || entityfound == null)
                    {
                        //add in a temp list in order to avoid find(default(PK)) to find this element before savechanges
                        entitytoadd.Add(entity);
                    }


                    dbset.AddRange(entitytoadd);
                }

                context.ChangeTracker.AutoDetectChangesEnabled = true;
                context.SaveChanges();
                return SaveResult.Saved;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<TDto> LoadAll()
        {
            var context = DataAccessHelper.Instance.CreateContext();


            foreach (var t in context.Set<TEntity>())
            {
                yield return t.Adapt<TDto>();
            }

        }

        public IEnumerable<TDto> Where(Expression<Func<TDto, bool>> predicate)
        {
            var context = DataAccessHelper.Instance.CreateContext();
            var dbset = context.Set<TEntity>();
            var entities = Enumerable.Empty<TEntity>();
            try
            {
                entities = dbset.Where(predicate.ReplaceParameter<TDto, TEntity>()).ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }

            foreach (var t in entities)
            {
                yield return t.Adapt<TDto>();
            }
        }
    }
}