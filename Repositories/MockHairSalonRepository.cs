using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public class MockHairSalonRepository
    {
        private readonly List<HairSalon> _salons = MockDataStore.HairSalons;

        public List<HairSalon> GetAll()
        {
            return _salons;
        }

        public HairSalon? GetById(int id)
        {
            return _salons.FirstOrDefault(s => s.Id == id);
        }

        public HairSalon? GetByName(string name)
        {
            return _salons.FirstOrDefault(s => s.Name == name);
        }
    }
}
