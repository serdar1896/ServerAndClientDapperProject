using CoreProject.BusinessLayer.Infrastructure;
using CoreProject.DataLayer.Helpers.Enum;
using CoreProject.DataLayer.Infrastructure;
using CoreProject.DataLayer.LogService;
using CoreProject.Entities.Infrastructure;
using CoreProject.Entities.VMModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CoreProject.BusinessLayer.Services
{
    public class BaseService<T> : IBaseService<T> where T : BaseEntity
    {
        readonly Commander _nlog;
        private readonly string _tableName;
        readonly IBaseRepository<T> _repository;

        public BaseService(Commander nlog, IBaseRepository<T> repository)
        {
            _nlog = nlog;
            _tableName = typeof(T).Name;
            _repository = repository;
        }

        public async Task<ServiceResponse<T>> GetAllAsync()
        {
            var response = new ServiceResponse<T>();
            try
            {
                response.List = await _repository.GetAll();

                if (response.List == null) throw new KeyNotFoundException($"{_tableName} Tablo'sunda hiç {ErrorCodes.KayitYok.Text}");

                response.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName}, Method:{MethodBase.GetCurrentMethod().Name}, Description: {response.ExceptionMessage}!", ex);
            }

            return response;

        }

        public async Task<ServiceResponse<T>> GetByIdAsync(int id)
        {
            if (id == 0) throw new ArgumentNullException("id", ErrorCodes.WrongParameter.Text);

            var response = new ServiceResponse<T>();
            try
            {
                response.Entity = await _repository.GetById(id);

                if (response.Entity == null) throw new KeyNotFoundException($"{_tableName} Tablosunda [{id}] Nolu {ErrorCodes.KayitYok.Text}");

                response.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName},  Method:{MethodBase.GetCurrentMethod().Name}, Description: {response.ExceptionMessage}!", ex);
            }
            return response;

        }

        public async Task<ServiceResponse<T>> GetByParamAsync(object param)
        {

            var response = new ServiceResponse<T>();
            try
            {
                var result = await _repository.GetByParam(param);

                if (result.Count() == 1) response.Entity = result.FirstOrDefault();

                else if (result.Count() > 1) response.List = result;

                else throw new KeyNotFoundException($"{_tableName} Tablo'sunda [{param}] ile ilgili {ErrorCodes.KayitYok.Text}");

                response.IsSuccessful = true;

            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName}, Method:{MethodBase.GetCurrentMethod().Name}, Description: {response.ExceptionMessage}!", ex);
            }
            return response;
        }


        public async Task<ServiceResponse<T>> DeleteRowAsync(int id)
        {
            if (id == 0) throw new ArgumentNullException("id", ErrorCodes.WrongParameter.Text);

            var response = new ServiceResponse<T>();
            try
            {
                int deleted = await _repository.DeleteRow(id);
                if (deleted != 0)
                {
                    _nlog.WriteLog(NLog.LogLevel.Info, $" Table: {_tableName} tablosunda [{id}] No'lu kayıt silindi.!");
                    response.IsSuccessful = true;
                }
                else throw new KeyNotFoundException($"{_tableName} Tablo'sunda [{id}] No'lu {ErrorCodes.KayitYok.Text}");

            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName}, Description:{ex.Message}!", ex);

            }
            return response;
        }


        public async Task<ServiceResponse<T>> InsertAsync(T entity)
        {
            var response = new ServiceResponse<T>();
            response.Entity = entity;
            if (entity == null) throw new ArgumentNullException("entity", "Add to DB null entity");
            try
            {
               
                int inserted = await _repository.Insert(entity);

                if (inserted != 0) response.IsSuccessful = true;

                else throw new KeyNotFoundException($"{_tableName} Tablo'sunda [{entity.Id}] No'lu {ErrorCodes.KayitYok.Text}");
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName}, Method:{MethodBase.GetCurrentMethod().Name}, Description:{ex.Message}!", ex);
            }

            return response;

        }


        public async Task<ServiceResponse<T>> InsertRangeAsync(IEnumerable<T> list)
        {
            if (list == null) throw new ArgumentNullException("entity", "Add to DB null entity");

            var response = new ServiceResponse<T>();
            response.List = list;

            try
            {
                int inserted = await _repository.InsertRange((dynamic)list);

                if (inserted == list.Count()) response.IsSuccessful = true;

                else if (inserted < list.Count()) new DataException($"{_tableName} Tablo'suna Kayıt edilmek istenen {list.Count()}, Başarılı olan {inserted}. {ErrorCodes.BilinmeyenHata.Text}");

                else throw new DataException($"{_tableName} Tablo'suna Insert edilirken {ErrorCodes.BilinmeyenHata.Text}");
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName}, Method:{MethodBase.GetCurrentMethod().Name}, Description:{ex.Message}!", ex);
            }
            return response;
        }

        public async Task<ServiceResponse<T>> UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity", "Add to DB null entity");

            var response = new ServiceResponse<T>();
            response.Entity = entity;

            try
            {
                var updated = await _repository.Update(entity);

                if (updated == 0) response.IsSuccessful = true;

                else throw new KeyNotFoundException($"{_tableName} Tablo'sunda [{entity.Id}] No'lu kayıt bulunamadı.!");

            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.ExceptionMessage = ex.Message;
                _nlog.WriteLog(NLog.LogLevel.Error, $" Table: {_tableName}, Method:{MethodBase.GetCurrentMethod().Name}, Description:{response.ExceptionMessage}!", ex);
            }
            return response;

        }

    }
}
