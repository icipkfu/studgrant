using Grant.Core.Enum;

namespace Grant.Services.DomainService
{

    using System.Collections.Generic;
    using Grant.Core.Entities;

    public class AchievementScore :IAchievementScore
    {
        public int GetScore(ICollection<Achievement> list)
        {
            var score = 0;


            foreach (var ach in list)
            {
                switch (ach.Subject)
                {

                        case AchievementSubject.SocialActivity:
                                switch (ach.Level)
                                {
                                    case AchievementLevel.University:
                                            switch (ach.State)
                                            {
                                               case AchievementState.Winner: score += 10; break;
                                               case AchievementState.Prize: score += 9; break;
                                               case AchievementState.Organizer: score += 8; break;
                                               case AchievementState.Participant: score += 7; break;
                                            }
                                    break;
                                    case AchievementLevel.City:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 12; break;
                                            case AchievementState.Prize: score += 11; break;
                                            case AchievementState.Organizer: score += 10; break;
                                            case AchievementState.Participant: score += 9; break;
                                        }
                                        break;
                            case AchievementLevel.Republic:
                                switch (ach.State)
                                {
                                    case AchievementState.Winner: score += 14; break;
                                    case AchievementState.Prize: score += 13; break;
                                    case AchievementState.Organizer: score += 12; break;
                                    case AchievementState.Participant: score += 11; break;
                                }
                                break;
                            case AchievementLevel.Russia:
                                switch (ach.State)
                                {
                                    case AchievementState.Winner: score += 16; break;
                                    case AchievementState.Prize: score += 15; break;
                                    case AchievementState.Organizer: score += 14; break;
                                    case AchievementState.Participant: score += 13; break;
                                }
                                break;
                            case AchievementLevel.International:
                                switch (ach.State)
                                {
                                    case AchievementState.Winner: score += 18; break;
                                    case AchievementState.Prize: score += 17; break;
                                    case AchievementState.Organizer: score += 16; break;
                                    case AchievementState.Participant: score += 15; break;
                                }
                                break;
                        }
                                break;


                        case AchievementSubject.Creation:
                        {

                            if (ach.Criterion == AchievementCriterion.Award)
                            {
                                switch (ach.Level)
                                {
                                    case AchievementLevel.University:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 10; break;
                                            case AchievementState.Prize: score += 9; break;
                                            case AchievementState.Participant: score += 7; break;
                                        }
                                        break;
                                    case AchievementLevel.City:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 12; break;
                                            case AchievementState.Prize: score += 11; break;
                                            case AchievementState.Participant: score += 9; break;
                                        }
                                        break;
                                    case AchievementLevel.Republic:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 14; break;
                                            case AchievementState.Prize: score += 13; break;
                                            case AchievementState.Participant: score += 11; break;
                                        }
                                        break;
                                    case AchievementLevel.Russia:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 16; break;
                                            case AchievementState.Prize: score += 15; break;
                                            case AchievementState.Participant: score += 13; break;
                                        }
                                        break;
                                    case AchievementLevel.International:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 18; break;
                                            case AchievementState.Prize: score += 17; break;
                                            case AchievementState.Participant: score += 15; break;
                                        }
                                        break;
                                }

