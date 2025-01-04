namespace MARINEYE.Utilities
{
    public enum BoatState
    {
        Operational,
        Repair
    }

static public class BoatStateUtils
{
    static private Dictionary<BoatState, string> _boatStateDictionary = new Dictionary<BoatState, string>
    {
        { BoatState.Operational, "Sprawny" },
        { BoatState.Repair, "Naprawa" }
    };

static public string? GetBoatStateString(BoatState key) {
            if (!_boatStateDictionary.ContainsKey(key)) {
                return null;
            }

            return _boatStateDictionary[key];
        }

        static public List<string> GetBoatStateAllStrings() {
            if (_boatStateDictionary.Count == 0) {
                return new List<string>();
            }

            return _boatStateDictionary.Values.ToList();
        }
    }
}
