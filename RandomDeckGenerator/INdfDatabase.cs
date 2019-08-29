using System.Collections.Generic;

namespace DeckGenerator {
    public interface INdfDatabase {
        int GetActivationPoints(Countries country);
        Dictionary<Categories, List<int>> GetCostMatrix();
        int GetDefaultActivationPoints();
        List<WargameUnit> GetOtanUnits();
        List<WargameUnit> GetPactUnits();
        Dictionary<Categories, int> GetSlotsPerCategory();
        Dictionary<Categories, List<int>> GetUnfilteredCostMatrix();
    }
}