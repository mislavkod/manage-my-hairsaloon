using manage_my_hairsaloon.Models;

namespace manage_my_hairsaloon.Repositories
{
    public interface IHairSalonRepository
    {
        List<HairSalon> GetAll();
        HairSalon? GetById(int id);
        HairSalon? GetByName(string name);
    }
}
