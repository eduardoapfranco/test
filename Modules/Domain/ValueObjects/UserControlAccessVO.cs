using Domain.Entities;
using System.Collections.Generic;

namespace Domain.ValueObjects
{
    public class UserControlAccessVO
    {        
        public UserControlAccessVO(UserPlans userPlans, Plan plan, Profile profile, IEnumerable<Category> categories, IEnumerable<Functionality> functionalities, IEnumerable<Area> areas = null)
        {
            UserPlans = userPlans;
            Plan = plan;
            Profile = profile;
            Categories = categories;
            Functionalities = functionalities;
            Areas = areas;
        }

        public UserPlans  UserPlans { get; set; }
        public Plan  Plan { get; set; }
        public Profile Profile { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Functionality> Functionalities { get; set; }
        public IEnumerable<Area> Areas { get; set; }
    }
}