                                if (ach.Criterion == AchievementCriterion.EventMaster)
                                {
                                    switch (ach.Level)
                                    {
                                        case AchievementLevel.University:
                                            switch (ach.State)
                                            {
                                                case AchievementState.Winner: score += 8; break;
                                                case AchievementState.Prize: score += 7; break;
                                                case AchievementState.Participant: score += 5; break;
                                            }
                                            break;
                                        case AchievementLevel.City:
                                            switch (ach.State)
                                            {
                                                case AchievementState.Winner: score += 10; break;
                                                case AchievementState.Prize: score += 9; break;
                                                case AchievementState.Participant: score += 7; break;
                                            }
                                            break;
                                        case AchievementLevel.Republic:
                                            switch (ach.State)
                                            {
                                                case AchievementState.Winner: score += 12; break;
                                                case AchievementState.Prize: score += 11; break;
                                                case AchievementState.Participant: score += 9; break;
                                            }
                                            break;
                                        case AchievementLevel.Russia:
                                            switch (ach.State)
                                            {
                                                case AchievementState.Winner: score += 14; break;
                                                case AchievementState.Prize: score += 13; break;
                                                case AchievementState.Participant: score += 11; break;
                                            }
                                            break;
                                        case AchievementLevel.International:
                                            switch (ach.State)
                                            {
                                                case AchievementState.Winner: score += 16; break;
                                                case AchievementState.Prize: score += 15; break;
                                                case AchievementState.Participant: score += 13; break;
                                            }
                                            break;
                                    }



                                }
                            }
                        }
                        break;

                        case AchievementSubject.Sport:
                        {
                            if (ach.Criterion == AchievementCriterion.Award)
                            {
                                switch (ach.Level)
                                {
                                    case AchievementLevel.University:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 10; break;
                                            case AchievementState.Prize: score += 9; break;
                                            case AchievementState.Participant: score += 7; break;
                                        }
                                        break;
                                    case AchievementLevel.City:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 12; break;
                                            case AchievementState.Prize: score += 11; break;
                                            case AchievementState.Participant: score += 9; break;
                                        }
                                        break;
                                    case AchievementLevel.Republic:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 14; break;
                                            case AchievementState.Prize: score += 13; break;
                                            case AchievementState.Participant: score += 11; break;
                                        }
                                        break;
                                    case AchievementLevel.Russia:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 16; break;
                                            case AchievementState.Prize: score += 15; break;
                                            case AchievementState.Participant: score += 13; break;
                                        }
                                        break;
                                    case AchievementLevel.International:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 18; break;
                                            case AchievementState.Prize: score += 17; break;
                                            case AchievementState.Participant: score += 15; break;
                                        }
                                        break;
                                }
                            }

                            if (ach.Criterion == AchievementCriterion.SportEvent)
                            {
                                switch (ach.Level)
                                {
                                    case AchievementLevel.University:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 8; break;
                                            case AchievementState.Prize: score += 7; break;
                                            case AchievementState.Participant: score += 5; break;
                                        }
                                        break;
                                    case AchievementLevel.City:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 10; break;
                                            case AchievementState.Prize: score += 9; break;
                                            case AchievementState.Participant: score += 7; break;
                                        }
                                        break;
                                    case AchievementLevel.Republic:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 12; break;
                                            case AchievementState.Prize: score += 11; break;
                                            case AchievementState.Participant: score += 9; break;
                                        }
                                        break;
                                    case AchievementLevel.Russia:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 14; break;
                                            case AchievementState.Prize: score += 13; break;
                                            case AchievementState.Participant: score += 11; break;
                                        }
                                        break;
                                    case AchievementLevel.International:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 16; break;
                                            case AchievementState.Prize: score += 15; break;
                                            case AchievementState.Participant: score += 13; break;
                                        }
                                        break;
                                }


                            }

                            if (ach.Criterion == AchievementCriterion.Gto)
                            {
                                switch (ach.State)
                                {

                                    case AchievementState.Gold: score += 20; break;
                                    case AchievementState.Silver: score += 18; break;
                                    case AchievementState.Participant: score += 16; break;
                                }


                            }


                        }
                        break;

                        case AchievementSubject.Science:


                        {
                            if (ach.Criterion == AchievementCriterion.Award)
                            {
                                switch (ach.Level)
                                {
                                    case AchievementLevel.University:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 6; break;
                                            case AchievementState.Prize: score += 5; break;
                                            case AchievementState.Participant: score += 3; break;
                                        }
                                        break;
                                    case AchievementLevel.City:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 8; break;
                                            case AchievementState.Prize: score += 7; break;
                                            case AchievementState.Participant: score += 5; break;
                                        }
                                        break;
                                    case AchievementLevel.Republic:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 10; break;
                                            case AchievementState.Prize: score += 9; break;
                                            case AchievementState.Participant: score += 7; break;
                                        }
                                        break;
                                    case AchievementLevel.Russia:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 12; break;
                                            case AchievementState.Prize: score += 11; break;
                                            case AchievementState.Participant: score += 9; break;
                                        }
                                        break;
                                    case AchievementLevel.International:
                                        switch (ach.State)
                                        {
                                            case AchievementState.Winner: score += 14; break;
                                            case AchievementState.Prize: score += 13; break;
                                            case AchievementState.Participant: score += 11; break;
                                        }
                                        break;
                                }
                            }

                            if (ach.Criterion == AchievementCriterion.Patent)
                            {
                                score += 40;
                            }

                            if (ach.Criterion == AchievementCriterion.GrantResearchWork)
                            {
                                score += 20;
                            }

                            if (ach.Criterion == AchievementCriterion.ScientificPublication)
                            {
                                score += 8;
                            }

                            if (ach.Criterion == AchievementCriterion.ScientificPublicationRu)
                            {
                                score += 12;
                            }

                            if (ach.Criterion == AchievementCriterion.ScientificPublicationWorld)
                            {
                                score += 15;
                            }
                        } break;

                    case AchievementSubject.StateAwards:
                        {
                            switch (ach.Level)
                            {
                                case AchievementLevel.CityHead: score += 30; break;
                                case AchievementLevel.CityMayor: score += 50; break;
                                case AchievementLevel.RegionDepartmentHead: score += 90; break;
                                case AchievementLevel.GovernmentHead: score += 120; break;
                                case AchievementLevel.LegislativeAssemblyChairman: score += 150; break;
                                case AchievementLevel.RepublicPresident: score += 300; break;
                                case AchievementLevel.President: score += 500; break;
                            }
                        }
                        break;
                }

              
                
            }

            return score;
        }
    }
}
