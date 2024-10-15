using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Interfaces.UoW;
using Domain.ValueObjects;
using Infra.CrossCutting.Domain.Services;
using Infra.CrossCutting.Notification.Interfaces;
using Infra.CrossCutting.Notification.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;

namespace Domain.Services
{
    public class ChecklistDomainService : DomainService<Checklist, int, IUnitOfWork>, IChecklistDomainService
    {
        private readonly IChecklistRepository _checklistRepository;
        private ISmartNotification _notification;
        private ILogger<ChecklistDomainService> _logger;

        public ChecklistDomainService(
           IChecklistRepository checklistRepository,
           ISmartNotification notification,
           IUnitOfWork unitOfWork,
           INotificationHandler<DomainNotification> messageHandler,
           ILogger<ChecklistDomainService> logger
       ) : base(checklistRepository, unitOfWork, messageHandler)
        {
            _checklistRepository = checklistRepository;
            _notification = notification;
            _logger = logger;
        }

        public async Task<IEnumerable<ChecklistSectionExportVO>> SelectExportSectionPDF(int categoryId, int[] ids, ExportTypeChecklistEnum type)
        {         

            var checklists = await SelectCaseExportSectionPDF(categoryId, ids, type);    
            return checklists;
        }

        private async Task<IEnumerable<ChecklistSectionExportVO>> SelectCaseExportSectionPDF(int categoryId, int[] ids, ExportTypeChecklistEnum type)
        {
            var checklistsVO = new List<ChecklistSectionExportVO>();

            IEnumerable<Checklist> checklists = await _repository.SelectFilterAsync(x => x.CategoryId.Equals(categoryId) && x.Active.Equals(1) && x.CheckEnable.Equals(1) && x.VisibleApp.Equals(1));
            checklists = checklists.OrderBy(x => x.Order);
            checklists = JoinGroupWithChecklists(checklists.ToList());

            if (type == ExportTypeChecklistEnum.CHECKS)
            {
                checklists = checklists.Where(x => ids.Contains(x.Id) || x.Type.Equals((int) ChecklistTypeEnum.GRUPO));
            }

            else if (type == ExportTypeChecklistEnum.UNCHECKS)
            {
                checklists = checklists.Where(x => !ids.Contains(x.Id) || x.Type.Equals((int)ChecklistTypeEnum.GRUPO));
            }

            var listChecklist = checklists as Checklist[] ?? checklists.ToArray();
            if (listChecklist.Any())
            {
                checklists = RemoveGroupExportWhenGroupDoesntChecklists(listChecklist.ToList());
                foreach (var check in checklists)
                {
                    checklistsVO.Add(new ChecklistSectionExportVO()
                    {
                        Id = check.Id,
                        Type = (ChecklistTypeEnum)check.Type,
                        Title = check.Title,
                        IsCheck = ValidateIsCheckedToPDF(ids.ToList(), check.Id, type),
                        GroupId = check.GroupId
                    });
                }
            }

            return checklistsVO;
        }

        private IEnumerable<Checklist> JoinGroupWithChecklists(List<Checklist> checklists)
        {
            var groupId = 0;
            for (int i = 0; i < checklists.Count(); i++)
            {
               
                if ((checklists[i].Type == (int) ChecklistTypeEnum.GRUPO))
                {
                    groupId = checklists[i].Id;
                    checklists[i].GroupId = 0;
                }

                if (checklists[i].Type != (int) ChecklistTypeEnum.GRUPO)
                {
                    checklists[i].GroupId = groupId;
                }
            }
            return checklists;
        }

        private IEnumerable<Checklist> RemoveGroupExportWhenGroupDoesntChecklists(List<Checklist> checklists)
        {
            var idGroups = checklists.Where(x => x.GroupId.HasValue && x.GroupId.Value.Equals(0) && x.Type.Equals((int)ChecklistTypeEnum.GRUPO)).Select(x => x.Id).ToList();

            for (int i = 0; i < idGroups.Count(); i++)
            {
                var validateGroups = checklists.Where(x => x.GroupId.HasValue && !x.Type.Equals((int)ChecklistTypeEnum.GRUPO) && x.GroupId.Value.Equals(idGroups[i]));
                if (!validateGroups.Any())
                {
                    var check = checklists.Where(x => x.Id.Equals(idGroups[i])).Select(x => x).FirstOrDefault();
                    if (check != null)
                    {
                        checklists.Remove(check);
                    }
                }
            }
            return checklists;
        }

        private bool ValidateIsCheckedToPDF(List<int> ids, int id, ExportTypeChecklistEnum type)
        {
            if (type == ExportTypeChecklistEnum.CHECKS)
                return true;
            else if (type == ExportTypeChecklistEnum.UNCHECKS)
                return false;

            return ids.Any(x => x.Equals(id));
        }
    }
}
