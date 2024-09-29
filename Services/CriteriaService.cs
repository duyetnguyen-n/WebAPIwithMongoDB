using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Interface;

namespace WebAPIwithMongoDB.Services
{
    public class CriteriaService
    {
        private readonly ICriteriaRepository _criteriaRepository;
        private readonly ICriteriaGroupRepository _criteriaGroupRepository;

        public CriteriaService(ICriteriaRepository criteriaRepository, ICriteriaGroupRepository criteriaGroupRepository)
        {
            _criteriaRepository = criteriaRepository;
            _criteriaGroupRepository = criteriaGroupRepository;
        }

        public async Task<IEnumerable<Criteria>> GetCriteriasAsync()
        {
            return await _criteriaRepository.GetAsync();
        }

        public async Task<Criteria> GetCriteriaByIdAsync(string id)
        {
            return await _criteriaRepository.GetAsync(id);
        }

        public async Task<Criteria> AddCriteriaAsync(Criteria criteria)
        {
            if (criteria == null)
            {
                return null;  // Trả về null nếu dữ liệu không hợp lệ
            }
            var newCriteria = new Criteria
            {
                Name = criteria.Name,
                Points = criteria.Points,
                CriteriaGroupId = criteria.CriteriaGroupId,
                Notes = criteria.Notes,
                PersonCheck = criteria.PersonCheck,
                TimeStamp = DateTime.Now
            };
            await _criteriaRepository.CreateAsync(newCriteria);
            return newCriteria;
        }

        public async Task<Criteria> UpdateCriteriaAsync(Criteria criteria)
        {
            if (!await _criteriaRepository.Exists(criteria.Id))
            {
                return null;  // Trả về null nếu không tìm thấy
            }

            var criteriaOld = await _criteriaRepository.GetAsync(criteria.Id);
            if (criteriaOld.CriteriaGroupId != criteria.CriteriaGroupId)
            {
                await _criteriaGroupRepository.DecrementCriteriaGroupCount(criteriaOld.CriteriaGroupId);
                await _criteriaGroupRepository.IncrementCriteriaGroupCount(criteria.CriteriaGroupId);
            }

            criteriaOld.Name = criteria.Name;
            criteriaOld.Points = criteria.Points;
            criteriaOld.CriteriaGroupId = criteria.CriteriaGroupId;
            criteriaOld.Notes = criteria.Notes;
            criteriaOld.PersonCheck = criteria.PersonCheck;
            criteriaOld.TimeStamp = DateTime.Now;

            await _criteriaRepository.UpdateAsync(criteria.Id, criteriaOld);
            return criteriaOld;
        }

        public async Task<bool> DeleteCriteriaAsync(string id)
        {
            if (!await _criteriaRepository.Exists(id))
            {
                return false;  // Trả về false nếu không tìm thấy
            }

            await _criteriaRepository.DeleteAsync(id);
            return true;  // Trả về true khi xóa thành công
        }
    }
}
