namespace MARINEYE.Utilities
{
    public enum BoatCalendarEventState
    {
        Reserved,
        Confirmed
    }

    static public class BoatCalendarEventStateUtils
    {
        static private Dictionary<BoatState, string> _boatCalendarEventStateDictionary = Enum.GetValues(typeof(BoatState))
                              .Cast<BoatState>()
                              .ToDictionary(state => state, state => state.ToString());

        static public string? GetBoatCalendarEventStateString(BoatState key) {
            if (!_boatCalendarEventStateDictionary.ContainsKey(key)) {
                return null;
            }

            return _boatCalendarEventStateDictionary[key];
        }

        static public List<string> GetBoatCalendarEventStateAllStrings() {
            if (_boatCalendarEventStateDictionary.Count == 0) {
                return new List<string>();
            }

            return _boatCalendarEventStateDictionary.Values.ToList();
        }
    }
}
