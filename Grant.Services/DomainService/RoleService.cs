using System;

namespace Grant.Services.DomainService
{
    using System.Collections.Generic;
    using Core.Entities;
    using Core.Enum;

    public class RoleService : IRoleService
    {
        private readonly IStudentService _studentService;

        /// <summary>
        /// Справочник видов разрешений
        /// </summary>
        private Dictionary<string, Dictionary<Role, List<EventType>>> _accessDictionary;

        /// <summary>
        /// Получение справочника видов разрешений
        /// </summary>
        private Dictionary<string, Dictionary<Role, List<EventType>>> GetAccessDictionary()
        {
            return _accessDictionary ?? (_accessDictionary = new Dictionary<string, Dictionary<Role, List<EventType>>>
            {
                {
                    typeof (Grant).ToString(),
                    new Dictionary<Role, List<EventType>>()
                    {
                        {
                            Role.Administrator, new List<EventType>()
                            {
                                EventType.FullAccess
                            }
                        },
                        {
                            Role.GrantAdministrator, new List<EventType>()
                            {
                                EventType.FullAccess
                            }
                        }
                    }
                },
                {
                    typeof (GrantQuota).ToString(),
                    new Dictionary<Role, List<EventType>>()
                    {
                        {
                            Role.Administrator, new List<EventType>()
                            {
                                EventType.FullAccess
                            }
                        },
                        {
                            Role.GrantAdministrator, new List<EventType>()
                            {
                                EventType.GrantQuotaChanged,
                            }
                        },
                        {
                            Role.UniversityCurator, new List<EventType>()
                            {
                                EventType.AttachWinnersReport
                            }
                        }
                    }
                },
                {
                    typeof (GrantStudent).ToString(),
                    new Dictionary<Role, List<EventType>>()
                    {
                        {
                            Role.Administrator, new List<EventType>()
                            {
                                EventType.FullAccess
                            }
                        },
                        { 
                            Role.GrantAdministrator, new List<EventType>()
                            {
                                EventType.FullAccess
                            }
                        },
                        {
                            Role.UniversityCurator, new List<EventType>()
                            {
                                EventType.GrantWinnerSelected,
                                EventType.GrantAdditionalWinnerSelected
                            }
                        },
                        {
                            Role.RegistredUser, new List<EventType>()
                            {
                                EventType.GrantUserRegister,
                                EventType.GrantUserCancel
                            }
                        },
                    }
                },
            }
                );
        }


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="studentService"></param>
        public RoleService(IStudentService studentService)
        {
            _studentService = studentService;
        }


        /// <summary>
        /// Проверка наличия прав на выполнение определенного действия
        /// </summary>
        /// <param name="obj">Сущность, над которым выполняется действие</param>
        /// <param name="eventType">Тип события, которое хотят выполнить</param>
        /// <returns>Результат выполнения</returns>
        public void CheckAccess(object obj, EventType eventType)
        {
            var roleInfo = _studentService.GetCurrentRole().Result;
            

            if (roleInfo.Role == Role.Administrator)
            {
                //админу всегда все можно
                return;
            }

            
            if (obj is Grant)
            {
                var grant = obj as Grant;

                //админ гранта может делать все что угодно с грантом
                if(roleInfo.GrantsAdmin.Contains(grant.Id))
                    return;
            }
            else if (obj is University)
            {
                var university = obj as University;
                if (roleInfo.UniversCurator.Contains(university.Id)
                    && GetAccessDictionary()[obj.ToString()][Role.UniversityCurator].Contains(eventType))
                    return;
            }
            else
            {
                throw new Exception("Недостаточно прав на данную операцию");
            }
        }
    }
}

